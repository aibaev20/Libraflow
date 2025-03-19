using System.Globalization;
using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Identity.Constants;
using BookDepoSystem.Services.Identity.Contracts;
using BookDepoSystem.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    public async Task<IActionResult> Rents()
    {
        var rents = await this.rentService.GetAllRentsAsync();
        var rentViewModels = rents.Select(r => new RentViewModel
        {
            RentId = r.RentId,
            RentDate = r.RentDate,
            DueDate = r.DueDate,
            ReturnDate = r.ReturnDate,
            Status = r.Status,
            BookId = r.BookId!.Value,
            BookTitle = r.Book!.Title,
            RenterId = r.RenterId!.Value,
            RenterName = r.Renter!.Name,
        }).ToList();
        return this.View(rentViewModels);
    }

    [HttpGet("/rents/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult Create()
    {
        this.ViewBag.Books = new SelectList(this.context.Books, "BookId", "Title");
        this.ViewBag.Renters = new SelectList(this.context.Renters, "RenterId", "Name");
        this.ViewBag.Statuses = new SelectList(new[] { "Confirmed", "Active", "Completed", "Overdue" });
        return this.View(new RentViewModel());
    }

    /*[HttpPost("/rents/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create(RentViewModel rentViewModel)
    {
        if (this.ModelState.IsValid)
        {
            var book = await this.context.Books.FindAsync(rentViewModel.BookId);
            string? userId = this.userManager.GetUserId(this.User);

            if (book == null || book.QuantityAvailable <= 0)
            {
                // Add validation error for BookId
                this.ModelState.AddModelError("BookId", "The selected book is not available for rent.");
            }
            else
            {
                var rent = new Rent
                {
                    RentDate = rentViewModel.RentDate!.Value, // RentDate = rentViewModel.RentDate!.Value
                    DueDate = rentViewModel.DueDate!.Value, // DueDate = rentViewModel.DueDate!.Value,
                    BookId = rentViewModel.BookId,
                    RenterId = rentViewModel.RenterId,
                    AdminId = Guid.Parse(userId!),
                };
                await this.rentService.AddRentAsync(rent);
                return this.RedirectToAction(nameof(this.Rents));
            }
        }

        // Repopulate the dropdowns if validation fails
        this.ViewBag.Books = new SelectList(this.context.Books, "BookId", "Title");
        this.ViewBag.Renters = new SelectList(this.context.Renters, "RenterId", "Name");
        return this.View(rentViewModel);
    }*/

    /*[HttpPost("/rents/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create(RentViewModel rentViewModel)
    {
        if (this.ModelState.IsValid)
        {
            var book = await this.context.Books.FindAsync(rentViewModel.BookId);
            string? userId = this.userManager.GetUserId(this.User);

            if (book == null || book.QuantityAvailable <= 0)
            {
                // Add validation error for BookId
                this.ModelState.AddModelError("BookId", "The selected book is not available for rent.");
            }
            else
            {
                try
                {
                    var rent = new Rent
                    {
                        RentDate = DateTime.ParseExact(rentViewModel.RentDateString, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(rentViewModel.DueDateString, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                        BookId = rentViewModel.BookId,
                        RenterId = rentViewModel.RenterId,
                        AdminId = Guid.Parse(userId!),
                    };

                    await this.rentService.AddRentAsync(rent);
                    return this.RedirectToAction(nameof(this.Rents));
                }
                catch (FormatException)
                {
                    this.ModelState.AddModelError("RentDate", "Invalid rent date format.");
                    this.ModelState.AddModelError("DueDate", "Invalid due date format.");
                }
            }
        }

        // Repopulate the dropdowns if validation fails
        this.ViewBag.Books = new SelectList(this.context.Books, "BookId", "Title");
        this.ViewBag.Renters = new SelectList(this.context.Renters, "RenterId", "Name");
        return this.View(rentViewModel);
    }*/

    [HttpPost("/rents/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create(RentViewModel rentViewModel)
    {
        /*// Parse date strings to DateTime
        if (!string.IsNullOrEmpty(rentViewModel.RentDateString))
        {
            if (DateTime.TryParseExact(rentViewModel.RentDateString, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime rentDate))
            {
                rentViewModel.RentDate = rentDate;
            }
            else
            {
                this.ModelState.AddModelError("RentDate", "Invalid date format.");
            }
        }

        if (!string.IsNullOrEmpty(rentViewModel.DueDateString))
        {
            if (DateTime.TryParseExact(rentViewModel.DueDateString, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dueDate))
            {
                rentViewModel.DueDate = dueDate;
            }
            else
            {
                this.ModelState.AddModelError("DueDate", "Invalid date format.");
            }
        }*/

        if (this.ModelState.IsValid)
        {
            // Rest of your existing code
            var book = await this.context.Books.FindAsync(rentViewModel.BookId);
            string? userId = this.userManager.GetUserId(this.User);

            if (book == null || book.QuantityAvailable <= 0)
            {
                // Add validation error for BookId
                this.ModelState.AddModelError("BookId", "The selected book is not available for rent.");
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

        // Repopulate the dropdowns if validation fails
        this.ViewBag.Books = new SelectList(this.context.Books, "BookId", "Title");
        this.ViewBag.Renters = new SelectList(this.context.Renters, "RenterId", "Name");
        return this.View(rentViewModel);
    }

    /*[HttpGet("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Return(Guid rentId)
    {
        var rent = (await this.rentService.GetAllRentsAsync()).FirstOrDefault(r => r.RentId == rentId);

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
    }*/

    [HttpGet("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Return(Guid rentId)
    {
        var rent = (await this.rentService.GetAllRentsAsync()).FirstOrDefault(r => r.RentId == rentId);

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
            // Set default ReturnDateString to current date/time
            ReturnDateString = rent.RentDate.ToString("dd-MM-yyyy HH:mm"),
        };

        return this.View(rentViewModel);
    }

    /*[HttpPost("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Return(RentViewModel rentViewModel)
    {
        if (rentViewModel.ReturnDate == DateTime.MinValue || rentViewModel.ReturnDate < rentViewModel.RentDate)
        {
            this.ModelState.AddModelError("ReturnDate", "Return date must be after the rent date.");
        }

        /*if (this.ModelState.IsValid)
        {
            try
            {
                var success = await this.rentService.UpdateReturnDateAsync(rentViewModel.RentId, rentViewModel.ReturnDate!.Value);
                if (success)
                {
                    return this.RedirectToAction(nameof(this.Rents));
                }
            }
            catch (InvalidOperationException ex)
            {
                this.ModelState.AddModelError("ReturnDate", ex.Message);
            }
        }#1#

        if (this.ModelState.IsValid)
        {
            var success = await this.rentService.UpdateReturnDateAsync(rentViewModel.RentId, rentViewModel.ReturnDate!.Value);
            if (success)
            {
                return this.RedirectToAction(nameof(this.Rents));
            }
        }

        return this.View(rentViewModel);
    }*/

    /*[HttpPost("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Return(RentViewModel rentViewModel)
    {
        if (rentViewModel.ReturnDate == DateTime.MinValue || rentViewModel.ReturnDate < rentViewModel.RentDate)
        {
            this.ModelState.AddModelError("ReturnDate", "Return date must be after the rent date.");
        }

        if (this.ModelState.IsValid)
        {
            var success = await this.rentService.UpdateReturnDateAsync(rentViewModel.RentId, rentViewModel.ReturnDate!.Value);
            if (success)
            {
                return this.RedirectToAction(nameof(this.Rents));
            }
        }

        return this.View(rentViewModel);
    }*/

    [HttpPost("/rents/return/{rentId}")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Return(RentViewModel rentViewModel)
    {
        // Parse ReturnDateString
        /*if (!string.IsNullOrEmpty(rentViewModel.ReturnDate.ToString()))
        {
            if (DateTime.TryParseExact(rentViewModel.ReturnDateString, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime returnDate))
            {
                rentViewModel.ReturnDate = returnDate;
            }
            else
            {
                this.ModelState.AddModelError("ReturnDate", "Invalid date format.");
            }
        }*/

        if (rentViewModel.ReturnDate == DateTime.MinValue || rentViewModel.ReturnDate < rentViewModel.RentDate)
        {
            this.ModelState.AddModelError("ReturnDate", "Return date must be after the rent date.");
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
}