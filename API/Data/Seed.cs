using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedData(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
        {
            if( await userManager.Users.AnyAsync()) return;

            var data = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(data);

            Console.WriteLine(data);
            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
                // using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Dunda"));
                // user.PasswordSalt = hmac.Key;
                await userManager.CreateAsync(user, "Dunda@123");
                await userManager.AddToRoleAsync(user, "Member");

                Console.WriteLine("USER:");
                Console.WriteLine(user);
            }

            var adminUser = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(adminUser, "Dunda@321");
            await userManager.AddToRolesAsync(adminUser, new[] {"Admin", "Moderator"});

        }
    }
}