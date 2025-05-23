using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Comando para processar o checkout de um pedido
/// </summary>
public class ProcessCheckoutCommand : IRequest<ProcessCheckoutCommandResult>
{
    /// <summary>
    /// ID do pedido a ser processado
    /// </summary>
    public Guid OrderId { get; set; }
}

/// <summary>
/// Resultado do processamento do checkout
/// </summary>
public class ProcessCheckoutCommandResult
{
    /// <summary>
    /// ID do pedido processado
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// QR Code gerado para pagamento
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
