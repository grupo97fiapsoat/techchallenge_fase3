using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Orders.ValueObjects;
using FastFood.Domain.Shared.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;

namespace FastFood.Application.Commands;

/// <summary>
/// Handler do comando de confirmação de pagamento
/// </summary>
public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentCommandResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<ConfirmPaymentCommandHandler> _logger;    public ConfirmPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        ILogger<ConfirmPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _logger = logger;
        
        // Log para debug - verificar qual serviço foi injetado
        _logger.LogInformation("[CONFIRM PAYMENT] PaymentService injetado: {PaymentServiceType}", 
            _paymentService.GetType().Name);
    }    public async Task<ConfirmPaymentCommandResult> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("[CONFIRM PAYMENT] === INICIANDO CONFIRMAÇÃO DE PAGAMENTO ===");
            _logger.LogInformation("[CONFIRM PAYMENT] Pedido ID: {OrderId}", request.OrderId);
            _logger.LogInformation("[CONFIRM PAYMENT] QR Code recebido: {QrCode}", request.QrCode);

            // Validação de entrada
            if (string.IsNullOrWhiteSpace(request.QrCode))
            {
                throw new ValidationException("QR Code é obrigatório para confirmação do pagamento");
            }

            // Buscar o pedido
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException($"Pedido com ID {request.OrderId} não encontrado");

            _logger.LogInformation("[CONFIRM PAYMENT] Pedido encontrado - Status: {Status}, QR Code: {QrCode}", 
                order.Status, order.QrCode ?? "NULL");

            // Verificar se o pedido está no status correto (AwaitingPayment)
            if (order.Status != OrderStatus.AwaitingPayment)
            {
                throw new ValidationException($"O pedido {request.OrderId} não está aguardando pagamento. Status atual: {order.Status}");
            }

            // Verificar se o pedido possui QR Code
            if (string.IsNullOrWhiteSpace(order.QrCode))
            {
                throw new ValidationException($"O pedido {request.OrderId} não possui QR Code gerado");
            }

            // Processar o pagamento usando o serviço
            _logger.LogInformation("[CONFIRM PAYMENT] Processando pagamento com {PaymentService}...", 
                _paymentService.GetType().Name);
            
            var paymentSuccess = await _paymentService.ProcessPaymentAsync(order.Id, request.QrCode);
            
            if (paymentSuccess)
            {
                // Atualizar o status do pedido para Paid
                order.UpdateStatus(OrderStatus.Paid);
                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("[CONFIRM PAYMENT] Pagamento do pedido {OrderId} confirmado com SUCESSO", request.OrderId);
                
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
                _logger.LogWarning("[CONFIRM PAYMENT] FALHA na confirmação do pagamento do pedido {OrderId}", request.OrderId);
                
                return new ConfirmPaymentCommandResult
                {
                    OrderId = order.Id,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalPrice,
                    ConfirmedAt = DateTime.UtcNow,
                    PaymentConfirmed = false
                };
            }        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            _logger.LogError(ex, "[CONFIRM PAYMENT] Erro ao confirmar pagamento do pedido {OrderId}", request.OrderId);
            throw;
        }
        finally
        {
            _logger.LogInformation("[CONFIRM PAYMENT] === FIM DA CONFIRMAÇÃO DE PAGAMENTO ===");
        }
    }
}
