using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Products.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Data;

public class FastFoodDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    public FastFoodDbContext(DbContextOptions<FastFoodDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FastFoodDbContext).Assembly);
    }
}
