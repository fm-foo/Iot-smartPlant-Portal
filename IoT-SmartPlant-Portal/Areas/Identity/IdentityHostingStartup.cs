using System;
using IoT_SmartPlant_Portal.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IoT_SmartPlant_Portal.Areas.Identity.IdentityHostingStartup))]
namespace IoT_SmartPlant_Portal.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IoT_SmartPlant_PortalContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("IoT_SmartPlant_PortalContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<IoT_SmartPlant_PortalContext>();
            });
        }
    }
}