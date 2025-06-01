namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para processar o checkout de um pedido
/// </summary>
public class ProcessCheckoutDto
{
    /// <summary>
    /// ID do pedido a processar
    /// </summary>
    public Guid OrderId { get; set; }
}

/// <summary>
/// DTO com o resultado do processamento do checkout
/// </summary>
public class CheckoutResponseDto
{
    /// <summary>
    /// ID do pedido processado
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// QR Code para pagamento
    /// </summary>
    public string QrCode { get; set; }

    /// <summary>
    /// Status atual do pedido
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Data/hora do processamento
    /// </summary>
    public DateTime ProcessedAt { get; set; }
}