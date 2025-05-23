using System;
using System.Threading.Tasks;
using FastFood.Domain.Orders.Services;
using Microsoft.Extensions.Logging;

namespace FastFood.Infrastructure.Services;

/// <summary>
/// Implementação simulada do serviço de pagamento para desenvolvimento e testes.
/// </summary>
public class FakePaymentService : IPaymentService
{
    private readonly ILogger<FakePaymentService> _logger;
    private readonly Dictionary<Guid, string> _qrCodes;

    public FakePaymentService(ILogger<FakePaymentService> logger)
    {
        _logger = logger;
        _qrCodes = new Dictionary<Guid, string>();
    }

    /// <summary>
    /// Gera um QR Code simulado para pagamento.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="amount">Valor a ser pago.</param>
    /// <returns>String simulando um QR Code.</returns>
    public async Task<string> GenerateQrCodeAsync(Guid orderId, decimal amount)
    {
        // Simula uma latência de rede
        await Task.Delay(100);

        // Gera um QR Code fake no formato: ORDEM_{orderId}_VALOR_{amount}
        var qrCode = $"ORDEM_{orderId}_VALOR_{amount:F2}";
        _qrCodes[orderId] = qrCode;

        _logger.LogInformation("QR Code gerado para o pedido {OrderId}: {QrCode}", orderId, qrCode);

        return qrCode;
    }

    /// <summary>
    /// Simula o processamento de um pagamento.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="qrCode">QR Code usado para pagamento.</param>
    /// <returns>True em 95% dos casos, simulando uma taxa de sucesso realista.</returns>
    public async Task<bool> ProcessPaymentAsync(Guid orderId, string qrCode)
    {
        // Simula uma latência de rede
        await Task.Delay(500);

        // Verifica se o QR Code é válido
        if (!_qrCodes.TryGetValue(orderId, out var storedQrCode) || storedQrCode != qrCode)
        {
            _logger.LogWarning("QR Code inválido para o pedido {OrderId}", orderId);
            return false;
        }

        // Simula uma taxa de sucesso de 95%
        var success = Random.Shared.NextDouble() <= 0.95;

        if (success)
        {
            _logger.LogInformation("Pagamento processado com sucesso para o pedido {OrderId}", orderId);
        }
        else
        {
            _logger.LogWarning("Falha no processamento do pagamento para o pedido {OrderId}", orderId);
        }

        return success;
    }
}
