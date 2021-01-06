using IoT_SmartPlant_Portal.Identity.Managers;
using IoT_SmartPlant_Portal.Identity.Models;
using IoT_SmartPlant_Portal.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IoT_SmartPlant_Portal.Application.Builders {
    public static class IdentityBuilderSetup {

        /// <summary>
        /// Configure and setup custom Identity for ASP.NET Core.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static void IdentityBuilder(this IServiceCollection services) {
            BuildAppUser(services);
        }

        private static void BuildAppUser(this IServiceCollection services) {
            IdentityBuilder builder = services.AddIdentityCore<AppUser>(opt => {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
            });
            builder.AddUserManager<MyUserManager<AppUser>>();
            builder.AddSignInManager<SignInManager<AppUser>>();
            builder.AddUserStore<MyUserStore>();
        }

    }
}
