using System;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Entities.User> Users { get; set; } = null!;
    public DbSet<Entities.Post> Posts { get; set; } = null!;
}
