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
    private readonly ILogger<ConfirmPaymentCommandHandler> _logger;

    public ConfirmPaymentCommandHandler(
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
    }

    public async Task<ConfirmPaymentCommandResult> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("[CONFIRM PAYMENT] === INICIANDO CONFIRMAÇÃO DE PAGAMENTO ===");
            _logger.LogInformation("[CONFIRM PAYMENT] Pedido ID: {OrderId}", request.OrderId);
            _logger.LogInformation("[CONFIRM PAYMENT] PreferenceId recebido: {PreferenceId}", request.PreferenceId ?? "NULL");
            _logger.LogInformation("[CONFIRM PAYMENT] QrCode recebido: {QrCode}", request.QrCode ?? "NULL");

            // Validação de entrada - pelo menos um dos campos deve estar preenchido
            if (string.IsNullOrWhiteSpace(request.PreferenceId) && string.IsNullOrWhiteSpace(request.QrCode))
            {
                throw new ValidationException("É obrigatório fornecer o PreferenceId ou o QrCode para confirmação do pagamento");
            }

            // Buscar o pedido
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException($"Pedido com ID {request.OrderId} não encontrado");

            _logger.LogInformation("[CONFIRM PAYMENT] Pedido encontrado - Status: {Status}, QR Code: {QrCode}, PreferenceId: {PreferenceId}", 
                order.Status, order.QrCode ?? "NULL", order.PreferenceId ?? "NULL");

            // Verificar se o pedido está no status correto (AwaitingPayment)
            if (order.Status != OrderStatus.AwaitingPayment)
            {
                throw new ValidationException($"O pedido {request.OrderId} não está aguardando pagamento. Status atual: {order.Status}");
            }

            // Verificar se o pedido possui dados de pagamento gerados
            if (string.IsNullOrWhiteSpace(order.QrCode) && string.IsNullOrWhiteSpace(order.PreferenceId))
            {
                throw new ValidationException($"O pedido {request.OrderId} não possui dados de pagamento gerados");
            }

            // Determinar qual método de validação usar
            string paymentIdentifier;
            string validationMethod;

            if (!string.IsNullOrWhiteSpace(request.PreferenceId))
            {
                // Priorizar PreferenceId se fornecido
                paymentIdentifier = request.PreferenceId;
                validationMethod = "PreferenceId";
                
                // Validar se o PreferenceId corresponde ao salvo no banco
                if (!string.IsNullOrWhiteSpace(order.PreferenceId) && 
                    !string.Equals(order.PreferenceId.Trim(), request.PreferenceId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("[CONFIRM PAYMENT] PreferenceId inválido. Esperado: '{Expected}', Recebido: '{Received}'", 
                        order.PreferenceId, request.PreferenceId);
                    throw new ValidationException("PreferenceId fornecido não corresponde ao pedido");
                }
            }
            else
            {
                // Usar QR Code como fallback
                paymentIdentifier = request.QrCode!;
                validationMethod = "QrCode";
                
                // Validação opcional do QR Code (para compatibilidade retroativa)
                if (!string.IsNullOrWhiteSpace(order.QrCode) && 
                    !string.Equals(order.QrCode.Trim(), request.QrCode!.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("[CONFIRM PAYMENT] QR Code pode não corresponder. Prosseguindo com validação no serviço de pagamento...");
                }
            }

            // Processar o pagamento usando o serviço
            _logger.LogInformation("[CONFIRM PAYMENT] Processando pagamento via {ValidationMethod} com {PaymentService}...", 
                validationMethod, _paymentService.GetType().Name);
            
            // Para o ProcessPaymentAsync, ainda passamos o QR Code (que pode ser extraído do PreferenceId se necessário)
            var qrCodeForValidation = !string.IsNullOrWhiteSpace(request.QrCode) ? request.QrCode : order.QrCode ?? "";
            
            var paymentSuccess = await _paymentService.ProcessPaymentAsync(order.Id, qrCodeForValidation);
            
            if (paymentSuccess)
            {
                // Atualizar o status do pedido para Paid
                order.UpdateStatus(OrderStatus.Paid);
                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("[CONFIRM PAYMENT] Pagamento do pedido {OrderId} confirmado com SUCESSO via {ValidationMethod}", 
                    request.OrderId, validationMethod);
                
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
                _logger.LogWarning("[CONFIRM PAYMENT] FALHA na confirmação do pagamento do pedido {OrderId} via {ValidationMethod}", 
                    request.OrderId, validationMethod);
                
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
            _logger.LogError(ex, "[CONFIRM PAYMENT] Erro ao confirmar pagamento do pedido {OrderId}", request.OrderId);
            throw;
        }
        finally
        {
            _logger.LogInformation("[CONFIRM PAYMENT] === FIM DA CONFIRMAÇÃO DE PAGAMENTO ===");
        }
    }
}
