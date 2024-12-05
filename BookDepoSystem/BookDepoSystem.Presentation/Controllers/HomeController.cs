using System.Diagnostics;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookDepoSystem.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IEmailService emailService,
        ILogger<HomeController> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(string emailSender = "SendGrid")
    {
        var emailSent = await this.emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to BookDepoSystem!",
                Email = "AIBaev20@codingburgas.bg",
                Message = $"You have received email with strategy {emailSender}.",
            },
            emailSender);

        return this.Ok(emailSent);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}