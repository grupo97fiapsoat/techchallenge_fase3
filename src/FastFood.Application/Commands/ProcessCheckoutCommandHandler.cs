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
    private readonly ILogger<ProcessCheckoutCommandHandler> _logger;    public ProcessCheckoutCommandHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        ILogger<ProcessCheckoutCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _logger = logger;
        
        // Log para debug - verificar qual serviço foi injetado
        _logger.LogInformation("[CHECKOUT] PaymentService injetado: {PaymentServiceType}", 
            _paymentService.GetType().Name);
    }

    public async Task<ProcessCheckoutCommandResult> Handle(ProcessCheckoutCommand request, CancellationToken cancellationToken)
    {        try
        {
            _logger.LogInformation("[CHECKOUT] === INICIANDO CHECKOUT ===");
            _logger.LogInformation("[CHECKOUT] Pedido ID: {OrderId}", request.OrderId);

            // Buscar o pedido com os itens
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException($"Pedido com ID {request.OrderId} não encontrado");

            // Verificar se o pedido está no status correto (Pending)
            if (order.Status != OrderStatus.Pending)
            {
                throw new ValidationException($"O pedido {request.OrderId} não está no status correto para checkout. Status atual: {order.Status}");
            }

            _logger.LogInformation("[CHECKOUT] Pedido encontrado - Status: {Status}, Total: {Total}", 
                order.Status, order.TotalPrice);

            // Gerar QR Code para pagamento
            _logger.LogInformation("[CHECKOUT] Gerando QR Code com {PaymentService}...", 
                _paymentService.GetType().Name);
            
            var (qrCode, preferenceId) = await _paymentService.GenerateQrCodeAsync(order.Id, order.TotalPrice);

            _logger.LogInformation("[CHECKOUT] QR Code gerado com sucesso para o pedido {OrderId}", request.OrderId);
            _logger.LogInformation("[CHECKOUT] Preference ID: {PreferenceId}", preferenceId);

            // Primeiro, associar o QR Code ao pedido (ainda em status Pending)
            order.SetQrCode(qrCode);
            order.SetPreferenceId(preferenceId);
            
            // Depois, atualizar o status para AwaitingPayment
            order.UpdateStatus(OrderStatus.AwaitingPayment);
            
            // Salvar as alterações no banco de dados
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("[CHECKOUT] Checkout do pedido {OrderId} processado com SUCESSO. Status: AwaitingPayment", request.OrderId);

            // Retornar o resultado
            return new ProcessCheckoutCommandResult
            {
                OrderId = order.Id,
                QrCode = qrCode,
                PreferenceId = preferenceId,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalPrice,
                ProcessedAt = Entity.GetBrasilDateTime()
            };
        }        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "[CHECKOUT] Erro ao processar checkout do pedido {OrderId}", request.OrderId);
            throw;
        }
        finally
        {
            _logger.LogInformation("[CHECKOUT] === FIM DO CHECKOUT ===");
        }
    }
}
