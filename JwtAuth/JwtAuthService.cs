using IoT_SmartPlant_Portal.JwtAuth.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace IoT_SmartPlant_Portal.JwtAuth {

    public interface IJwtAuthService {
        /// <summary>
        /// creates token with lifespan duration expressed in hours
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="userId"></param>
        /// <param name="duration">duration expressed in hours</param>
        /// <returns></returns>
        public string CreateToken(string userId, double duration);
        /// <summary>
        /// hash given password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password);
        /// <summary>
        /// compares hash password to input password
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <param name="password"></param>
        /// <returns>returns if or not the password is correct</returns>
        public (bool Verified, bool NeedsUpgrade) VerifyPassword(string hashedPassword, string password);
        /// <summary>
        /// returns id from first part of the JBT payload
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetIdFromToken(string token);
    }

    public class JwtAuthService : IJwtAuthService {
        private readonly JwTokenCreation _jwTokenCreation;

        public JwtAuthService(string secret) {
            _jwTokenCreation = new JwTokenCreation(secret);
        }

        public string CreateToken(string userId, double duration) {
            return this._jwTokenCreation.CreateToken(userId, duration);
        }

        public string HashPassword(string password) {
            PasswordHasher passwordHasher = new PasswordHasher();
            return passwordHasher.Hash(password);
        }

        public (bool Verified, bool NeedsUpgrade) VerifyPassword(string hashedPassword, string password) {
            PasswordHasher passwordHasher = new PasswordHasher();
            return passwordHasher.Check(hashedPassword, password);
        }

        public string GetIdFromToken(string token) {
            var parts = token.Split(' ', 2);
            var stream = parts[1];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            JwtSecurityToken tokenS = (JwtSecurityToken)handler.ReadToken(stream);

            string id = tokenS.Claims.First(claim => claim.Type == "unique_name").Value;
            return id;
        }
    }
}