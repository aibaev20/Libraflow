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

    public async Task<Renter?> GetRenterById(Guid renterId)
    {
        return await this.context.Renters.FirstOrDefaultAsync(b => b.RenterId == renterId);
    }

    public async Task<bool> UpdateRenter(Renter updatedRenter)
    {
        Renter? oldRenter = await this.GetRenterById(updatedRenter.RenterId);

        if (oldRenter == null)
        {
            return false;
        }

        this.context.Entry(oldRenter).CurrentValues.SetValues(updatedRenter);
        await this.context.SaveChangesAsync();

        return true;
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