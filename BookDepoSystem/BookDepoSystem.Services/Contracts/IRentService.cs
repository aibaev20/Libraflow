using System.Collections.Generic;
using BookDepoSystem.Data.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IRentService
{
    Task<IEnumerable<Rent>> GetAllRentsAsync();
    Task AddRentAsync(Rent rent);
    Task UpdateAllRentStatusesAsync();
    Task<bool> UpdateReturnDateAsync(Guid rentId, DateTime returnDate);
}