namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para consulta pública do status de um pedido
/// Usado pelo endpoint público que permite acompanhamento de pedidos anônimos
/// </summary>
public class OrderStatusDto
{
    /// <summary>
    /// ID único do pedido
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Status atual do pedido
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Descrição amigável do status atual
    /// </summary>
    public string StatusDescription { get; set; } = string.Empty;

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Data e hora de criação do pedido
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indica se o pedido foi feito por um cliente anônimo
    /// </summary>
    public bool IsAnonymous { get; set; }
}
