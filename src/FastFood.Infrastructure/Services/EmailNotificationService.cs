using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Orders.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace FastFood.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de notificação por email.
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly IConfiguration _configuration;
    // Aqui poderíamos injetar um cliente SMTP real

    public EmailNotificationService(ILogger<EmailNotificationService> logger, IConfiguration configuration)
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
            "Notificando cliente {CustomerId} sobre mudança de status do pedido {OrderId}: {PreviousStatus} -> {CurrentStatus}",
            order.CustomerId, order.Id, previousStatus, order.Status);

        // Aqui implementaríamos a lógica real de envio de email
        // Por enquanto, apenas simulamos o envio

        string emailSubject = $"Atualização do seu pedido #{order.Id}";
        string emailBody = $"Olá!\n\nSeu pedido #{order.Id} teve o status atualizado de {previousStatus} para {order.Status}.\n\nObrigado por escolher o FastFood!";

        // Simular tempo de processamento do envio de email
        await Task.Delay(100); 
        
        _logger.LogInformation(
            "Email enviado para o cliente {CustomerId} sobre o pedido {OrderId}. Assunto: {Subject}",
            order.CustomerId, order.Id, emailSubject);
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
            "Notificando cliente {CustomerId} que o pedido {OrderId} está pronto para retirada",
            order.CustomerId, order.Id);

        // Aqui implementaríamos a lógica real de envio de email
        // Por enquanto, apenas simulamos o envio

        string emailSubject = $"Seu pedido #{order.Id} está pronto para retirada!";
        string emailBody = $"Olá!\n\nSeu pedido #{order.Id} está pronto para retirada no balcão.\n\nObrigado por escolher o FastFood!";

        // Simular tempo de processamento do envio de email
        await Task.Delay(100);
        
        _logger.LogInformation(
            "Email de pedido pronto enviado para o cliente {CustomerId} sobre o pedido {OrderId}. Assunto: {Subject}",
            order.CustomerId, order.Id, emailSubject);
    }
}
