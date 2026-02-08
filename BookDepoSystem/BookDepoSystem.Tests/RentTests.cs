using System.Runtime.Serialization;
using BookDepoSystem.Common;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Implementations;
using FluentAssertions;

namespace BookDepoSystem.Tests;

public class RentTests
{
    [Fact]
    public async Task GetRentByIdAsync_OnExistingRent_ShouldReturnRentModel()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var renterService = new RenterService(dbContext);
        var bookService = new BookService(dbContext);
        var rentService = new RentService(dbContext);
        
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
            AdminId = Guid.NewGuid(),
        };

        await bookService.AddBook(newBook);
        await dbContext.SaveChangesAsync();

        var newRenter = new Renter()
        {
            RenterId = Guid.NewGuid(),
            Name = @T.RentTestNewRenterName,
            Email = "ivan.ivanov@example.com",
            PhoneNumber = "0888888888",
        };
            
        await renterService.AddRenter(newRenter);
        await dbContext.SaveChangesAsync();

        var newRent = new Rent()
        {
            RentId = Guid.NewGuid(),
            RentDate = new DateTime(2025, 3, 30),
            DueDate = new DateTime(2025, 3, 31),
            ReturnDate = DateTime.MinValue,
            Status = @T.ConfirmedRent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookId = newBook.BookId,
            RenterId = newRenter.RenterId,
            AdminId = Guid.NewGuid(),
        };

        await rentService.AddRentAsync(newRent);
        await dbContext.SaveChangesAsync();
        
        var retrievedRent = await rentService.GetRentById(newRent.RentId);
        
        retrievedRent.Should().NotBeNull();
        retrievedRent.BookId.Should().Be(newRent.BookId);
        retrievedRent.Status.Should().Be(@T.ConfirmedRent);
        retrievedRent.RentDate.Should().Be(new DateTime(2025, 3, 30));
        retrievedRent.DueDate.Should().Be(new DateTime(2025, 3, 31));

        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task CreateRent_OnValidInput_ShouldCreateRent()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var renterService = new RenterService(dbContext);
        var bookService = new BookService(dbContext);
        var rentService = new RentService(dbContext);
        
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
            AdminId = Guid.NewGuid(),
        };

        await bookService.AddBook(newBook);
        await dbContext.SaveChangesAsync();

        var newRenter = new Renter()
        {
            RenterId = Guid.NewGuid(),
            Name = @T.RentTestNewRenterName,
            Email = "ivan.ivanov@example.com",
            PhoneNumber = "0888888888",
        };
            
        await renterService.AddRenter(newRenter);
        await dbContext.SaveChangesAsync();

        var newRent = new Rent()
        {
            RentId = Guid.NewGuid(),
            RentDate = new DateTime(2025, 3, 30),
            DueDate = new DateTime(2025, 3, 31),
            ReturnDate = DateTime.MinValue,
            Status = @T.ConfirmedRent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookId = newBook.BookId,
            RenterId = newRenter.RenterId,
            AdminId = Guid.NewGuid(),
        };

        await rentService.AddRentAsync(newRent);
        await dbContext.SaveChangesAsync();
        
        var createdRent = await rentService.GetRentById(newRent.RentId);
        var createdBook = await bookService.GetBookById(newBook.BookId);
        
        createdRent.Should().NotBeNull();
        createdRent.BookId.Should().Be(newRent.BookId);
        createdRent.Status.Should().Be(@T.ConfirmedRent);
        createdRent.RentDate.Should().Be(new DateTime(2025, 3, 30));
        createdRent.DueDate.Should().Be(new DateTime(2025, 3, 31));
        createdRent.RenterId.Should().Be(newRenter.RenterId);
        createdBook!.QuantityAvailable.Should().Be(9);

        await dbContext.DisposeAsync();
    }
    
    [Fact]
    public async Task ReturnRent_OnValidInput_ShouldReturnRent()
    {
        var dbContext = TestHelpers.CreateDbContext();
        var renterService = new RenterService(dbContext);
        var bookService = new BookService(dbContext);
        var rentService = new RentService(dbContext);
        
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
            AdminId = Guid.NewGuid(),
        };

        await bookService.AddBook(newBook);
        await dbContext.SaveChangesAsync();

        var newRenter = new Renter()
        {
            RenterId = Guid.NewGuid(),
            Name = @T.RentTestNewRenterName,
            Email = "ivan.ivanov@example.com",
            PhoneNumber = "0888888888",
        };
            
        await renterService.AddRenter(newRenter);
        await dbContext.SaveChangesAsync();

        var newRent = new Rent()
        {
            RentId = Guid.NewGuid(),
            RentDate = new DateTime(2025, 3, 30),
            DueDate = new DateTime(2025, 3, 31),
            ReturnDate = DateTime.MinValue,
            Status = @T.ConfirmedRent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookId = newBook.BookId,
            RenterId = newRenter.RenterId,
            AdminId = Guid.NewGuid(),
        };

        await rentService.AddRentAsync(newRent);
        await dbContext.SaveChangesAsync();
        
        newRent.ReturnDate = new DateTime(2025, 4, 1);

        await rentService.UpdateReturnDateAsync(newRent.RentId, newRent.ReturnDate, newRent.Penalty);
        await dbContext.SaveChangesAsync();
        
        var returnedRent = await rentService.GetRentById(newRent.RentId);
        var returnedBook = await bookService.GetBookById(newBook.BookId);
        
        returnedRent.Should().NotBeNull();
        returnedRent.BookId.Should().Be(newRent.BookId);
        returnedRent.Status.Should().Be(@T.CompletedRent);
        returnedRent.RentDate.Should().Be(new DateTime(2025, 3, 30));
        returnedRent.DueDate.Should().Be(new DateTime(2025, 3, 31));
        returnedRent.ReturnDate.Should().Be(new DateTime(2025, 4, 1));
        returnedRent.RenterId.Should().Be(newRenter.RenterId);
        returnedBook!.QuantityAvailable.Should().Be(10);
        
        await dbContext.DisposeAsync();
    }
}