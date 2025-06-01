using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.ValueObjects;
using FastFood.Domain.Shared.Repositories;

namespace FastFood.Domain.Orders.Repositories;

/// <summary>
/// Interface para o repositório de pedidos.
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Obtém um pedido pelo seu identificador, incluindo seus itens.
    /// </summary>
    /// <param name="id">O identificador do pedido</param>
    /// <returns>O pedido encontrado ou null se não existir</returns>
    Task<Order?> GetByIdWithItemsAsync(Guid id);

    /// <summary>
    /// Obtém pedidos por cliente, com suporte a paginação.
    /// </summary>
    /// <param name="customerId">O identificador do cliente</param>
    /// <param name="pageNumber">O número da página, começando em 1</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <returns>Uma coleção com os pedidos encontrados</returns>
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Obtém pedidos por status, com suporte a paginação.
    /// </summary>
    /// <param name="status">O status dos pedidos</param>
    /// <param name="pageNumber">O número da página, começando em 1</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <returns>Uma coleção com os pedidos encontrados</returns>
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Obtém pedidos com filtros e paginação, retornando também o total de registros.
    /// </summary>
    /// <param name="pageNumber">O número da página, começando em 1</param>
    /// <param name="pageSize">O tamanho da página</param>
    /// <param name="customerId">Filtro opcional por cliente</param>
    /// <param name="status">Filtro opcional por status</param>
    /// <returns>Uma tupla com os pedidos encontrados e o total de registros</returns>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersAsync(int pageNumber, int pageSize, Guid? customerId = null, OrderStatus? status = null);
}
