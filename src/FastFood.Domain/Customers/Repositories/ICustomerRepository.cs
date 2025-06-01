using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Shared.Repositories;

namespace FastFood.Domain.Customers.Repositories;

/// <summary>
/// Interface para o repositório de clientes.
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Obtém um cliente pelo seu CPF.
    /// </summary>
    /// <param name="cpf">O CPF do cliente</param>
    /// <returns>O cliente encontrado ou null se não existir</returns>
    Task<Customer?> GetByCpfAsync(string cpf);
}
