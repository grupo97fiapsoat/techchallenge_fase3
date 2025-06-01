using FastFood.Domain.Products.Entities;
using FastFood.Domain.Products.Enums;
using FastFood.Domain.Products.Repositories;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace FastFood.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de produtos usando Entity Framework Core.
/// </summary>
public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(FastFoodDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryName, int pageNumber = 1, int pageSize = 10)
    {
        if (Enum.TryParse<ProductCategory>(categoryName, true, out var category))
        {
            return await GetByCategoryAsync(category, pageNumber, pageSize);
        }
        
        // Se não for possível converter a string para o enum, retorna uma lista vazia
        return Enumerable.Empty<Product>();
    }

    public override async Task<IEnumerable<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
