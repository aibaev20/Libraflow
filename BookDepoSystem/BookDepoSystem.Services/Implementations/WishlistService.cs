using BookDepoSystem.Common;
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

    public async Task AddToWishlist(Guid renterId, Guid bookId)
    {
        await EnsureBookExists(bookId);

        if (await IsAlreadyInWishlist(renterId, bookId))
        {
            return;
        }

        await CreateWishlistItem(renterId, bookId);
    }

    public async Task RemoveFromWishlist(Guid renterId, Guid bookId)
    {
        var wishlistItem = await GetWishlistItem(renterId, bookId);

        if (wishlistItem == null)
        {
            return;
        }

        await DeleteWishlistItem(wishlistItem);
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

    private async Task EnsureBookExists(Guid bookId)
    {
        var exists = await this.context.Books
            .AnyAsync(b => b.BookId == bookId);

        if (!exists)
        {
            throw new ArgumentException(@T.BookNotExist);
        }
    }

    private async Task<bool> IsAlreadyInWishlist(Guid renterId, Guid bookId)
    {
        return await this.context.Wishlist
            .AnyAsync(w => w.RenterId == renterId && w.BookId == bookId);
    }

    private async Task CreateWishlistItem(Guid renterId, Guid bookId)
    {
        var wishlistItem = new Wishlist
        {
            WishlistId = Guid.NewGuid(),
            RenterId = renterId,
            BookId = bookId,
            AddedOn = DateTime.UtcNow,
        };

        await this.context.Wishlist.AddAsync(wishlistItem);
        await this.context.SaveChangesAsync();
    }

    private async Task<Wishlist?> GetWishlistItem(Guid renterId, Guid bookId)
    {
        return await this.context.Wishlist
            .FirstOrDefaultAsync(w => w.RenterId == renterId && w.BookId == bookId);
    }

    private async Task DeleteWishlistItem(Wishlist wishlistItem)
    {
        this.context.Wishlist.Remove(wishlistItem);
        await this.context.SaveChangesAsync();
    }
}