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

public class BookController : Controller
{
    private readonly IBookService bookService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;

    public BookController(
        IBookService bookService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager)
    {
        this.bookService = bookService;
        this.currentUser = currentUser;
        this.userManager = userManager;
    }

    [HttpGet("/books")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Books(string search = "", int page = 1, int pageSize = 5)
    {
        if (page < 1)
        {
            return this.RedirectToAction(nameof(this.Books), new { page = 1, pageSize });
        }

        var (books, totalCount) = await this.bookService.GetBooksPaginated(search, page, pageSize);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Redirect to the last available page if requested page is out of range

        if (page > totalPages && totalPages > 0)
        {
            return this.RedirectToAction(nameof(this.Books), new { search, page = totalPages, pageSize });
        }

        var viewModel = new PaginationViewModel<Book>
        {
            Items = books,
            PageIndex = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            TotalCount = totalCount,
        };

        this.ViewData["Search"] = search; // Retain search term in the view
        this.ViewData["PageSize"] = pageSize;

        return this.View(viewModel);
    }

    [HttpGet("/books/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost("/books/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            string? userId = this.userManager.GetUserId(this.User);

            var newBook = new Book
            {
                AdminId = Guid.Parse(userId!),
                BookId = Guid.NewGuid(),
                Title = model.Title,
                Author = model.Author,
                Genre = model.Genre,
                Location = model.Location,
                Information = model.Information,
                PublishedDate = model.PublishedDate!.Value,
                QuantityAvailable = model.QuantityAvailable!.Value,
            };

            await this.bookService.AddBook(newBook);
            return this.RedirectToAction(nameof(this.Books));
        }

        return this.View(model);
    }

    [HttpGet("/edit")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Edit(Guid id)
    {
        var book = await this.bookService.GetBookById(id);
        if (book == null)
        {
            return this.NotFound();
        }

        var model = new BookViewModel()
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Location = book.Location,
            Information = book.Information,
            PublishedDate = book.PublishedDate,
            QuantityAvailable = book.QuantityAvailable,
        };

        return this.View(model);
    }

    [HttpPost("/edit")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Edit(BookViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var existingBook = await this.bookService.GetBookById(model.BookId);

        if (existingBook == null)
        {
            return this.NotFound();
        }

        existingBook.Title = model.Title;
        existingBook.Author = model.Author;
        existingBook.Genre = model.Genre;
        existingBook.Location = model.Location;
        existingBook.Information = model.Information;
        existingBook.PublishedDate = model.PublishedDate!.Value;
        existingBook.QuantityAvailable = model.QuantityAvailable!.Value;

        var result = await this.bookService.UpdateBook(existingBook);
        if (!result)
        {
            return this.NotFound();
        }

        return this.RedirectToAction(nameof(this.Books));
    }

    [HttpGet("/delete")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var book = await this.bookService.GetBookById(id);
        if (book == null)
        {
            return this.NotFound();
        }

        var model = new BookViewModel()
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Location = book.Location,
            Information = book.Information,
            PublishedDate = book.PublishedDate,
            QuantityAvailable = book.QuantityAvailable,
        };

        return this.View(model);
    }

    [HttpPost("/delete")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteConfirmed(BookViewModel model)
    {
        var book = await this.bookService.GetBookById(model.BookId);
        if (book == null)
        {
            return this.NotFound();
        }

        await this.bookService.DeleteBook(book);
        return this.RedirectToAction(nameof(this.Books));
    }

    [HttpGet("/details")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Details(Guid id)
    {
        var book = await this.bookService.GetBookById(id);
        if (book == null)
        {
            return this.NotFound();
        }

        var model = new BookViewModel()
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Location = book.Location,
            Information = book.Information,
            PublishedDate = book.PublishedDate,
            QuantityAvailable = book.QuantityAvailable,
        };

        return this.View(model);
    }
}