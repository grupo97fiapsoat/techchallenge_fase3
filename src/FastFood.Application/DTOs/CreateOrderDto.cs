using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para criação de um novo pedido
/// </summary>
public class CreateOrderDto
{    /// <summary>
    /// ID do cliente que está fazendo o pedido (opcional para pedidos anônimos)
    /// </summary>
    /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
    public Guid? CustomerId { get; set; }    /// <summary>
    /// Lista de itens do pedido (mínimo 1 item)
    /// </summary>
    [Required(ErrorMessage = "Os itens do pedido são obrigatórios")]
    [MinLength(1, ErrorMessage = "O pedido deve ter pelo menos um item")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
