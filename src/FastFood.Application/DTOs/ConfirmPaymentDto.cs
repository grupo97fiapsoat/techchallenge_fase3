using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para confirmação de pagamento de um pedido
/// </summary>
public class ConfirmPaymentDto
{
    /// <summary>
    /// ID da preferência de pagamento (recomendado)
    /// </summary>
    /// <example>FAKE-12345678-abcd-1234-abcd-123456789abc</example>
    public string? PreferenceId { get; set; }

    /// <summary>
    /// QR Code usado para pagamento (compatibilidade retroativa)
    /// </summary>
    /// <example>https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=FAKE-12345</example>
    public string? QrCode { get; set; }

    /// <summary>
    /// Valida se pelo menos um dos campos foi fornecido
    /// </summary>
    public bool IsValid => !string.IsNullOrWhiteSpace(PreferenceId) || !string.IsNullOrWhiteSpace(QrCode);
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
