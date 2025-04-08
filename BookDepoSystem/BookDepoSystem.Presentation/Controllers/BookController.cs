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

public class BookController : Controller
{
    private readonly IBookService bookService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IWebHostEnvironment webHostEnvironment;

    public BookController(
        IBookService bookService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment webHostEnvironment)
    {
        this.bookService = bookService;
        this.userManager = userManager;
        this.webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("/books")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Books(string search = "", int page = 1, int pageSize = 5)
    {
        if (page < 1)
        {
            return this.RedirectToAction(nameof(this.Books), new { page = 1, pageSize });
        }

        var (books, totalCount) = await this.bookService.GetBooksPaginated(search, page, pageSize);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

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

        this.ViewData["Search"] = search;
        this.ViewData["PageSize"] = pageSize;

        return this.View(viewModel);
    }

    [HttpGet("/books/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost("/books/create")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Create(BookViewModel model, IFormFile? coverImageFile)
    {
        if (this.ModelState.IsValid)
        {
            string? fileName = null;
            if (coverImageFile != null)
            {
                fileName = Guid.NewGuid() + Path.GetExtension(coverImageFile.FileName);
                string filePath = Path.Combine(this.webHostEnvironment.WebRootPath, "img", "books", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImageFile.CopyToAsync(stream);
                }
            }

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
                CoverImage = fileName,
                Isbn = model.Isbn,
                Sku = model.Sku,
                Pages = model.Pages,
                AgeRange = model.AgeRange,
                CoverType = model.CoverType,
            };

            await this.bookService.AddBook(newBook);
            return this.RedirectToAction(nameof(this.Books));
        }

        return this.View(model);
    }

    [HttpGet("/books/edit")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
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
            CoverImage = book.CoverImage,
            Isbn = book.Isbn,
            Sku = book.Sku,
            Pages = book.Pages,
            AgeRange = book.AgeRange,
            CoverType = book.CoverType,
        };

        return this.View(model);
    }

    [HttpPost("/books/edit")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> Edit(BookViewModel model, IFormFile? coverImageFile)
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

        if (coverImageFile != null)
        {
            if (existingBook.CoverImage != null)
            {
                string oldImagePath = Path.Combine(this.webHostEnvironment.WebRootPath, "img", "books", existingBook.CoverImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(coverImageFile.FileName);
            var filePath = Path.Combine(this.webHostEnvironment.WebRootPath, "img", "books", fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await coverImageFile.CopyToAsync(stream);
            }

            existingBook.CoverImage = fileName;
        }

        existingBook.Title = model.Title;
        existingBook.Author = model.Author;
        existingBook.Genre = model.Genre;
        existingBook.Location = model.Location;
        existingBook.Information = model.Information;
        existingBook.PublishedDate = model.PublishedDate!.Value;
        existingBook.QuantityAvailable = model.QuantityAvailable!.Value;
        existingBook.Isbn = model.Isbn;
        existingBook.Sku = model.Sku;
        existingBook.Pages = model.Pages;
        existingBook.AgeRange = model.AgeRange;
        existingBook.CoverType = model.CoverType;

        var result = await this.bookService.UpdateBook(existingBook);
        if (!result)
        {
            return this.NotFound();
        }

        return this.RedirectToAction(nameof(this.Books));
    }

    [HttpGet("/delete")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
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
            CoverImage = book.CoverImage,
            Isbn = book.Isbn,
            Sku = book.Sku,
            Pages = book.Pages,
            AgeRange = book.AgeRange,
            CoverType = book.CoverType,
        };

        return this.View(model);
    }

    [HttpPost("/delete")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
    public async Task<IActionResult> DeleteConfirmed(BookViewModel model)
    {
        var book = await this.bookService.GetBookById(model.BookId);
        if (book == null)
        {
            return this.NotFound();
        }

        if (book.CoverImage != null)
        {
            string filePath = Path.Combine(this.webHostEnvironment.WebRootPath, "img", "books", book.CoverImage);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        await this.bookService.DeleteBook(book);
        return this.RedirectToAction(nameof(this.Books));
    }

    [HttpGet("/details")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(DefaultPolicies.AdminPolicy)]
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
            CoverImage = book.CoverImage,
            Isbn = book.Isbn,
            Sku = book.Sku,
            Pages = book.Pages,
            AgeRange = book.AgeRange,
            CoverType = book.CoverType,
        };

        return this.View(model);
    }
}