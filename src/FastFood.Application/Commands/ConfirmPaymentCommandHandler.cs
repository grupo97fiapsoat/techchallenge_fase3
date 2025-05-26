using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Orders.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastFood.Application.Commands;

/// <summary>
/// Handler do comando de confirmação de pagamento
/// </summary>
public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentCommandResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<ConfirmPaymentCommandHandler> _logger;

    public ConfirmPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        ILogger<ConfirmPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<ConfirmPaymentCommandResult> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando confirmação de pagamento do pedido {OrderId}", request.OrderId);

            // Buscar o pedido
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException($"Pedido com ID {request.OrderId} não encontrado");

            // Verificar se o pedido está no status correto (AwaitingPayment)
            if (order.Status != OrderStatus.AwaitingPayment)
            {
                throw new ValidationException($"O pedido {request.OrderId} não está aguardando pagamento. Status atual: {order.Status}");
            }

            // Processar o pagamento usando o serviço
            var paymentSuccess = await _paymentService.ProcessPaymentAsync(order.Id, request.QrCode);
            
            if (paymentSuccess)
            {
                // Atualizar o status do pedido para Paid
                order.UpdateStatus(OrderStatus.Paid);
                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("Pagamento do pedido {OrderId} confirmado com sucesso", request.OrderId);
                
                return new ConfirmPaymentCommandResult
                {
                    OrderId = order.Id,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalPrice,
                    ConfirmedAt = DateTime.UtcNow,
                    PaymentConfirmed = true
                };
            }
            else
            {
                _logger.LogWarning("Falha na confirmação do pagamento do pedido {OrderId}", request.OrderId);
                
                return new ConfirmPaymentCommandResult
                {
                    OrderId = order.Id,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalPrice,
                    ConfirmedAt = DateTime.UtcNow,
                    PaymentConfirmed = false
                };
            }
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Erro ao confirmar pagamento do pedido {OrderId}", request.OrderId);
            throw;
        }
    }
}
