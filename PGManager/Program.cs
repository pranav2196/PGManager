using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PGManager.DataAccess.Data;
using PGManager.DataAccess.SeedData;

namespace PGManager
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
					var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
					var signinManager = services.GetRequiredService<SignInManager<IdentityUser>>();
					var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
					context.Database.Migrate();
					await SeedDataInitializer.SeedData(context, userManager,signinManager,roleManager);
                }
                catch (Exception ex)
                {
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occured during migration");
				}
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
