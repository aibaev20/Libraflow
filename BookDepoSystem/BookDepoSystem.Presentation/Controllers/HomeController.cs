using System.Diagnostics;
using BookDepoSystem.Data;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;
using BookDepoSystem.Services.Identity.Constants;
using BookDepoSystem.Services.Identity.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookDepoSystem.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IEmailService emailService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger)
    {
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.userManager = userManager;
        this.logger = logger;
    }

    [HttpGet("/")]
    [Authorize(DefaultPolicies.AdminPolicy)]
    // scheme cookies
    // to see Index first must be logged in
    public async Task<IActionResult> Index(string emailSender = "SendGrid")
    {
        /*
        var emailSent = await this.emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to BookDepoSystem!",
                Email = "AIBaev20@codingburgas.bg",
                Message = $"You have received email with strategy {emailSender}.",
            },
            emailSender);

        return this.Ok(emailSent);
        */

        return this.View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }

    public IActionResult Privacy()
    {
        return this.View(this.Privacy());
    }
}