using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;

namespace BookDepoSystem.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IEmailService emailService,
        ILogger<HomeController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(string emailSender = "SendGrid")
    {
        var emailSent = await _emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to BookDepoSystem!",
                Email = "AIBaev20@codingburgas.bg",
                Message = $"You have received email with strategy {emailSender}."
            },
            emailSender);
        
        return Ok(emailSent);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}