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
    }    /// <summary>
    /// Gera um QR Code simulado para pagamento.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="amount">Valor a ser pago.</param>
    /// <returns>Tupla contendo a URL simulada do QR Code e um ID de preferência fake.</returns>
    public async Task<(string QrCodeUrl, string PreferenceId)> GenerateQrCodeAsync(Guid orderId, decimal amount)
    {
        // Simula uma latência de rede
        await Task.Delay(100);

        // Gera um QR Code fake no formato do Mercado Pago para manter consistência
        var preferenceId = $"FAKE-{Guid.NewGuid()}";
        var qrCode = $"https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id={preferenceId}";
        
        _logger.LogInformation("[FAKE PAYMENT] QR Code gerado para o pedido {OrderId}: {QrCode}", orderId, qrCode);
        _logger.LogInformation("[FAKE PAYMENT] Preference ID: {PreferenceId}", preferenceId);

        return (qrCode, preferenceId);
    }    /// <summary>
    /// Simula o processamento de um pagamento.
    /// Valida se o QR Code fornecido corresponde ao QR Code gerado para o pedido.
    /// </summary>
    /// <param name="orderId">ID do pedido.</param>
    /// <param name="qrCode">QR Code usado para pagamento.</param>
    /// <returns>True se o pagamento for válido e processado com sucesso.</returns>
    public async Task<bool> ProcessPaymentAsync(Guid orderId, string qrCode)
    {
        // Simula uma latência de rede
        await Task.Delay(500);

        _logger.LogInformation("[FAKE PAYMENT] === INICIANDO PROCESSAMENTO DE PAGAMENTO ===");
        _logger.LogInformation("[FAKE PAYMENT] Pedido ID: {OrderId}", orderId);
        _logger.LogInformation("[FAKE PAYMENT] QR Code recebido: {QrCodeReceived}", qrCode);

        // Validação de entrada
        if (string.IsNullOrWhiteSpace(qrCode))
        {
            _logger.LogWarning("[FAKE PAYMENT] QR Code recebido está vazio ou nulo");
            return false;
        }

        // Busca o pedido no banco de dados
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null)
        {
            _logger.LogWarning("[FAKE PAYMENT] Pedido {OrderId} não encontrado no banco de dados", orderId);
            return false;
        }

        _logger.LogInformation("[FAKE PAYMENT] Pedido encontrado - Status: {Status}", order.Status);
        _logger.LogInformation("[FAKE PAYMENT] QR Code no banco: {QrCodeInDatabase}", order.QrCode ?? "NULL");
        
        // Validação se o QR Code foi definido para o pedido
        if (string.IsNullOrWhiteSpace(order.QrCode))
        {
            _logger.LogWarning("[FAKE PAYMENT] QR Code não foi definido para o pedido {OrderId}", orderId);
            return false;
        }

        // Comparação exata dos QR Codes
        bool qrCodeMatches = string.Equals(order.QrCode.Trim(), qrCode.Trim(), StringComparison.OrdinalIgnoreCase);
        _logger.LogInformation("[FAKE PAYMENT] QR Codes são iguais? {QrCodeMatches}", qrCodeMatches);
        
        if (!qrCodeMatches)
        {
            _logger.LogWarning("[FAKE PAYMENT] QR Code inválido para o pedido {OrderId}", orderId);
            _logger.LogWarning("[FAKE PAYMENT] Esperado: '{Expected}'", order.QrCode);
            _logger.LogWarning("[FAKE PAYMENT] Recebido: '{Received}'", qrCode);
            return false;
        }

        // Simula uma taxa de sucesso de 95% (apenas para pedidos com QR Code válido)
        var success = Random.Shared.NextDouble() <= 0.95;

        if (success)
        {
            _logger.LogInformation("[FAKE PAYMENT] Pagamento processado com SUCESSO para o pedido {OrderId}", orderId);
        }
        else
        {
            _logger.LogWarning("[FAKE PAYMENT] Falha no processamento do pagamento para o pedido {OrderId} (simulação 5% de falha)", orderId);
        }

        _logger.LogInformation("[FAKE PAYMENT] === FIM DO PROCESSAMENTO DE PAGAMENTO ===");
        return success;
    }
}
