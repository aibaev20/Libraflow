using System;
using BookDepoSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Tests;

public static class TestHelpers
{
    public static EntityContext CreateDbContext()
    {
        var dbContextOptions = new DbContextOptionsBuilder<EntityContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        
        return new EntityContext(dbContextOptions.Options);
    }
}