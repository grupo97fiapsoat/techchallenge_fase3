using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para confirmação de pagamento de um pedido
/// </summary>
public class ConfirmPaymentDto
{
    /// <summary>
    /// QR Code usado para pagamento
    /// </summary>
    /// <example>QR_CODE_12345_ABCDEF</example>
    [Required(ErrorMessage = "O QR Code é obrigatório")]
    [MinLength(10, ErrorMessage = "QR Code deve ter pelo menos 10 caracteres")]
    public required string QrCode { get; set; }
}

/// <summary>
/// DTO com o resultado da confirmação do pagamento
/// </summary>
public class ConfirmPaymentResponseDto
{
    /// <summary>
    /// ID do pedido processado
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Status atual do pedido
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Data/hora da confirmação
    /// </summary>
    public DateTime ConfirmedAt { get; set; }
    
    /// <summary>
    /// Indica se o pagamento foi confirmado com sucesso
    /// </summary>
    public bool PaymentConfirmed { get; set; }
    
    /// <summary>
    /// Mensagem sobre o resultado do pagamento
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
