using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Services.Implementations;

public class WishlistService : IWishlistService
{
    private readonly EntityContext context;

    public WishlistService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<bool> AddToWishlist(Guid renterId, Guid bookId)
    {
        Console.WriteLine($"DEBUG: AddToWishlist called with renterId={renterId}, bookId={bookId}");

        // Check if book exists
        var bookExists = await this.context.Books.AnyAsync(b => b.BookId == bookId);
        Console.WriteLine($"DEBUG: Book exists = {bookExists}");

        if (!bookExists)
        {
            return false;
        }

        // Check if already in wishlist
        var alreadyExists = await this.context.Wishlist
            .AnyAsync(w => w.RenterId == renterId && w.BookId == bookId);

        Console.WriteLine($"DEBUG: Already in wishlist = {alreadyExists}");
        if (alreadyExists)
        {
            return true;
        }

        var wishlistItem = new Wishlist
        {
            WishlistId = Guid.NewGuid(),
            RenterId = renterId,
            BookId = bookId,
            AddedOn = DateTime.UtcNow,
        };

        await this.context.Wishlist.AddAsync(wishlistItem);
        var saveResult = await this.context.SaveChangesAsync();
        Console.WriteLine($"DEBUG: Save changes result = {saveResult}");
        return true;
    }

    public async Task<bool> RemoveFromWishlist(Guid renterId, Guid bookId)
    {
        var wishlistItem = await this.context.Wishlist
            .FirstOrDefaultAsync(w => w.RenterId == renterId && w.BookId == bookId);

        if (wishlistItem == null)
        {
            return false;
        }

        this.context.Wishlist.Remove(wishlistItem);
        await this.context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsBookInWishlist(Guid renterId, Guid bookId)
    {
        return await this.context.Wishlist
            .AnyAsync(w => w.RenterId == renterId && w.BookId == bookId);
    }

    public async Task<List<Book>> GetUserWishlist(Guid renterId)
    {
        return await this.context.Wishlist
            .Where(w => w.RenterId == renterId)
            .Include(w => w.Book!)
            .OrderByDescending(w => w.AddedOn)
            .Select(w => w.Book!)
            .ToListAsync();
    }

    public async Task<int> GetWishlistCount(Guid renterId)
    {
        return await this.context.Wishlist
            .Where(w => w.RenterId == renterId)
            .CountAsync();
    }
}