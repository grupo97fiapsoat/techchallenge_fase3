using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Products.Entities;
using FastFood.Domain.Shared.Entities;
using FastFood.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace FastFood.Infrastructure.Data;

public class FastFoodDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }

    public FastFoodDbContext(DbContextOptions<FastFoodDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.QrCode)
                .HasMaxLength(1000);

            entity.Property(e => e.Status)
                .HasConversion<string>();
                
            // Configurar as propriedades de data para usar o fuso horário do Brasil
            entity.Property(e => e.CreatedAt)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime()
                );
                
            entity.Property(e => e.UpdatedAt)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc).ToLocalTime() : (DateTime?)null
                );
        });
        
        // Configurar as propriedades de data para todas as entidades que herdam de Entity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                var createdAtProperty = entityType.FindProperty(nameof(Entity.CreatedAt));
                if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
                {
                    createdAtProperty.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime()
                        )
                    );
                }
                
                var updatedAtProperty = entityType.FindProperty(nameof(Entity.UpdatedAt));
                if (updatedAtProperty != null && updatedAtProperty.ClrType == typeof(DateTime?))
                {
                    updatedAtProperty.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc).ToLocalTime() : (DateTime?)null
                        )
                    );
                }
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FastFoodDbContext).Assembly);
    }
}
