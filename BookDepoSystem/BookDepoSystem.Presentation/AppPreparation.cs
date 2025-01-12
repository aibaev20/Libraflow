using BookDepoSystem.Data;
using BookDepoSystem.Services.Identity.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Presentation;

public static class AppPreparation
{
    public static async Task PrepareAsync(this IApplicationBuilder app)
    {
        try
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();

            await dbContext.Database.MigrateAsync();

            if (!await dbContext.Roles.AnyAsync())
            {
                using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                foreach (var role in DefaultRoles.List)
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role,
                    });
                }
            }

            if (!await dbContext.Users.AnyAsync())
            {
                using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = new ApplicationUser
                {
                    UserName = InitialAdminCredentials.AdminEmail,
                    Email = InitialAdminCredentials.AdminEmail,
                    EmailConfirmed = true,
                };
                var adminCreatedResult = await userManager.CreateAsync(
                    user,
                    InitialAdminCredentials.AdminPassword);
                if (adminCreatedResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, DefaultRoles.Admin);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}