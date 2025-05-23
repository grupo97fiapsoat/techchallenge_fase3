using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.ValueObjects;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de pedidos usando Entity Framework Core.
/// </summary>
public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(FastFoodDbContext context) : base(context)
    {
    }

    public override async Task<Order?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithItemsAsync(Guid id)
    {
        return await DbSet
            .Include(o => o.Customer)
            .Include(o => o.Items) // Mudança: usar expressão lambda
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public override async Task<IEnumerable<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .Include(o => o.Customer)
            .Include(o => o.Items) // Mudança: usar expressão lambda
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .Include(o => o.Customer)
            .Include(o => o.Items) // Mudança: usar expressão lambda
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .Include(o => o.Customer)
            .Include(o => o.Items) // Mudança: usar expressão lambda
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersAsync(
        int pageNumber,
        int pageSize,
        Guid? customerId = null,
        OrderStatus? status = null)
    {
        var query = DbSet
            .Include(o => o.Customer)
            .Include(o => o.Items) // Mudança: usar expressão lambda
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId);

        if (status.HasValue)
            query = query.Where(o => o.Status == status);

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }
}