namespace FastFood.Domain.Orders.ValueObjects;

/// <summary>
/// Representa os possíveis status de um pedido.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Pedido pendente (recém-criado).
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Pedido em processamento (em preparação).
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Pedido pronto para retirada.
    /// </summary>
    Ready = 2,
    
    /// <summary>
    /// Pedido concluído (entregue ao cliente).
    /// </summary>
    Completed = 3,
      /// <summary>
    /// Pedido cancelado.
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// Pedido pago.
    /// </summary>
    Paid = 5,
    
    /// <summary>
    /// Aguardando confirmação de pagamento.
    /// </summary>
    AwaitingPayment = 6
}
