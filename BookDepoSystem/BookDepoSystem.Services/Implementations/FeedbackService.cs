using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    private readonly EntityContext context;

    public FeedbackService(EntityContext context)
    {
        this.context = context;
    }

    public async Task AddFeedback(Feedback newFeedback)
    {
        await this.context.Feedback.AddAsync(newFeedback);
        await this.context.SaveChangesAsync();
    }

    public async Task<bool> AlreadyHasFeedback(Guid? renterId, Guid? bookId)
    {
        return await this.context.Feedback
            .AnyAsync(f => f.RenterId == renterId && f.BookId == bookId);
    }

    public async Task<List<FeedbackDisplayModel>> GetFeedbacksForBook(Guid bookId)
    {
        return await context.Feedback
            .Where(f => f.BookId == bookId)
            .OrderByDescending(f => f.CreatedOn)
            .Select(f => new FeedbackDisplayModel
            {
                RenterName = f.Renter!.Name,
                Rate = f.Rate,
                Message = f.Message,
                CreatedOn = f.CreatedOn,
            })
            .ToListAsync();
    }
}