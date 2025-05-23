using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para atualização do status de um pedido
/// </summary>
public class UpdateOrderStatusDto
{
    /// <summary>
    /// Novo status do pedido
    /// </summary>
    /// <example>Processing</example>
    [Required(ErrorMessage = "O status é obrigatório")]
    [EnumDataType(typeof(OrderStatusEnum), ErrorMessage = "Status inválido")]
    public string Status { get; set; }
}

/// <summary>
/// Status possíveis para um pedido
/// </summary>
public enum OrderStatusEnum
{
    /// <summary>
    /// Pedido pendente de processamento
    /// </summary>
    Pending,

    /// <summary>
    /// Pedido em processamento na cozinha
    /// </summary>
    Processing,

    /// <summary>
    /// Pedido pronto para retirada
    /// </summary>
    Ready,

    /// <summary>
    /// Pedido concluído e entregue ao cliente
    /// </summary>
    Completed,

    /// <summary>
    /// Pedido cancelado
    /// </summary>
    Cancelled
}
