using IoT_SmartPlant_Portal.Identity.Models;
using IoT_SmartPlant_Portal.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Identity.Managers {

    public class MyUserManager<TUser> : UserManager<AppUser> {
        private readonly IMyUserStore _myUserStore;

        public MyUserManager(IMyUserStore myUserStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) : base(myUserStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
            _myUserStore = myUserStore;
        }


        /// <summary>
        /// Creates the specified user in the backing store with given password
        /// </summary>
        /// <param name="appUser"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> CreateUserAsync(AppUser appUser, string password) {
            IdentityResult result = await CreateAsync(appUser, password);
            if (result.Succeeded) {
                return true;
            }
            return false;
        }
    }
}
