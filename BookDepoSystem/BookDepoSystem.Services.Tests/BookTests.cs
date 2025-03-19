using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookDepoSystem.Services.Tests;

public class BookTests
{
    [Fact]
    public async Task GetByIdAsync_OnExistingBook_ShouldReturnBookModel()
    {
        var dbContext = TestHelpers.CreateDbContext();

        var newBook = new Book()
        {
            BookId = Guid.NewGuid(),
            Title = "Test Title",
            Author = "Test Author",
            Genre = "Test Genre",
            Location = "Test Location",
            Information = "Test Information",
            PublishedDate = new DateTime(2025, 3, 19),
            QuantityAvailable = 10,
        };

        dbContext.Books.Add(newBook);
        await dbContext.SaveChangesAsync();

        var service = new BookService(dbContext);
        
        var retrievedBook = await service.GetBookById(newBook.BookId);
        
        retrievedBook.Should().NotBeNull();
        retrievedBook!.BookId.Should().Be(newBook.BookId);
        retrievedBook.Title.Should().Be("Test Title");
        retrievedBook.Information.Should().Be("Test Information");

        await dbContext.DisposeAsync();
        
        /*var booksPaginated = await service.GetBooksPaginated("", 1, 5);
        booksPaginated.Should().NotBeNull();
        booksPaginated.Books.Should().NotBeEmpty();*/
        
        /*var firstBook = booksPaginated.Books.FirstOrDefault();
        firstBook.Should().NotBeNull();
        retrievedBook!.BookId.Should().Be(bookId);
        /*firstBook.BookId.Should().Be(BookId); // Use the bookId variable instead of Guid.NewGuid()#1#
        firstBook.Title.Should().Be("Test Title");
        firstBook.Information.Should().Be("Test Information");*/
        
        /*booksPaginated.Id.Should().Be(Guid.NewGuid());
        booksPaginated.Title.Should().Be("Test Title");
        booksPaginated.Information.Should().Be("Test Information");*/
    }
}