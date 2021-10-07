using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           var host = CreateHostBuilder(args).Build();
           using var scope = host.Services.CreateScope();
            try{
                var dbcontext = scope.ServiceProvider.GetRequiredService<DatatContext>();
                var usermgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var rolemgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                await dbcontext.Database.MigrateAsync();
                
                await Seed.SeedData(usermgr, rolemgr);
            }
            catch(Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");

                logger.LogError(ex.Message);
            }
           await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
