using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Internals;
using BookDepoSystem.Services.Common.Internals.EmailSenders;
using BookDepoSystem.Services.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSendGridOptions>(configuration.GetSection("Emails:SendGrid"));
        services.AddKeyedScoped<IEmailSender, SendGridSender>("SendGrid");
        
        services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
}