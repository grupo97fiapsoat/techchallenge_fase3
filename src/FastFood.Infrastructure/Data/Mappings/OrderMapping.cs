using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFood.Infrastructure.Data.Mappings;

/// <summary>
/// Configuração do mapeamento de Order para o EF Core.
/// </summary>
public class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.PreferenceId)
            .HasMaxLength(100)
            .IsRequired(false);

        // Relacionamento com Customer
        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);        // Configuração de OrderItem como owned type em uma coleção
        builder.OwnsMany(o => o.Items, ownedBuilder =>
        {
            ownedBuilder.ToTable("OrderItems");
            
            // Configurar o Id como GUID usando uma sombra property
            ownedBuilder.Property<Guid>("Id")
                .ValueGeneratedOnAdd();
            ownedBuilder.HasKey("Id");

            // Propriedades do OrderItem
            ownedBuilder.Property(i => i.ProductId)
                .IsRequired();

            ownedBuilder.Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            ownedBuilder.Property(i => i.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            ownedBuilder.Property(i => i.Quantity)
                .IsRequired();

            // Configurar o pedido pai
            ownedBuilder.WithOwner()
                .HasForeignKey("OrderId");
        });
    }
}
