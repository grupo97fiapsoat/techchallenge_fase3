using FastFood.Api.Models;
using FastFood.Application.Commands;
using FastFood.Application.Common.Exceptions;
using FastFood.Application.DTOs;
using FastFood.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    /// <param name="request">Dados do pedido</param>
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

        return CreatedAtAction(nameof(GetById), new { id = orderDto.Id }, orderDto);
    }

    /// <summary>
    /// Obtém todos os pedidos com opções de filtragem e paginação
    /// </summary>
    /// <param name="customerId">ID do cliente (opcional)</param>
    /// <param name="status">Status do pedido (opcional)</param>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <returns>Lista de pedidos paginada</returns>
    /// <response code="200">Lista de pedidos</response>
    [HttpGet]
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
        
        return Ok(result);
    }

    /// <summary>
    /// Obtém um pedido pelo ID
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Detalhes do pedido</returns>
    /// <response code="200">Pedido encontrado</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        _logger.LogInformation("Buscando pedido por ID: {OrderId}", id);
        
        var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);        if (!result.Success)
        {
            _logger.LogWarning("Pedido não encontrado. ID: {OrderId}", id);
            var errorMessage = result.Error ?? "Pedido não encontrado";
            return NotFound(new ErrorResponse { Errors = new List<string> { errorMessage } });
        }

        return Ok(result.Order);
    }

    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="request">Novo status</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Status atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpPut("{id:guid}/status")]
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
        }
    }    /// <summary>
    /// Processa o pagamento de um pedido
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Detalhes do processamento do pagamento</returns>
    /// <response code="200">Pagamento processado com sucesso</response>
    /// <response code="400">Dados inválidos ou erro no processamento</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpPost("{id:guid}/checkout")]
    [ProducesResponseType(typeof(CheckoutResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessCheckout([FromRoute] Guid id)
    {
        _logger.LogInformation("Iniciando processamento de checkout do pedido {OrderId}", id);

        var command = new ProcessCheckoutCommand { OrderId = id };
        var result = await _mediator.Send(command);

        var response = new CheckoutResponseDto
        {
            OrderId = result.OrderId,
            QrCode = result.QrCode,
            Status = result.Status,
            TotalAmount = result.TotalAmount,
            ProcessedAt = result.ProcessedAt
        };

        _logger.LogInformation("Checkout do pedido {OrderId} processado com sucesso. QR Code gerado.", id);
        return Ok(response);
    }
}
