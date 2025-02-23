using BookDepoSystem.Data;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Identity.Contracts;
using BookDepoSystem.Services.Identity.Internals;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services.Implementations;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}