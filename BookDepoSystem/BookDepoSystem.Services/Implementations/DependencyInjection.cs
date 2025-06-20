﻿using BookDepoSystem.Data;
using BookDepoSystem.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services.Implementations;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IRenterService, RenterService>();
        services.AddScoped<IRentService, RentService>();

        return services;
    }
}