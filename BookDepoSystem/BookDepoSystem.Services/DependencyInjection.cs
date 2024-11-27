using BookDepoSystem.Services.Common;
using BookDepoSystem.Services.Identity;
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
        
        return services;
    }
}