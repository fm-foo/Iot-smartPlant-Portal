using IoT_SmartPlant_Portal.Application.Builders;
using IoT_SmartPlant_Portal.Application.Configuration;
using IoT_SmartPlant_Portal.Gateways;
using IoT_SmartPlant_Portal.Identity.Stores;
using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IoT_SmartPlant_Portal.JwtAuth;

namespace IoT_SmartPlant_Portal.Application {
    public class Startup {
        public IConfiguration Configuration { get; }
        private readonly LaunchConfiguration launchConfig;

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
            launchConfig = new LaunchConfiguration();
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            /******************************* move this later security reasons*************************************/

            // TODO: Fix this this cant be here
            // for testing only 
            string secret = "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING";

            byte[] key = Encoding.UTF8.GetBytes(secret);
            services.AddAuthentication(authOptions => {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions => {
                jwtBearerOptions.RequireHttpsMetadata = false;
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IJwtAuthService>(jwtService => new JwtAuthService(secret));

            /********************************************************************************************************/

            services.AddScoped<IMQTTBroker>(mqtt => new MQTTBroker(launchConfig));
            services.AddScoped<IMyUserStore, MyUserStore>();
            services.AddScoped<IUserGateway, UserGateway>();
            // TODO: Fix this string cant be here
            // for testing only 
            services.AddTransient<IMySqlService>(mySqlService => new MySqlService("server=207.154.226.178;user=user;password=eAT-Hqf_JwZCbnwY9AA*;database=users;Connection Timeout=30"));

            services.IdentityBuilder();

            services.AddSwaggerSetup();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //set the use of Authentication middleware on our app
            app.UseAuthentication();
            //set the use of Authorization middleware on our app
            app.UseAuthorization();

            app.AddSwaggerSetup();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
