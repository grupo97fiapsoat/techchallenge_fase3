using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Comando para confirmar o pagamento de um pedido
/// </summary>
public class ConfirmPaymentCommand : IRequest<ConfirmPaymentCommandResult>
{
    /// <summary>
    /// ID do pedido a ser confirmado
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// ID da preferência de pagamento (recomendado)
    /// </summary>
    public string? PreferenceId { get; set; }

    /// <summary>
    /// QR Code usado para pagamento (compatibilidade retroativa)
    /// </summary>
    public string? QrCode { get; set; }
}

/// <summary>
/// Resultado da confirmação do pagamento
/// </summary>
public class ConfirmPaymentCommandResult
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
}
