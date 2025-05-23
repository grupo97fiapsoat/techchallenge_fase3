using FastFood.Domain.Products.Entities;
using FastFood.Domain.Products.Repositories;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de produtos usando Entity Framework Core.
/// </summary>
public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(FastFoodDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
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
