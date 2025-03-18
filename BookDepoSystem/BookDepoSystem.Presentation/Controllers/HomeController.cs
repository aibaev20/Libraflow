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
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<HomeController> logger;
    private readonly EntityContext context;

    public HomeController(
        IEmailService emailService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger,
        EntityContext context)
    {
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.userManager = userManager;
        this.logger = logger;
        this.context = context;
    }

    [HttpGet("/")]
    [Authorize(DefaultPolicies.AdminPolicy)]
    // scheme cookies
    // to see Index first must be logged in
    public async Task<IActionResult> Index()
    {
        var availableBooksCount = this.context.Books.Where(b => b.QuantityAvailable > 0).Count();
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
            .Where(r => r.Status == "Overdue")
            .Count();
        var allRentsCount = this.context.Rents.Count();

        this.ViewBag.AvailableBooksCount = availableBooksCount;
        this.ViewBag.AllBooksCount = allBooksCount;
        this.ViewBag.PopularBooks = popularBooks;
        this.ViewBag.MostActiveRenters = mostActiveRenters;
        this.ViewBag.CompletedRents = completedRents;
        this.ViewBag.AllRentsCount = allRentsCount;

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