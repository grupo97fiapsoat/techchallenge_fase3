namespace FastFood.Application.DTOs;

/// <summary>
/// DTO com os dados de um item do pedido
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Identificador único do item do pedido
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Identificador do produto
    /// </summary>
    public required Guid ProductId { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    public required string ProductName { get; set; }

    /// <summary>
    /// Preço unitário do produto
    /// </summary>
    public required decimal UnitPrice { get; set; }

    /// <summary>
    /// Quantidade solicitada do produto
    /// </summary>
    public required int Quantity { get; set; }

    /// <summary>
    /// Subtotal do item (quantidade * preço unitário)
    /// </summary>
    public required decimal SubTotal { get; set; }

    /// <summary>
    /// Observações do item (exemplo: sem cebola, sem tomate, etc.)
    /// </summary>
    public string? Observation { get; set; }
}
