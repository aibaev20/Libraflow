using System.Globalization;
using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
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

public class RentController : Controller
{
    private readonly IRentService rentService;
    private readonly EntityContext context;
    private readonly UserManager<ApplicationUser> userManager;

    public RentController(IRentService rentService, EntityContext context, UserManager<ApplicationUser> userManager)
    {
        this.rentService = rentService;
        this.context = context;
        this.userManager = userManager;
    }

    [HttpGet("/rents")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Rents(string status = "", string search = "", int page = 1, int pageSize = 5)
    {
        if (page < 1)
        {
            return this.RedirectToAction(nameof(this.Rents), new { page = 1, pageSize });
        }

        var (books, totalCount) = await this.rentService.GetRentsPaginated(status, search, page, pageSize);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (page > totalPages && totalPages > 0)
        {
            return this.RedirectToAction(nameof(this.Rents), new { status, search, page = totalPages, pageSize });
        }

        var viewModel = new PaginationViewModel<Rent>
        {
            Items = books,
            PageIndex = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            TotalCount = totalCount,
        };

        this.ViewData["Status"] = status;
        this.ViewData["Search"] = search;
        this.ViewData["PageSize"] = pageSize;

        return this.View(viewModel);
    }

    [HttpGet("/rents/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public IActionResult Create()
    {
        this.ViewBag.Books = new SelectList(this.context.Books, "BookId", "Title");
        this.ViewBag.Renters = new SelectList(this.context.Renters, "RenterId", "Name");
        this.ViewBag.Statuses = new SelectList(new[] { "Confirmed", "Active", "Completed", "Overdue" });
        return this.View(new RentViewModel());
    }

    [HttpPost("/rents/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Create(RentViewModel rentViewModel)
    {
        if (this.ModelState.IsValid)
        {
            var book = await this.context.Books.FindAsync(rentViewModel.BookId);
            string? userId = this.userManager.GetUserId(this.User);

            if (rentViewModel.RentDate == DateTime.MinValue)
            {
                this.ModelState.AddModelError(
                    nameof(rentViewModel.RentDate),
                    Common.T.RentDateIsInvalidErrorMessage);
            }

            if (rentViewModel.DueDate == DateTime.MinValue)
            {
                this.ModelState.AddModelError(
                    nameof(rentViewModel.DueDate),
                    Common.T.DueDateIsInvalidErrorMessage);
            }

            if (rentViewModel.RentDate > rentViewModel.DueDate)
            {
                this.ModelState.AddModelError(
                    nameof(rentViewModel.DueDate),
                    Common.T.DueDateMustBeGreaterThanRentDate);
            }

            if (book == null || book.QuantityAvailable <= 0)
            {
                this.ModelState.AddModelError(
                    nameof(rentViewModel.BookId),
                    Common.T.SelectedBookNotAvailable);
            }
            else
            {
                var rent = new Rent
                {
                    RentDate = rentViewModel.RentDate,
                    DueDate = rentViewModel.DueDate,
                    BookId = rentViewModel.BookId,
                    RenterId = rentViewModel.RenterId,
                    AdminId = Guid.Parse(userId!),
                };
                await this.rentService.AddRentAsync(rent);
                return this.RedirectToAction(nameof(this.Rents));
            }
        }

        this.ViewBag.Books = new SelectList(this.context.Books, "BookId", "Title");
        this.ViewBag.Renters = new SelectList(this.context.Renters, "RenterId", "Name");
        return this.View(rentViewModel);
    }

    [HttpGet("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Return(Guid rentId)
    {
        var rent = await this.rentService.GetRentById(rentId);

        if (rent == null)
        {
            return this.NotFound();
        }

        var rentViewModel = new RentViewModel
        {
            RentId = rent.RentId,
            RentDate = rent.RentDate,
            DueDate = rent.DueDate,
            ReturnDate = rent.ReturnDate,
            Status = rent.Status,
            BookId = rent.BookId!.Value,
            BookTitle = rent.Book!.Title,
            RenterId = rent.RenterId!.Value,
            RenterName = rent.Renter!.Name,
        };

        return this.View(rentViewModel);
    }

    [HttpPost("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Return(RentViewModel rentViewModel)
    {
        if (rentViewModel.ReturnDate == DateTime.MinValue)
        {
            this.ModelState.AddModelError(
                nameof(rentViewModel.ReturnDate),
                Common.T.ReturnDateIsInvalidErrorMessage);
        }

        if (rentViewModel.ReturnDate < rentViewModel.RentDate)
        {
            this.ModelState.AddModelError(
                nameof(rentViewModel.ReturnDate),
                Common.T.ReturnDateMustBeGreaterThanRentDate);
        }

        if (this.ModelState.IsValid)
        {
            var success = await this.rentService.UpdateReturnDateAsync(rentViewModel.RentId, rentViewModel.ReturnDate);
            if (success)
            {
                return this.RedirectToAction(nameof(this.Rents));
            }
        }

        return this.View(rentViewModel);
    }

    [HttpGet("/rents/my-assigned-rents")]
    [Authorize(DefaultPolicies.UserPolicy)]
    public async Task<IActionResult> MyAssignedRents()
    {
        var userId = this.userManager.GetUserId(this.User);

        var renter = await this.context.Renters.FirstOrDefaultAsync(r => r.RenterId.ToString() == userId);

        var rents = await this.context.Rents
            .Include(r => r.Book)
            .Where(r => r.RenterId == renter!.RenterId)
            .ToListAsync();

        return View(rents);
    }
}