using FastFood.Application.Common.Exceptions;
using FastFood.Application.DTOs;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Orders.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastFood.Application.Commands;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusCommandResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        INotificationService notificationService,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<UpdateOrderStatusCommandResult> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {        try
        {
            _logger.LogInformation("Atualizando status do pedido {OrderId} para {NewStatus}", request.Id, request.Status);
            
            // Buscar o pedido pelo ID com detalhes do cliente
            var order = await _orderRepository.GetByIdWithItemsAsync(request.Id);
            if (order == null)
                throw new NotFoundException($"Pedido com ID {request.Id} não encontrado");

            // Converter a string para enum
            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
                throw new ValidationException($"Status inválido: {request.Status}");

            // Guardar o status anterior para notificação
            var previousStatus = order.Status;
            
            // Atualizar o status do pedido
            order.UpdateStatus(newStatus);

            // Persistir a atualização
            await _orderRepository.UpdateAsync(order);
            
            // Notificar o cliente sobre a mudança de status
            await _notificationService.NotifyOrderStatusChangeAsync(order, previousStatus);
            
            // Se o status foi atualizado para Ready (Pronto), enviar notificação específica
            if (newStatus == OrderStatus.Ready)
            {
                _logger.LogInformation("Pedido {OrderId} está pronto para retirada! Enviando notificação especial", order.Id);
                await _notificationService.NotifyOrderReadyAsync(order);
            }            // Retornar o resultado
            var result = new UpdateOrderStatusCommandResult
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                UpdatedAt = order.UpdatedAt.Value,
                NotificationSent = true // Indicar que a notificação foi enviada
            };
            
            _logger.LogInformation("Status do pedido {OrderId} atualizado com sucesso para {NewStatus}", order.Id, newStatus);
            return result;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId} para {NewStatus}", request.Id, request.Status);
            throw;
        }
    }
}
