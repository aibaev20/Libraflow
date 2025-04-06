using BookDepoSystem.Data;
using BookDepoSystem.Services.Identity.Contracts;
using BookDepoSystem.Services.Identity.Internals;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services.Identity;

internal static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<EntityContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}