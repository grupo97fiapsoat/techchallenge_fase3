using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.ValueObjects;

namespace FastFood.Domain.Orders.Services;

/// <summary>
/// Interface para serviço de notificação de mudanças de status de pedidos.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifica o cliente sobre uma mudança de status no pedido.
    /// </summary>
    /// <param name="order">Pedido atualizado</param>
    /// <param name="previousStatus">Status anterior do pedido</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task NotifyOrderStatusChangeAsync(Order order, OrderStatus previousStatus);
    
    /// <summary>
    /// Notifica o cliente que o pedido está pronto para retirada.
    /// </summary>
    /// <param name="order">Pedido pronto</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task NotifyOrderReadyAsync(Order order);
}
