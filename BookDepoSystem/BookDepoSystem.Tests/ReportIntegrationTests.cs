using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookDepoSystem.Tests;

public class ReportIntegrationTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;
    private Guid adminId;
    
    public ReportIntegrationTests(CustomWebApplicationFactory factory)
    {
        this.factory = factory;
    }
    
    [Fact]
    public async Task GetMonthlyRents_WithMultipleRentsInMayAndJune_ShouldGroupCorrectly()
    {
        // Arrange - Set up the test data
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            var adminUser = await userManager.FindByEmailAsync("admin@bookdeposystem.dev");
            adminUser.Should().NotBeNull("Admin user should exist in the database");
            adminId = adminUser!.Id;
            
            dbContext.Rents.RemoveRange(dbContext.Rents);
            dbContext.Renters.RemoveRange(dbContext.Renters);
            dbContext.Books.RemoveRange(dbContext.Books);
            await dbContext.SaveChangesAsync();
            
            var firstBook = new Book()
            {
                BookId = Guid.NewGuid(),
                Title = "Изкуството да бъдеш спокоен",
                Author = "Райън Холидей",
                Genre = "Философия",
                Information = "Спокойствието е ключ към по-щастлив и смислен живот ",
                Location = "F01-A1",
                PublishedDate = new DateTime(2020, 1, 15),
                QuantityAvailable = 10,
                CoverType = "Твърда",
                Isbn = "9789542832485",
                Sku = "111111111111",
                Pages = 276,
                AgeRange = "Възрастни",
                AdminId = adminId,
            };
            
            var secondBook = new Book()
            {
                BookId = Guid.NewGuid(),
                Title = "Истории от ръчния багаж",
                Author = "Георги Милков",
                Genre = "Самоусъвършенстване",
                Information = "Само човек, който стои на педя от лицето на Кадафи",
                Location = "F01-A2",
                PublishedDate = new DateTime(2023, 5, 20),
                QuantityAvailable = 15,
                CoverType = "Мека",
                Isbn = "9786191953493",
                Sku = "222222222222",
                Pages = 520,
                AgeRange = "Възрастни",
                AdminId = adminId,
            };
            
            dbContext.Books.Add(firstBook);
            dbContext.Books.Add(secondBook);
            
            var firstRenter = new Renter()
            {
                AdminId = adminId,
                RenterId = Guid.NewGuid(),
                Name = "Милен Нанков",
                Email = "milen.nankov@example.com",
                PhoneNumber = "0878787878",
            };
            
            var secondRenter = new Renter()
            {
                AdminId = adminId,
                RenterId = Guid.NewGuid(),
                Name = "Самуил Иванов",
                Email = "samuil.ivanov@example.com",
                PhoneNumber = "0898989898",
            };
            
            dbContext.Renters.Add(firstRenter);
            dbContext.Renters.Add(secondRenter);
            
            var firstMayRent = new Rent()
            {
                RentId = Guid.NewGuid(),
                RentDate = new DateTime(2025, 5, 5),
                DueDate = new DateTime(2025, 5, 20),
                ReturnDate = DateTime.MinValue,
                Status = "Потвърден",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BookId = firstBook.BookId,
                RenterId = firstRenter.RenterId,
                AdminId = adminId,
            };
            
            var secondMayRent = new Rent()
            {
                RentId = Guid.NewGuid(),
                RentDate = new DateTime(2025, 5, 15),
                DueDate = new DateTime(2025, 5, 30),
                ReturnDate = DateTime.MinValue,
                Status = "Потвърден",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BookId = secondBook.BookId,
                RenterId = secondRenter.RenterId,
                AdminId = adminId,
            };
            
            var juneRent = new Rent()
            {
                RentId = Guid.NewGuid(),
                RentDate = new DateTime(2025, 6, 10),
                DueDate = new DateTime(2025, 6, 25),
                ReturnDate = DateTime.MinValue,
                Status = "Потвърден",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BookId = firstBook.BookId,
                RenterId = secondRenter.RenterId,
                AdminId = adminId,
            };
            
            dbContext.Rents.Add(firstMayRent);
            dbContext.Rents.Add(secondMayRent);
            dbContext.Rents.Add(juneRent);
            
            await dbContext.SaveChangesAsync();
        }
        
        using (var scope = factory.Services.CreateScope())
        {   
            var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
        
            var monthlyRents = dbContext.Rents
                .GroupBy(r => new { r.RentDate.Year, r.RentDate.Month })
                .Select(g => new 
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    RentCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
        
            monthlyRents.Should().HaveCount(2);
        
            var mayRents = monthlyRents.FirstOrDefault(g => g.Year == 2025 && g.Month == 5);
            mayRents.Should().NotBeNull();
            mayRents.RentCount.Should().Be(2); // Two rents in May
        
            var juneRents = monthlyRents.FirstOrDefault(g => g.Year == 2025 && g.Month == 6);
            juneRents.Should().NotBeNull();
            juneRents.RentCount.Should().Be(1);
            
            await dbContext.DisposeAsync();
        }
    }
}