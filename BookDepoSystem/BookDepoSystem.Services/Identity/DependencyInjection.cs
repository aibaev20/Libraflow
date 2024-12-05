using BookDepoSystem.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services.Identity;

internal static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<EntityContext>();

        return services;
    }
}