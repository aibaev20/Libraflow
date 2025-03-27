using BookDepoSystem.Data.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IRenterService
{
    public Task AddRenter(Renter newRenter);

    public Task<Renter?> GetRenterById(Guid renterId);

    public Task<bool> UpdateRenter(Renter updatedRenter);

    public Task<(List<Renter> Renters, int TotalCount)> GetRentersPaginated(string search, int pageIndex, int pageSize);
}