using BookDepoSystem.Data.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IBookService
{
    public Task AddBook(Book newBook);
    public Task DeleteBook(Book book);

    public Task<Book?> GetBookById(Guid bookId);

    public Task<bool> UpdateBook(Book updatedBook);

    public Task<(List<Book> Books, int TotalCount)> GetBooksPaginated(string genre, string ageRange, string sortBy, string search, int pageIndex, int pageSize);
}