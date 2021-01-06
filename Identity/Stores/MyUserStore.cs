using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoT_SmartPlant_Portal.Gateways;
using IoT_SmartPlant_Portal.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace IoT_SmartPlant_Portal.Identity.Stores {

    public interface IMyUserStore : IUserStore<AppUser>, IUserEmailStore<AppUser>, IUserPhoneNumberStore<AppUser>,
    IUserTwoFactorStore<AppUser>, IUserPasswordStore<AppUser>, IUserRoleStore<AppUser> {
        Task<List<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<IdentityResult> ActivateUserAsync(string id, CancellationToken cancellationToken);
    }

    public class MyUserStore : IMyUserStore {

        private readonly IUserGateway _userGateway;

        public MyUserStore(IUserGateway userGateway) {
            _userGateway = userGateway;
        }

        /// <summary>
        /// Creates user with default role "admin"
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return await _userGateway.CreateAsync(user, cancellationToken);
        }
        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return await _userGateway.DeleteAsync(user, cancellationToken);
        }
        /// <summary>
        /// uådate user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return await _userGateway.UpdateAsync(user, cancellationToken);
        }
        /// <summary>
        /// find and returns user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            AppUser appUser = await _userGateway.FindByIdAsync(userId, cancellationToken);
            if (appUser != null) {
                return appUser;
            }
            return null;
        }
        /// <summary>
        /// finds and returns user by normalizedUserName (normalizedUserName = Email.ToUpper())
        /// </summary>
        /// <param name="normalizedUserName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return await _userGateway.FindByEmailAsync(normalizedUserName, cancellationToken);

        }
        /// <summary>
        /// finds and returns user by Email
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AppUser> FindByEmailAsync(string Email, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            AppUser appUser = await _userGateway.FindByEmailAsync(Email, cancellationToken);
            if (appUser != null) {
                return appUser;
            }
            return null;
        }


        /// <summary>
        /// Returns user id 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Id.ToString());
        }
        /// <summary>
        /// returns email from user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email.ToString());
        }
        /// <summary>
        /// set normalizedName, this case normalizedName = email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken) {
            user.Email = normalizedName;
            return Task.FromResult(0);
        }
        /// <summary>
        /// return user username (username = email)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email.ToString());
        }

        /// <summary>
        /// return password hash
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);
        }
        /// <summary>
        /// set normalizedName to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="normalizedEmail"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken) {
            user.Email = normalizedEmail;
            return Task.FromResult(0);
        }
        /// <summary>
        /// set password hash to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(AppUser user, CancellationToken cancellationToken) {
            return Task.FromResult(false);
        }

        public Task<List<AppUser>> GetAllUsersAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ActivateUserAsync(string id, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(AppUser user, string phoneNumber, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            // nothing to dispose of
        }
    }
}
