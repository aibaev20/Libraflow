using BookDepoSystem.Data.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IWishlistService
{
    Task<bool> AddToWishlist(Guid renterId, Guid bookId);
    Task<bool> RemoveFromWishlist(Guid renterId, Guid bookId);
    Task<bool> IsBookInWishlist(Guid renterId, Guid bookId);
    Task<List<Book>> GetUserWishlist(Guid renterId);
    Task<int> GetWishlistCount(Guid renterId);
}