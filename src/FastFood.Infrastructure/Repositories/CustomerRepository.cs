using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Customers.Repositories;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de clientes usando Entity Framework Core.
/// </summary>
public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(FastFoodDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByCpfAsync(string cpf)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.Cpf.Value == cpf);
    }

    public override async Task<IEnumerable<Customer>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .OrderBy(x => x.Name.Value)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
