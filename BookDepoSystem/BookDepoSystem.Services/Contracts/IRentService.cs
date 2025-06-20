﻿using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IRentService
{
    Task<Rent?> GetRentById(Guid rentId);
    Task AddRentAsync(Rent rent);
    Task UpdateAllRentStatusesAsync();
    Task<bool> UpdateReturnDateAsync(Guid rentId, DateTime returnDate);

    Task<(List<Rent> Rents, int TotalCount)> GetRentsPaginated(string status, string search, int pageIndex, int pageSize);

    Task<RentPdfModel> ExportMonthlyRentsPdfAsync();
}