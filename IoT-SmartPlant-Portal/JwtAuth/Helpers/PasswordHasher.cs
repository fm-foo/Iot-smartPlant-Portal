using IoT_SmartPlant_Portal.JwtAuth.Models;
using System;
using System.Linq;
using System.Security.Cryptography;


namespace IoT_SmartPlant_Portal.JwtAuth.Helpers {
    public class PasswordHasher {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        readonly HashingOptions Options = new HashingOptions();

        public PasswordHasher() {
        }

        /// <summary>
        /// Returns the inputed password that has been hashed with a random salt
        /// </summary>
        /// <param name="password"></param>
        /// <returns>returns the password that has been hashed with a random salt</returns>
        public string Hash(string password) {
            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              SaltSize,
              Options.Iterations,
              HashAlgorithmName.SHA512)) {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{Options.Iterations}.{salt}.{key}";
            }
        }

        /// <summary>
        /// Compares both paswword and hash to check if the same, returns if it has been verified and if password needs to be upgraded
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password) {

            try {
                var parts = hash.Split('.', 3);

                if (parts.Length != 3) {
                    throw new FormatException("Unexpected hash format. " +
                      "Should be formatted as `{iterations}.{salt}.{hash}`");
                }


                var iterations = Convert.ToInt32(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var key = Convert.FromBase64String(parts[2]);

                var needsUpgrade = iterations != Options.Iterations;

                using (var algorithm = new Rfc2898DeriveBytes(
                  password,
                  salt,
                  iterations,
                  HashAlgorithmName.SHA512)) {
                    var keyToCheck = algorithm.GetBytes(KeySize);

                    var verified = keyToCheck.SequenceEqual(key);

                    return (verified, needsUpgrade);
                }
            } catch (Exception ex) {
                throw ex;
            }

        }
    }
}
