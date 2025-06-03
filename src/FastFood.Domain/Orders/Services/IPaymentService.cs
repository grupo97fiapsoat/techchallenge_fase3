using System;
using System.Threading.Tasks;

namespace FastFood.Domain.Orders.Services;

/// <summary>
/// Interface para serviço de pagamento.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Gera um QR Code para pagamento de um pedido.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="amount">Valor a ser pago.</param>
    /// <returns>Tupla contendo a URL do QR Code e o ID da preferência no Mercado Pago.</returns>
    Task<(string QrCodeUrl, string PreferenceId)> GenerateQrCodeAsync(Guid orderId, decimal amount);

    /// <summary>
    /// Processa o pagamento de um pedido.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="qrCode">QR Code usado para pagamento.</param>
    /// <returns>True se o pagamento foi processado com sucesso; False caso contrário.</returns>
    Task<bool> ProcessPaymentAsync(Guid orderId, string qrCode);
}
