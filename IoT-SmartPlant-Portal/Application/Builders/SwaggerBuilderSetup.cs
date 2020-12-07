using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IoT_SmartPlant_Portal.Application.Builders {
    public static class SwaggerBuilderSetup {

        /// <summary>
        /// Setup Swagger Generator.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appSettingsConfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerSetup(this IServiceCollection services) {
            // Swagger setup
            services.AddSwaggerGen(setup => {
                setup.SwaggerDoc(
                    "v1.0",
                    new OpenApiInfo {
                        Title = "Smart Plant API",
                        Version = "v1.0"
                    });
            });
            return services;
        }

        /// <summary>
        /// Setup Swagger UI with information from the application.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appSettingsConfig">AppSettings configuration</param>
        /// <returns></returns>
        public static IApplicationBuilder AddSwaggerSetup(this IApplicationBuilder app) {
            app.UseSwagger();
            app.UseSwaggerUI(x => {
                x.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Smart Plant API v1.0");
            });

            return app;
        }
    }
}
