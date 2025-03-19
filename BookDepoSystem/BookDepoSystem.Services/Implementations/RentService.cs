using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Services.Implementations;

public class RentService : IRentService
{
    private readonly EntityContext context;

    public RentService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Rent>> GetAllRentsAsync()
    {
        var rents = await this.context.Rents
            .Include(r => r.Book)
            .Include(r => r.Renter)
            .OrderByDescending(r => r.RentDate)
            .ToListAsync();

        //rents = rents.OrderByDescending(r => r.RentDate).ToList();
        var popularBooks = rents
            .GroupBy(r => r.BookId)
            .OrderByDescending(g => g.Count())
            .SelectMany(g => g)
            .ToList();

        return popularBooks;
    }

    public async Task AddRentAsync(Rent rent)
    {
        var book = await this.context.Books.FindAsync(rent.BookId);

        rent.RentId = Guid.NewGuid();
        rent.ReturnDate = DateTime.MinValue;
        rent.Status = "Confirmed";
        rent.CreatedAt = DateTime.UtcNow;
        rent.UpdatedAt = DateTime.UtcNow;

        book!.QuantityAvailable -= 1;
        this.context.Books.Update(book);
        this.context.Rents.Add(rent);
        await this.context.SaveChangesAsync();
    }

    public async Task UpdateAllRentStatusesAsync()
    {
        var rents = await this.context.Rents.ToListAsync();

        foreach (var rent in rents)
        {
            if (DateTime.UtcNow >= rent.DueDate)
            {
                rent.Status = "Overdue";
            }
            else
            {
                rent.Status = "Active";
            }

            rent.UpdatedAt = DateTime.UtcNow;
            this.context.Rents.Update(rent);
        }

        await this.context.SaveChangesAsync();
    }

    public async Task<bool> UpdateReturnDateAsync(Guid rentId, DateTime returnDate)
    {
        var rent = await this.context.Rents.Include(r => r.Book).FirstOrDefaultAsync(r => r.RentId == rentId);

        if (rent == null)
        {
            return false;
        }

        /*if (returnDate < rent.RentDate)
        {
            throw new InvalidOperationException("Return date cannot be before the rent date.");
        }*/

        rent.ReturnDate = returnDate;
        rent.Status = "Completed";
        rent.UpdatedAt = DateTime.UtcNow;

        // Restore book quantity
        if (rent.Book != null)
        {
            rent.Book.QuantityAvailable += 1;
        }

        this.context.Rents.Update(rent);

        await this.context.SaveChangesAsync();

        return true;
    }
}