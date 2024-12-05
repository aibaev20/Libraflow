using System;
using System.Threading.Tasks;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;
using Essentials.Results;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Services.Common.Internals;

internal class EmailService : IEmailService
{
    private readonly IServiceProvider serviceProvider;

    public EmailService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<StandardResult> SendEmailAsync(EmailModel model, string senderStrategy)
    {
        var sender = this.serviceProvider.GetKeyedService<IEmailSender>(senderStrategy);

        if (sender == null)
        {
            return StandardResult
                .UnsuccessfulResult($"There is no registered email sender strategy: '{senderStrategy}'");
        }

        return await sender.SendEmailAsync(model);
    }
}