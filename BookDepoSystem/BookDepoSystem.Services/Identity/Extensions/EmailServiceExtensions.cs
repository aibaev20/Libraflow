using System.Threading.Tasks;
using BookDepoSystem.Services.Common.Constants;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;
using Essentials.Results;

namespace BookDepoSystem.Services.Identity.Extensions;

public static class EmailServiceExtensions
{
    public static async Task<StandardResult> SendResetPasswordEmailAsync(
        this IEmailService emailService,
        string email,
        string token)
    {
        Console.WriteLine(token);

        return await emailService.SendEmailAsync(
            new EmailModel
            {
                Email = email,
                Subject = BookDepoSystem.Common.T.ResetPasswordTitle,
                Message = string.Format($"{BookDepoSystem.Common.Emails.ResetPassword} ", token),
            },
            EmailSenderStrategies.SendGrid);
    }
}