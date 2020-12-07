using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IoT_SmartPlant_Portal.JwtAuth.Helpers {
    public class JwTokenCreation {
        //This is the secret key
        private readonly string _secret;

        public JwTokenCreation(string secret) {
            _secret = secret;
        }

        /// <summary>
        /// Returns Token with Claims of the inputed data
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string CreateToken(string id, double duration) {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, id)
                        };

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(duration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
