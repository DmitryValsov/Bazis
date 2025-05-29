// Data/IdentityDataSeeder.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Bazis.Data
{
    public static class IdentityDataSeeder
    {
        private static readonly string[] Roles = { "Administrator", "User" };
        private const string AdminEmail    = "admin@gmail.com";
        private const string AdminPassword = "Admin123123!";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            if (await userManager.FindByEmailAsync(AdminEmail) is null)
            {
                var admin = new IdentityUser
                {
                    UserName       = AdminEmail,
                    Email          = AdminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, AdminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }
    }
}
