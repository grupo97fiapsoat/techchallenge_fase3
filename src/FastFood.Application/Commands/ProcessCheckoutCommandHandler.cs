using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Orders.ValueObjects;
using FastFood.Domain.Shared.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastFood.Application.Commands;

/// <summary>
/// Handler do comando de processamento de checkout
/// </summary>
public class ProcessCheckoutCommandHandler : IRequestHandler<ProcessCheckoutCommand, ProcessCheckoutCommandResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<ProcessCheckoutCommandHandler> _logger;

    public ProcessCheckoutCommandHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        ILogger<ProcessCheckoutCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<ProcessCheckoutCommandResult> Handle(ProcessCheckoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando checkout do pedido {OrderId}", request.OrderId);

            // Buscar o pedido com os itens
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException($"Pedido com ID {request.OrderId} não encontrado");

            // Verificar se o pedido está no status correto (Pending)
            if (order.Status != OrderStatus.Pending)
            {
                throw new ValidationException($"O pedido {request.OrderId} não está no status correto para checkout. Status atual: {order.Status}");
            }            // Gerar QR Code para pagamento
            var qrCode = await _paymentService.GenerateQrCodeAsync(order.Id, order.TotalPrice);

            _logger.LogInformation("QR Code gerado para o pedido {OrderId}. Aguardando confirmação de pagamento", request.OrderId);

            // Primeiro, associar o QR Code ao pedido (ainda em status Pending)
            order.SetQrCode(qrCode);
            
            // Depois, atualizar o status para AwaitingPayment
            order.UpdateStatus(OrderStatus.AwaitingPayment);
            
            // Salvar as alterações no banco de dados
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("Checkout do pedido {OrderId} processado com sucesso. Status alterado para AwaitingPayment", request.OrderId);

            // Retornar o resultado
            return new ProcessCheckoutCommandResult
            {
                OrderId = order.Id,
                QrCode = qrCode,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalPrice,
                ProcessedAt = Entity.GetBrasilDateTime()
            };
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Erro ao processar checkout do pedido {OrderId}", request.OrderId);
            throw;
        }
    }
}
