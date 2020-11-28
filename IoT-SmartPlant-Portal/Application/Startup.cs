using API.Application.Builders;
using IoT_SmartPlant_Portal.Application.Configuration;
using IoT_SmartPlant_Portal.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.AddScoped<IMQTTBroker>(mqtt => new MQTTBroker(launchConfig));

            services.AddSwaggerSetup();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.AddSwaggerSetup();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
