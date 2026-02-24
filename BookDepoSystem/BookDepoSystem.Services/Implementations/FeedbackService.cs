using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
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
}