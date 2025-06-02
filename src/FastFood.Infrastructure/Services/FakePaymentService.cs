using System;
using System.Threading.Tasks;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.Services;
using Microsoft.Extensions.Logging;

namespace FastFood.Infrastructure.Services;

/// <summary>
/// Implementação simulada do serviço de pagamento para desenvolvimento e testes.
/// </summary>
public class FakePaymentService : IPaymentService
{
    private readonly ILogger<FakePaymentService> _logger;
    private readonly IOrderRepository _orderRepository;

    public FakePaymentService(
        ILogger<FakePaymentService> logger,
        IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Gera um QR Code simulado para pagamento.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="amount">Valor a ser pago.</param>
    /// <returns>Tupla contendo a URL simulada do QR Code e um ID de preferência fake.</returns>
    public async Task<(string QrCodeUrl, string PreferenceId)> GenerateQrCodeAsync(Guid orderId, decimal amount)
    {
        // Simula uma latência de rede
        await Task.Delay(100);

        // Gera um QR Code fake no formato: ORDEM_{orderId}_VALOR_{amount}
        var qrCode = $"https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=FAKE-{Guid.NewGuid()}";
        var preferenceId = $"FAKE-{Guid.NewGuid()}";
        
        _logger.LogInformation("QR Code gerado para o pedido {OrderId}: {QrCode}", orderId, qrCode);

        return (qrCode, preferenceId);
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

        // Busca o pedido no banco de dados para validar o QR Code
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null || string.IsNullOrEmpty(order.QrCode) || order.QrCode != qrCode)
        {
            _logger.LogWarning("QR Code inválido ou não encontrado para o pedido {OrderId}", orderId);
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
