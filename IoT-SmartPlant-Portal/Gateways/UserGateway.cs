using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using IoT_SmartPlant_Portal.Identity.Models;
using IoT_SmartPlant_Portal.Services;

namespace IoT_SmartPlant_Portal.Gateways {
    public interface IUserGateway {
        /// <summary>
        /// Creates user to database with default role "admin" // TODO: FM: A User should never be Admin pr. default. I should be able to pass in parameters for the roles
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken);
        /// <summary>
        /// Delete said user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken);
        /// <summary>
        /// Find User by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AppUser> FindByEmailAsync(string email, CancellationToken cancellationToken);
        /// <summary>
        /// Find User by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken);
        /// <summary>
        /// update user by id
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken);
        /// <summary>
        /// Return all users
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken);
    }

    public class UserGateway : IUserGateway {
        private readonly IMySqlService _mySqlDatabase;

        public UserGateway(IMySqlService mySqlDatabase) {
            _mySqlDatabase = mySqlDatabase;
        }


        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            _mySqlDatabase.StartTransaction();

            string commandText = @"INSERT INTO users (id, email, password_hash)
                                   VALUES (@id, @email, @password_hash);";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@id", user.Id },
                { "@email", user.Email },
                { "@password_hash", user.PasswordHash }
            };

            int rowsAffectedInUserTable = await _mySqlDatabase.ExecuteAsync(commandText, parameters);

            if (rowsAffectedInUserTable == 0) {
                _mySqlDatabase.Rollback();
                return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.Email}." });
            }

            foreach (var device in user.Devices) {
                commandText = @"INSERT INTO user_device (user_id, device_id)
                                VALUES (@user_id, @device_id);";

                parameters = new Dictionary<string, object> {
                    { "@user_id", user.Id },
                    { "@device_id", device.ToString() }
                };

                var rowsAffectedInUserRoleTable = await _mySqlDatabase.ExecuteAsync(commandText, parameters);

                if (rowsAffectedInUserRoleTable == 0) {
                    _mySqlDatabase.Rollback();
                    return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.Email}." });
                }

            }

            _mySqlDatabase.Commit();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            string commandText = @"DELETE FROM users WHERE id = @id;";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@id", user.Id }
            };

            //return rows affected if >= 1 == success
            var rowsAffected = await _mySqlDatabase.ExecuteAsync(commandText, parameters);

            if (rowsAffected >= 1) {
                return IdentityResult.Success;
            } else {
                return IdentityResult.Failed(new IdentityError { Description = $"Could not Delete user {user.Email}." });
            }

        }

        public async Task<AppUser> FindByEmailAsync(string email, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            string commandText = @"SELECT *
                                FROM users
                                WHERE email = @email";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@email", email }
                };

            List<Dictionary<string, string>> dict = await _mySqlDatabase.QueryAsync(commandText, parameters);

            if (dict.Count == 0) {
                // IMPORTANT!!! that when user does not exist , it must return null
                return null;
            }

            string jsonString = JsonConvert.SerializeObject(dict[0]);
            AppUser user = JsonConvert.DeserializeObject<AppUser>(jsonString);

            string commandText2 = @"SELECT device_id
                                FROM user_device
                                WHERE user_id = @user_id";

            Dictionary<string, object> parameters2 = new Dictionary<string, object> {
                    { "@user_id", user.Id }
                };

            List<Dictionary<string, string>> dict2 = await _mySqlDatabase.QueryAsync(commandText2, parameters2);

            if (dict2.Count == 0) {
                // IMPORTANT!!! that when user does not exist , it must return null
                return null;
            }

            foreach (var item in dict2) {
                user.Devices.Add(Guid.Parse(item["device_id"]));
            }


            return user;
        }

        public async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            string commandText = @"SELECT *
                                FROM users
                                WHERE id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@id", userId }
            };

            List<Dictionary<string, string>> user = await _mySqlDatabase.QueryAsync(commandText, parameters);

            // in this case if user.Count() is equal to zero , it only means that a user with that email does not exist in our DB
            if (user.Count == 0) {
                throw new ArgumentException("User with ID: " + userId + " does not exist");
            }
            string jsonString = JsonConvert.SerializeObject(user[0]);
            AppUser deserializeObject = JsonConvert.DeserializeObject<AppUser>(jsonString);
            return deserializeObject;
        }

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            string commandText = @"UPDATE users
                                   SET id = @id, email = @email, password_hash = @password_hash
                                   WHERE id = @id;";


            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@id", user.Id },
                { "@email", user.Email },
                { "@password_hash", user.PasswordHash }
            };

            //return rows affected if >= 1 == success
            var rowsAffected = await _mySqlDatabase.ExecuteAsync(commandText, parameters);

            if (rowsAffected >= 1) {
                return IdentityResult.Success;
            } else {
                return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.Email}." });
            }
        }

        public async Task<List<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            string commandText = @"SELECT *
                                FROM users";

            List<Dictionary<string, string>> dictionaryOfAllUsers = await _mySqlDatabase.QueryAsync(commandText);

            //in this case if user.Count() is equal to zero , it only means that a user with that email does not exist in our DB
            if (dictionaryOfAllUsers.Count > 0) {
                //using Newtonsoft.Json , we can serilize the Dctionary into a Json string
                List<AppUser> ListOfAllUsers = new List<AppUser>();

                foreach (var item in dictionaryOfAllUsers) {
                    string jsonString = JsonConvert.SerializeObject(item);
                    AppUser deserializeObject = JsonConvert.DeserializeObject<AppUser>(jsonString);
                    ListOfAllUsers.Add(deserializeObject);
                }
                return ListOfAllUsers;
            }
            return null;
        }
    }
}
