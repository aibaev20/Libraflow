using System.Globalization;
using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Presentation.Extensions;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Identity.Constants;
using BookDepoSystem.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Presentation.Controllers;

public class FeedbackController : Controller
{
    private readonly IFeedbackService feedbackService;
    private readonly IBookService bookService;
    private readonly IRenterService renterService;
    private readonly EntityContext context;
    private readonly UserManager<ApplicationUser> userManager;

    public FeedbackController(IFeedbackService feedbackService, IBookService bookService, IRenterService renterService, EntityContext context, UserManager<ApplicationUser> userManager)
    {
        this.feedbackService = feedbackService;
        this.bookService = bookService;
        this.renterService = renterService;
        this.context = context;
        this.userManager = userManager;
    }

    [HttpGet("/feedback/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.UserPolicy)]
    public async Task<IActionResult> Create(Guid bookId, Guid renterId)
    {
        //var book = await this.context.Books.FindAsync(bookId);
        //var renter = await this.context.Renters.FindAsync(renterId);
        var book = await this.bookService.GetBookById(bookId);
        var renter = await this.renterService.GetRenterById(renterId);

        if (book == null || renter == null)
        {
            return NotFound();
        }

        var model = new FeedbackViewModel
        {
            BookId = book.BookId,
            BookTitle = book.Title!,
            RenterId = renter.RenterId,
        };

        return this.View(model);
    }

    [HttpPost("/feedback/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.UserPolicy)]
    public async Task<IActionResult> Create(FeedbackViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return this.View(model);
        }

        var newFeedback = new Feedback
        {
            FeedbackId = Guid.NewGuid(),
            BookId = model.BookId,
            RenterId = model.RenterId,
            CreatedOn = DateTime.UtcNow,
            Rate = model.Rate,
            Message = model.Message,
        };

        await this.feedbackService.AddFeedback(newFeedback);
        return this.RedirectToMyAssignedRents();
        //return this.RedirectToAction("MyAssignedRents", "Rent");
    }
}