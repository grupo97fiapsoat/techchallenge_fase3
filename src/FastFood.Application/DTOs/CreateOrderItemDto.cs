using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para criação de um item do pedido
/// </summary>
public class CreateOrderItemDto
{
    /// <summary>
    /// ID do produto a ser adicionado ao pedido
    /// </summary>
    /// <example>e47ac10b-58cc-4372-a567-0e02b2c3d123</example>
    [Required(ErrorMessage = "O ID do produto é obrigatório")]
    public required Guid ProductId { get; set; }

    /// <summary>
    /// Quantidade desejada do produto (maior que zero)
    /// </summary>
    /// <example>2</example>
    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public required int Quantity { get; set; }

    /// <summary>
    /// Observações do item (exemplo: sem cebola, sem tomate, etc.)
    /// </summary>
    /// <example>Sem cebola</example>
    public string? Observation { get; set; }
}
