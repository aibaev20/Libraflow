using BookDepoSystem.Common;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookDepoSystem.Tests;

public class BookTests
{
    [Fact]
    public async Task GetBookByIdAsync_OnExistingBook_ShouldReturnBookModel()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var service = new BookService(dbContext);

        var newBook = new Book()
        {
            BookId = Guid.NewGuid(),
            Title = @T.BookTestTitle,
            Author = @T.BookTestAuthor,
            Genre = @T.BiographyGenreText,
            Information = @T.BooKTestInformation,
            Location = "E01-A1",
            PublishedDate = new DateTime(2025, 3, 19),
            QuantityAvailable = 10,
            CoverType = @T.SoftCoverTypeText,
            Isbn = "1111111111111",
            Sku = "111111111111",
            Pages = 200,
            AgeRange = @T.AdultAgeRangeText,
        };

        await service.AddBook(newBook);
        await dbContext.SaveChangesAsync();
        
        var retrievedBook = await service.GetBookById(newBook.BookId);
        
        retrievedBook.Should().NotBeNull();
        retrievedBook.BookId.Should().Be(newBook.BookId);
        retrievedBook.Title.Should().Be(@T.BookTestTitle);
        retrievedBook.Genre.Should().Be(@T.BiographyGenreText);

        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task CreateBook_OnValidInput_ShouldCreateBook()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var service = new BookService(dbContext);

        var newBook = new Book()
        {
            BookId = Guid.NewGuid(),
            Title = @T.BookTestTitle,
            Author = @T.BookTestAuthor,
            Genre = @T.BiographyGenreText,
            Information = @T.BooKTestInformation,
            Location = "E01-A1",
            PublishedDate = new DateTime(2025, 3, 19),
            QuantityAvailable = 10,
            CoverType = @T.SoftCoverTypeText,
            Isbn = "1111111111111",
            Sku = "111111111111",
            Pages = 200,
            AgeRange = @T.AdultAgeRangeText,
        };

        await service.AddBook(newBook);
        await dbContext.SaveChangesAsync();
        
        var createdBook = await service.GetBookById(newBook.BookId);
        createdBook.Should().NotBeNull();
        createdBook.BookId.Should().Be(newBook.BookId);
        createdBook.Title.Should().Be(newBook.Title);
        createdBook.Author.Should().Be(newBook.Author);
        createdBook.Genre.Should().Be(newBook.Genre);
        createdBook.QuantityAvailable.Should().Be(10);

        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task EditBook_OnValidInput_ShouldEditBook()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var service = new BookService(dbContext);

        var existingBook = new Book()
        {
            BookId = Guid.NewGuid(),
            Title = @T.BookTestTitle,
            Author = @T.BookTestAuthor,
            Genre = @T.BiographyGenreText,
            Information = @T.BooKTestInformation,
            Location = "E01-A1",
            PublishedDate = new DateTime(2025, 3, 19),
            QuantityAvailable = 10,
            CoverType = @T.SoftCoverTypeText,
            Isbn = "1111111111111",
            Sku = "111111111111",
            Pages = 200,
            AgeRange = @T.AdultAgeRangeText,
        };

        await service.AddBook(existingBook);
        await dbContext.SaveChangesAsync();

        existingBook.Location = "E02-A2";
        existingBook.CoverType = @T.HardCoverTypeText;
        existingBook.Pages = 300;
        
        await service.UpdateBook(existingBook);
        await dbContext.SaveChangesAsync();
        
        var updatedBook = await service.GetBookById(existingBook.BookId);

        updatedBook.Should().NotBeNull();
        updatedBook.Location.Should().Be("E02-A2");
        updatedBook.CoverType.Should().Be(@T.HardCoverTypeText);
        updatedBook.Pages.Should().Be(300);

        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task DeleteBook_OnExistingBook_ShouldDeleteBook()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var service = new BookService(dbContext);

        var existingBook = new Book()
        {
            BookId = Guid.NewGuid(),
            Title = @T.BookTestTitle,
            Author = @T.BookTestAuthor,
            Genre = @T.BiographyGenreText,
            Information = @T.BooKTestInformation,
            Location = "E01-A1",
            PublishedDate = new DateTime(2025, 3, 19),
            QuantityAvailable = 10,
            CoverType = @T.SoftCoverTypeText,
            Isbn = "1111111111111",
            Sku = "111111111111",
            Pages = 200,
            AgeRange = @T.AdultAgeRangeText,
        };

        await service.AddBook(existingBook);
        await dbContext.SaveChangesAsync();
        
        await service.DeleteBook(existingBook);
        await dbContext.SaveChangesAsync();
        
        var deletedBook = await service.GetBookById(existingBook.BookId);
        deletedBook.Should().BeNull();

        await dbContext.DisposeAsync();
    }
}