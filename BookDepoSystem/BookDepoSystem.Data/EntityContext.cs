using System;
using BookDepoSystem.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookDepoSystem.Data;

public class EntityContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Renter> Renters { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Rent> Rents { get; set; }
    public EntityContext(DbContextOptions<EntityContext> options)
        : base(options)
    {
    }
}