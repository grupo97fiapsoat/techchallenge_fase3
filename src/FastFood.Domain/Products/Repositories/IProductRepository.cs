using FastFood.Domain.Products.Entities;
using FastFood.Domain.Shared.Repositories;

namespace FastFood.Domain.Products.Repositories;

/// <summary>
/// Interface para o repositório de produtos.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Obtém produtos por categoria, com suporte a paginação.
    /// </summary>
    /// <param name="category">A categoria dos produtos</param>
    /// <param name="pageNumber">O número da página, começando em 1</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <returns>Uma coleção com os produtos encontrados</returns>
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, int pageNumber = 1, int pageSize = 10);
}
