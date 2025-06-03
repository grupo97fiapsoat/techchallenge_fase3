namespace FastFood.Application.DTOs;

/// <summary>
/// DTO com os dados de um pedido
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Identificador único do pedido
    /// </summary>
    public Guid Id { get; set; }    /// <summary>
    /// Identificador do cliente que fez o pedido (null para pedidos anônimos)
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Nome do cliente que fez o pedido (null para pedidos anônimos)
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Lista de itens do pedido
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Status atual do pedido (Pending, Processing, Ready, Completed, Cancelled)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Data e hora de criação do pedido
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização do pedido
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
