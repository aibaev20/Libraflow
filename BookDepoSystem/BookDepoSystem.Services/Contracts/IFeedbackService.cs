using BookDepoSystem.Data.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IFeedbackService
{
    Task AddFeedback(Feedback newFeedback);
    Task<bool> AlreadyHasFeedback(Guid? renterId, Guid? bookId);
}