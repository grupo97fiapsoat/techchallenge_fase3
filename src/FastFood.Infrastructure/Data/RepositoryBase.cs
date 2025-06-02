using FastFood.Domain.Shared.Entities;
using FastFood.Domain.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Data;

/// <summary>
/// Classe base para implementação de repositórios usando Entity Framework Core.
/// </summary>
/// <typeparam name="T">O tipo da entidade gerenciada pelo repositório</typeparam>
public abstract class RepositoryBase<T> : IRepository<T> where T : class, IEntity
{
    protected readonly FastFoodDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected RepositoryBase(FastFoodDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }
    
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await DbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }    public virtual async Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    protected virtual IQueryable<T> AddPagination(IQueryable<T> query, int pageNumber = 1, int pageSize = 10)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}