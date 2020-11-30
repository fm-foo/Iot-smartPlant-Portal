using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IoT_SmartPlant_Portal.Identity.Models {
    public class AppUser {

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password_hash")]
        public string PasswordHash { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("devices")]
        public List<Guid> Devices { get; set; }

        [JsonConstructor]
        public AppUser(Guid id, string email, string passwordHash, string token) {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            Token = token;
            Devices = new List<Guid>();
        }

        public AppUser(string email) {
            Id = Guid.NewGuid();
            Email = email;
            Devices = new List<Guid>();
        }
    }
}
