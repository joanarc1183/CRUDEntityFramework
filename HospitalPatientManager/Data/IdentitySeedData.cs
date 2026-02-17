using HospitalPatientManager.Models;
using Microsoft.AspNetCore.Identity;

namespace HospitalPatientManager.Data;

public static class IdentitySeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        const string adminRole = "Admin";
        const string adminUserName = "admin";
        const string adminPassword = "admin123";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                DisplayName = "Administrator",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {errors}");
            }
        }
        else if (!await userManager.CheckPasswordAsync(adminUser, adminPassword))
        {
            var removeResult = await userManager.RemovePasswordAsync(adminUser);
            if (!removeResult.Succeeded)
            {
                string errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to reset admin password: {errors}");
            }

            var addResult = await userManager.AddPasswordAsync(adminUser, adminPassword);
            if (!addResult.Succeeded)
            {
                string errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to set admin password: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
