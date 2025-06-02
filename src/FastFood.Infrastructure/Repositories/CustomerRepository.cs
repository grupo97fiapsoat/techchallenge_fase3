using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Customers.Repositories;
using FastFood.Domain.Customers.ValueObjects;
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
    }    public async Task<Customer?> GetByCpfAsync(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return null;

        try
        {
            // Usa o Value Object para fazer a validação e normalização
            var cpfValue = Cpf.Create(cpf);
            
            return await DbSet
                .FirstOrDefaultAsync(x => x.Cpf.Value == cpfValue.Value);
        }
        catch
        {
            // Se o CPF for inválido, retorna null
            return null;
        }
    }
    
    public async Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return null;

        try
        {
            // Usa o Value Object para fazer a validação e normalização
            var cpfValue = Cpf.Create(cpf);
            
            return await DbSet
                .FirstOrDefaultAsync(x => x.Cpf.Value == cpfValue.Value, cancellationToken);
        }
        catch
        {
            // Se o CPF for inválido, retorna null
            return null;
        }
    }    public override async Task<IEnumerable<Customer>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await DbSet
            .OrderBy(x => x.Name.Value)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
      public override async Task<IEnumerable<Customer>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await DbSet
            .OrderBy(x => x.Name.Value)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
