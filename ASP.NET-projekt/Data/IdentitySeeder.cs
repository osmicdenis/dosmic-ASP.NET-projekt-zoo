using ASP.NET_projekt.Models;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_projekt.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in new[] { "Admin", "Manager", "User" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            async Task EnsureUserAsync(string userName, string email, string password, string role, string oib, string jmbg)
            {
                var existing = await userManager.FindByEmailAsync(email);
                if (existing == null)
                {
                    var user = new AppUser
                    {
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true,
                        OIB = oib,
                        JMBG = jmbg
                    };

                    var createResult = await userManager.CreateAsync(user, password);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(existing, role))
                    {
                        await userManager.AddToRoleAsync(existing, role);
                    }
                }
            }

            // Seeded test users (passwords meet configured requirements)
            await EnsureUserAsync("admin@zoo.test", "admin@zoo.test", "Admin123!", "Admin", "00000000001", "0000000000001");
            await EnsureUserAsync("manager@zoo.test", "manager@zoo.test", "Manager123!", "Manager", "00000000002", "0000000000002");
            await EnsureUserAsync("user@zoo.test", "user@zoo.test", "User123!", "User", "00000000003", "0000000000003");
        }
    }
}