using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IFeedbackService
{
    Task AddFeedback(Feedback newFeedback);
    Task<bool> AlreadyHasFeedback(Guid? renterId, Guid? bookId);
    Task<List<FeedbackDisplayModel>> GetFeedbacksForBook(Guid bookId);
}