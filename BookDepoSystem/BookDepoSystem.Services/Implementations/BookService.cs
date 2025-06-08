using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Services.Implementations;

public class BookService : IBookService
{
    private readonly EntityContext context;

    public BookService(EntityContext context)
    {
        this.context = context;
    }

    public async Task AddBook(Book newBook)
    {
        await this.context.Books.AddAsync(newBook);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteBook(Book book)
    {
        this.context.Books.Remove(book);
        await this.context.SaveChangesAsync();
    }

    public async Task<Book?> GetBookById(Guid bookId)
    {
        return await this.context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
    }

    public async Task<bool> UpdateBook(Book updatedBook)
    {
        Book? oldBook = await this.GetBookById(updatedBook.BookId);

        if (oldBook == null)
        {
            return false;
        }

        this.context.Entry(oldBook).CurrentValues.SetValues(updatedBook);
        await this.context.SaveChangesAsync();

        return true;
    }

    public async Task<(List<Book> Books, int TotalCount)> GetBooksPaginated(string genre, string ageRange, string sortBy, string search, int pageIndex, int pageSize)
    {
        var query = this.context.Books.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Title!.Contains(search) ||
                                     b.Author!.Contains(search) ||
                                     b.Genre!.Contains(search) ||
                                     b.Location!.Contains(search));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }

        if (!string.IsNullOrEmpty(ageRange))
        {
            query = query.Where(b => b.AgeRange == ageRange);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy)
            {
                case "Количество (низходящ ред)":
                    query = query.OrderByDescending(b => b.QuantityAvailable).ThenBy(b => b.Title);
                    break;
                case "Количество (възходящ ред)":
                    query = query.OrderBy(b => b.QuantityAvailable).ThenBy(b => b.Title);
                    break;
                case "Име (низходящ ред)":
                    query = query.OrderByDescending(b => b.Title);
                    break;
                case "Име (възходящ ред)":
                    query = query.OrderBy(b => b.Title);
                    break;
                default:
                    query = query.OrderBy(b => b.Title);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(b => b.Title);
        }

        int totalCount = await query.CountAsync();
        var books = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (books, totalCount);
    }
}