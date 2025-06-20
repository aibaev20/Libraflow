using System.Diagnostics;
using System.Globalization;
using BookDepoSystem.Data;
using BookDepoSystem.Presentation.Extensions;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Common.Models;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Identity.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<HomeController> logger;
    private readonly EntityContext context;
    private readonly IWebHostEnvironment environment;
    private readonly IRentService rentService;

    public HomeController(
        IEmailService emailService,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger,
        EntityContext context,
        IWebHostEnvironment environment,
        IRentService rentService)
    {
        this.emailService = emailService;
        this.userManager = userManager;
        this.logger = logger;
        this.context = context;
        this.environment = environment;
        this.rentService = rentService;
    }

    [HttpGet("/")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    // scheme cookies
    // to see Index first must be logged in
    public async Task<IActionResult> Index()
    {
        var availableBooksCount = this.context.Books.Count(b => b.QuantityAvailable > 0);
        var allBooksCount = this.context.Books.Count();

        var popularBooks = this.context.Rents
            .GroupBy(r => r.BookId)
            .Select(g => new
            {
                BookTitle = g.First().Book!.Title,
                RentCount = g.Count(),
            })
            .OrderByDescending(b => b.RentCount)
            .Take(3)
            .ToList();

        var mostActiveRenters = this.context.Rents
            .GroupBy(r => r.RenterId)
            .Select(g => new
            {
                RenterName = g.First().Renter!.Name,
                RentCount = g.Count(),
            })
            .OrderByDescending(b => b.RentCount)
            .Take(3)
            .ToList();

        var completedRents = this.context.Rents
            .Count(r => r.Status == "Завършен");
        var allRentsCount = this.context.Rents.Count();

        var monthlyRents = this.context.Rents
            .GroupBy(r => r.RentDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                RentCount = g.Count(),
            })
            .OrderBy(x => x.Month)
            .ToList();

        this.ViewBag.MonthlyRentData = monthlyRents;

        this.ViewBag.AvailableBooksCount = availableBooksCount;
        this.ViewBag.AllBooksCount = allBooksCount;
        this.ViewBag.PopularBooks = popularBooks;
        this.ViewBag.MostActiveRenters = mostActiveRenters;
        this.ViewBag.CompletedRents = completedRents;
        this.ViewBag.AllRentsCount = allRentsCount;

        return this.View();
    }

    [HttpGet("/ExportMonthlyRentsToPdf")]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> ExportMonthlyRentsToPdf()
    {
        var pdfResult = await this.rentService.ExportMonthlyRentsPdfAsync();
        this.Response.Headers["Content-Disposition"] = $"inline; filename={pdfResult.FileName}";
        this.Response.Headers["Content-Type"] = "application/pdf";
        return File(pdfResult.File!, "application/pdf");
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