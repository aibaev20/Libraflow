using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Services.Implementations;

public class RenterService : IRenterService
{
    private readonly EntityContext context;

    public RenterService(EntityContext context)
    {
        this.context = context;
    }

    public async Task AddRenter(Renter newRenter)
    {
        await this.context.Renters.AddAsync(newRenter);
        await this.context.SaveChangesAsync();
    }

    public async Task<(List<Renter> Renters, int TotalCount)> GetRentersPaginated(string search, int pageIndex, int pageSize)
    {
        var query = this.context.Renters.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Name!.Contains(search) ||
                                     b.Email!.Contains(search) ||
                                     b.PhoneNumber!.Contains(search));
        }

        int totalCount = await query.CountAsync();
        var renters = await query
            .OrderBy(b => b.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (renters, totalCount);
    }
}