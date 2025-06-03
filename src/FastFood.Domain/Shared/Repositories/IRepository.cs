using FastFood.Domain.Shared.Entities;

namespace FastFood.Domain.Shared.Repositories;

/// <summary>
/// Interface genérica para os repositórios do domínio.
/// </summary>
/// <typeparam name="T">O tipo da entidade gerenciada pelo repositório</typeparam>
public interface IRepository<T> where T : IEntity
{    /// <summary>
    /// Obtém uma entidade pelo seu identificador.
    /// </summary>
    /// <param name="id">O identificador da entidade</param>
    /// <returns>A entidade encontrada ou null se não existir</returns>
    Task<T?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Obtém uma entidade pelo seu identificador com suporte a cancelamento.
    /// </summary>
    /// <param name="id">O identificador da entidade</param>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>A entidade encontrada ou null se não existir</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todas as entidades do tipo, com suporte a paginação.
    /// </summary>
    /// <param name="pageNumber">O número da página, começando em 1</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <returns>Uma coleção com as entidades encontradas</returns>
    Task<IEnumerable<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    
    /// <summary>
    /// Obtém todas as entidades do tipo, com suporte a paginação e cancelamento.
    /// </summary>
    /// <param name="pageNumber">O número da página, começando em 1</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    /// <returns>Uma coleção com as entidades encontradas</returns>
    Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Cria uma nova entidade no repositório.
    /// </summary>
    /// <param name="entity">A entidade a ser criada</param>
    /// <returns>A entidade criada com seu identificador atualizado</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Atualiza uma entidade existente no repositório.
    /// </summary>
    /// <param name="entity">A entidade a ser atualizada</param>
    Task UpdateAsync(T entity);    /// <summary>
    /// Remove uma entidade do repositório.
    /// </summary>
    /// <param name="entity">A entidade a ser removida</param>
    Task DeleteAsync(T entity);
    
    /// <summary>
    /// Remove uma entidade do repositório com suporte a cancelamento.
    /// </summary>
    /// <param name="entity">A entidade a ser removida</param>
    /// <param name="cancellationToken">Token de cancelamento da operação</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken);
}