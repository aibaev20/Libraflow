using BookDepoSystem.Services.Common;
using BookDepoSystem.Services.Identity;
using BookDepoSystem.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommonServices(configuration);
        services.AddIdentityServices();
        services.AddServices();

        return services;
    }
}