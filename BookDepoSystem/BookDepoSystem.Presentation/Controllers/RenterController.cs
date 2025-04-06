using System.Diagnostics;
using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Identity.Constants;
using BookDepoSystem.Services.Identity.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookDepoSystem.Presentation.Controllers;

public class RenterController : Controller
{
    private readonly IRenterService renterService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;

    public RenterController(
        IRenterService renterService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager)
    {
        this.renterService = renterService;
        this.currentUser = currentUser;
        this.userManager = userManager;
    }

    [HttpGet("/renters")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Renters(string search = "", int page = 1, int pageSize = 5)
    {
        if (page < 1)
        {
            return this.RedirectToAction(nameof(this.Renters), new { page = 1, pageSize });
        }

        var (renters, totalCount) = await this.renterService.GetRentersPaginated(search, page, pageSize);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Redirect to the last available page if requested page is out of range

        if (page > totalPages && totalPages > 0)
        {
            return this.RedirectToAction(nameof(this.Renters), new { search, page = totalPages, pageSize });
        }

        var viewModel = new PaginationViewModel<Renter>
        {
            Items = renters,
            PageIndex = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            TotalCount = totalCount,
        };

        this.ViewData["Search"] = search; // Retain search term in the view
        this.ViewData["PageSize"] = pageSize;

        return this.View(viewModel);
    }
}