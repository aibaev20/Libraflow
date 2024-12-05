using System;
using System.Threading.Tasks;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;
using BookDepoSystem.Services.Common.Options;
using Essentials.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BookDepoSystem.Services.Common.Internals.EmailSenders;

internal class SendGridSender : IEmailSender
{
    private readonly IOptions<EmailSendGridOptions> optionsAccessor;
    private readonly ILogger<SendGridSender> logger;

    public SendGridSender(
        IOptions<EmailSendGridOptions> optionsAccessor,
        ILogger<SendGridSender> logger)
    {
        this.optionsAccessor = optionsAccessor;
        this.logger = logger;
    }

    public async Task<StandardResult> SendEmailAsync(EmailModel model)
    {
        try
        {
            var options = this.optionsAccessor.Value;
            var client = new SendGridClient(options.ApiKey);
            var from = new EmailAddress(options.Email, options.Name);
            var to = new EmailAddress(model.Email);
            var message = MailHelper.CreateSingleEmail(from, to, model.Subject, plainTextContent: null, model.Message);
            var response = await client.SendEmailAsync(message);
            return StandardResult.ResultFrom(response.IsSuccessStatusCode);
        }
        catch (Exception ex)
        {
            var errorMessage = "An error occured while sending email via SendGrid.";
            this.logger.LogError(ex, errorMessage);
            return StandardResult.UnsuccessfulResult(errorMessage);
        }
    }
}