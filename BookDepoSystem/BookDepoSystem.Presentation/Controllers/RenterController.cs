using System.Diagnostics;
using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Presentation.Models;
using BookDepoSystem.Services.Common.Contracts;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Identity.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookDepoSystem.Presentation.Controllers;

public class RenterController : Controller
{
    private readonly IRenterService renterService;

    public RenterController(
        IRenterService renterService)
    {
        this.renterService = renterService;
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

        this.ViewData["Search"] = search;
        this.ViewData["PageSize"] = pageSize;

        return this.View(viewModel);
    }
}