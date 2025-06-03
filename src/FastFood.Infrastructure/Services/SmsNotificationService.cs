using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Orders.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace FastFood.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de notificação por SMS.
/// </summary>
public class SmsNotificationService : INotificationService
{
    private readonly ILogger<SmsNotificationService> _logger;
    private readonly IConfiguration _configuration;
    // Aqui poderíamos injetar um cliente de SMS real

    public SmsNotificationService(ILogger<SmsNotificationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task NotifyOrderStatusChangeAsync(Order order, OrderStatus previousStatus)
    {
        if (order == null)
        {
            _logger.LogWarning("Tentativa de notificar mudança de status para um pedido nulo");
            return;
        }

        _logger.LogInformation(
            "Enviando SMS para cliente {CustomerId} sobre mudança de status do pedido {OrderId}: {PreviousStatus} -> {CurrentStatus}",
            order.CustomerId, order.Id, previousStatus, order.Status);

        // Aqui implementaríamos a lógica real de envio de SMS
        // Por enquanto, apenas simulamos o envio

        string smsText = $"FastFood: Seu pedido #{order.Id} teve o status atualizado de {previousStatus} para {order.Status}.";

        // Simular tempo de processamento do envio de SMS
        await Task.Delay(100);
        
        _logger.LogInformation(
            "SMS enviado para o cliente {CustomerId} sobre o pedido {OrderId}",
            order.CustomerId, order.Id);
    }

    public async Task NotifyOrderReadyAsync(Order order)
    {
        if (order == null)
        {
            _logger.LogWarning("Tentativa de notificar pedido pronto para um pedido nulo");
            return;
        }

        if (order.Status != OrderStatus.Ready)
        {
            _logger.LogWarning(
                "Tentativa de notificar pedido pronto para um pedido com status inválido. Pedido {OrderId} com status {Status}",
                order.Id, order.Status);
            return;
        }

        _logger.LogInformation(
            "Enviando SMS para cliente {CustomerId} que o pedido {OrderId} está pronto para retirada",
            order.CustomerId, order.Id);

        // Aqui implementaríamos a lógica real de envio de SMS
        // Por enquanto, apenas simulamos o envio

        string smsText = $"FastFood: Seu pedido #{order.Id} está pronto para retirada no balcão!";

        // Simular tempo de processamento do envio de SMS
        await Task.Delay(100);
        
        _logger.LogInformation(
            "SMS de pedido pronto enviado para o cliente {CustomerId} sobre o pedido {OrderId}",
            order.CustomerId, order.Id);
    }
}
