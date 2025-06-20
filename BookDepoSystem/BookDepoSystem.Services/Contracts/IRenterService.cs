﻿using BookDepoSystem.Data.Models;

namespace BookDepoSystem.Services.Contracts;

public interface IRenterService
{
    public Task<(List<Renter> Renters, int TotalCount)> GetRentersPaginated(string search, int pageIndex, int pageSize);
}