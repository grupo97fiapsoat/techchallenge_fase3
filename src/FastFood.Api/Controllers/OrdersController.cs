using FastFood.Api.Models;
using FastFood.Application.Commands;
using FastFood.Application.Common.Exceptions;
using FastFood.Application.DTOs;
using FastFood.Application.Queries;
using FastFood.Domain.Orders.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

/// <summary>
/// Controlador para gerenciamento de pedidos do restaurante
/// 
/// **Finalidade:** Gerencia todo o ciclo de vida dos pedidos, desde a criação até a entrega.
/// 
/// **Fluxo do pedido:**
/// 1. **Criação** - Cliente cria pedido (público)
/// 2. **Pagamento** - Sistema processa pagamento automaticamente  
/// 3. **Preparação** - Cozinha recebe e prepara o pedido
/// 4. **Finalização** - Cliente retira o pedido
/// 
/// **Status disponíveis:**
/// - **Recebido (0)**: Pedido criado, aguardando pagamento
/// - **EmPreparacao (1)**: Pago e sendo preparado na cozinha
/// - **Pronto (2)**: Pronto para retirada
/// - **Finalizado (3)**: Entregue ao cliente
/// 
/// **Níveis de acesso:**
/// - **Públicos**: Criar pedido, processar checkout, confirmar pagamento
/// - **Protegidos**: Listar todos os pedidos, consultar detalhes, atualizar status
/// </summary>
[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;
    private readonly IPaymentService _paymentService;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger, IPaymentService paymentService)
    {
        _mediator = mediator;
        _logger = logger;
        _paymentService = paymentService;
    }   
    /// <param name="request">Dados do pedido (customerId e lista de itens)</param>
    /// <returns>Pedido criado</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto request)
    {
        _logger.LogInformation("Criando novo pedido para cliente {CustomerId}", request.CustomerId);
        
        var command = new CreateOrderCommand
        {
            CustomerId = request.CustomerId,
            Items = request.Items.Select(item => new CreateOrderItemCommand
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        var result = await _mediator.Send(command);

        var orderDto = new OrderDto
        {
            Id = result.Id,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            Status = result.Status,
            TotalPrice = result.TotalPrice,
            CreatedAt = result.CreatedAt,
            Items = result.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                SubTotal = item.SubTotal
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = orderDto.Id }, orderDto);    }

    
    /// <param name="customerId">ID do cliente para filtrar pedidos (opcional)</param>
    /// <param name="status">Status do pedido para filtrar (opcional)</param>
    /// <param name="pageNumber">Número da página para paginação (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <returns>Lista paginada de pedidos com metadados de paginação</returns>
    /// <response code="200">Lista de pedidos retornada com sucesso</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Token válido mas sem permissões adequadas</response>
    [HttpGet]
    [Authorize] // Exige autenticação para listar todos os pedidos
    [ProducesResponseType(typeof(GetOrdersQueryResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? customerId,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Buscando pedidos. Cliente: {CustomerId}, Status: {Status}, Página: {Page}, Tamanho: {Size}", 
            customerId, status, pageNumber, pageSize);
        
        var query = new GetOrdersQuery
        {
            CustomerId = customerId,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        
        return Ok(result);    }

   
    /// <param name="id">ID único do pedido (formato GUID)</param>
    /// <returns>Detalhes completos do pedido</returns>
    /// <response code="200">Pedido encontrado e retornado com sucesso</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Token válido mas sem permissões adequadas</response>
    /// <response code="404">Pedido não encontrado - Verifique se o ID está correto</response>
    [HttpGet("{id:guid}")]
    [Authorize] // Exige autenticação para visualizar detalhes de um pedido
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        _logger.LogInformation("Buscando pedido por ID: {OrderId}", id);
          var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (!result.Success)
        {
            _logger.LogWarning("Pedido não encontrado. ID: {OrderId}", id);
            var errorMessage = result.Error ?? "Pedido não encontrado";
            return NotFound(new ErrorResponse { Errors = new List<string> { errorMessage } });
        }

        return Ok(result.Order);
    }    
    /// <param name="id">ID único do pedido (formato GUID)</param>
    /// <param name="request">Objeto contendo o novo status do pedido</param>
    /// <returns>Resultado da operação de atualização</returns>
    /// <response code="200">Status atualizado com sucesso</response>    /// <response code="400">Status inválido ou transição não permitida</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Apenas administradores podem atualizar status</response>
    /// <response code="404">Pedido não encontrado - Verifique se o ID está correto</response>
    [HttpPut("{id:guid}/status")]
    [Authorize] // Apenas administradores podem atualizar status
    [ProducesResponseType(typeof(UpdateOrderStatusCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusDto request)
    {
        _logger.LogInformation("Atualizando status do pedido {OrderId} para {Status}", id, request.Status);
        
        try
        {
            var command = new UpdateOrderStatusCommand
            {
                Id = id,
                Status = request.Status
            };

            var result = await _mediator.Send(command);
            
            _logger.LogInformation("Status do pedido {OrderId} atualizado com sucesso para {Status}, notificação enviada: {NotificationSent}", 
                id, request.Status, result.NotificationSent);
                
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Pedido não encontrado para atualização. ID: {OrderId}", id);
            return NotFound(new ErrorResponse { Errors = new List<string> { ex.Message } });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Dados inválidos para atualização de status. ID: {OrderId}, Status: {Status}", id, request.Status);
            return BadRequest(new ErrorResponse { Errors = new List<string> { ex.Message } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId} para {Status}", id, request.Status);
            return StatusCode(500, new ErrorResponse { Errors = new List<string> { "Ocorreu um erro ao processar sua solicitação. Tente novamente mais tarde." } });
        }    }  
    /// <param name="id">ID único do pedido (formato GUID)</param>
    /// <returns>Status atual do pedido</returns>
    /// <response code="200">Status do pedido retornado com sucesso</response>
    /// <response code="404">Pedido não encontrado - Verifique se o ID está correto</response>
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderStatus([FromRoute] Guid id)
    {
        _logger.LogInformation("Consultando status do pedido {OrderId} (endpoint público)", id);
        
        var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);
          if (!result.Success || result.Order == null)
        {
            _logger.LogWarning("Pedido não encontrado para consulta de status. ID: {OrderId}", id);
            var errorMessage = result.Error ?? "Pedido não encontrado";
            return NotFound(new ErrorResponse { Errors = new List<string> { errorMessage } });
        }

        var order = result.Order;
        var statusResponse = new OrderStatusDto
        {
            OrderId = order.Id,
            Status = order.Status,
            StatusDescription = GetStatusDescription(order.Status),
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            IsAnonymous = !order.CustomerId.HasValue
        };

        _logger.LogInformation("Status do pedido {OrderId} consultado com sucesso: {Status}", id, order.Status);
        return Ok(statusResponse);
    }

   
    /// <param name="id">ID único do pedido para gerar pagamento (formato GUID)</param>
    /// <returns>QR Code e dados para pagamento via MercadoPago</returns>
    /// <response code="200">QR Code gerado com sucesso - Cliente pode efetuar pagamento</response>    /// <response code="400">Erro na geração do QR Code - Verifique se o pedido está válido</response>
    /// <response code="404">Pedido não encontrado - Verifique se o ID está correto</response>
    [HttpPost("{id:guid}/checkout")]
    [ProducesResponseType(typeof(CheckoutResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessCheckout([FromRoute] Guid id)
    {
        _logger.LogInformation("Iniciando processamento de checkout do pedido {OrderId}", id);

        var command = new ProcessCheckoutCommand { OrderId = id };
        var result = await _mediator.Send(command);

        // Gera o QR Code e obtém a preferência do Mercado Pago
        string qrCode = result.QrCode;
        string preferenceId = result.PreferenceId;

        var response = new CheckoutResponseDto
        {
            OrderId = result.OrderId,
            QrCode = qrCode,
            PreferenceId = preferenceId,
            TotalAmount = result.TotalAmount,
            ProcessedAt = result.ProcessedAt
        };

        _logger.LogInformation("Checkout do pedido {OrderId} processado com sucesso. QR Code gerado.", id);
        return Ok(response);    }

    /// <param name="id">ID único do pedido para confirmar pagamento (formato GUID)</param>
    /// <param name="request">Dados para validação do pagamento (PreferenceId ou QrCode)</param>
    /// <returns>Resultado da confirmação do pagamento</returns>
    /// <response code="200">Pagamento confirmado com sucesso - Pedido em preparação</response>
    /// <response code="400">Falha na confirmação - Pagamento não encontrado ou inválido</response>
    /// <response code="404">Pedido não encontrado - Verifique se o ID está correto</response>
    [HttpPost("{id:guid}/confirm-payment")]
    [ProducesResponseType(typeof(ConfirmPaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmPayment([FromRoute] Guid id, [FromBody] ConfirmPaymentDto request)
    {
        _logger.LogInformation("Iniciando confirmação de pagamento do pedido {OrderId}", id);

        // Validação inicial do DTO
        if (!request.IsValid)
        {
            return BadRequest("É obrigatório fornecer o PreferenceId ou o QrCode para validação do pagamento");
        }

        var command = new ConfirmPaymentCommand 
        { 
            OrderId = id,
            PreferenceId = request.PreferenceId,
            QrCode = request.QrCode
        };
        
        var result = await _mediator.Send(command);

        var response = new ConfirmPaymentResponseDto
        {
            OrderId = result.OrderId,
            Status = result.Status,
            TotalAmount = result.TotalAmount,
            ConfirmedAt = result.ConfirmedAt,
            PaymentConfirmed = result.PaymentConfirmed,
            Message = result.PaymentConfirmed 
                ? "Pagamento confirmado com sucesso" 
                : "Falha na confirmação do pagamento"
        };

        if (result.PaymentConfirmed)
        {
            _logger.LogInformation("Pagamento do pedido {OrderId} confirmado com sucesso", id);
            return Ok(response);
        }
        else
        {
            _logger.LogWarning("Falha na confirmação do pagamento do pedido {OrderId}", id);            return BadRequest(response);
        }
    }

    /// <summary>
    /// Retorna uma descrição amigável para o status do pedido
    /// </summary>
    /// <param name="status">Status do pedido</param>
    /// <returns>Descrição amigável do status</returns>
    private static string GetStatusDescription(string status)
    {
        return status?.ToLower() switch
        {
            "pending" => "Pedido criado, aguardando pagamento",
            "awaitingpayment" => "Aguardando confirmação do pagamento",
            "paid" => "Pagamento confirmado",
            "processing" => "Seu pedido está sendo preparado",
            "ready" => "Pedido pronto para retirada",
            "completed" => "Pedido finalizado",
            _ => "Status do pedido"
        };
    }
}
