using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
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
                var adminUser = new ApplicationUser
                {
                    UserName = InitialAdminCredentials.AdminEmail,
                    Email = InitialAdminCredentials.AdminEmail,
                    EmailConfirmed = true,
                };
                var adminCreatedResult = await userManager.CreateAsync(
                    adminUser,
                    InitialAdminCredentials.AdminPassword);
                if (adminCreatedResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, DefaultRoles.Admin);
                }

                var regularUser = new ApplicationUser
                {
                    UserName = InitialUserCredentials.UserName,
                    Email = InitialUserCredentials.UserEmail,
                    PhoneNumber = InitialUserCredentials.UserPhoneNumber,
                    EmailConfirmed = true,
                };
                var userCreatedResult = await userManager.CreateAsync(
                    regularUser,
                    InitialUserCredentials.UserPassword);
                if (userCreatedResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, DefaultRoles.User);

                    var renter = new Renter
                    {
                        RenterId = regularUser.Id,
                        Name = regularUser.UserName,
                        Email = regularUser.Email,
                        PhoneNumber = regularUser.PhoneNumber,
                    };

                    dbContext.Renters.Add(renter);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}