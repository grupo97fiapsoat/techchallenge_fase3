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
    }    /// <summary>
    /// Cria um novo pedido no sistema
    /// 
    /// **Endpoint público** - Não requer autenticação (qualquer pessoa pode fazer pedidos).
    /// 
    /// **Como usar:**
    /// 1. Selecione os produtos do menu usando os endpoints de produtos
    /// 2. Envie uma lista de itens com ProductId e Quantity
    /// 3. O sistema calculará automaticamente o valor total
    /// 4. O pedido será criado com status "Recebido"
    /// 5. Use o ID retornado para processar o checkout e pagamento
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "customerId": "550e8400-e29b-41d4-a716-446655440000",
    ///   "items": [
    ///     {
    ///       "productId": "123e4567-e89b-12d3-a456-426614174000",
    ///       "quantity": 2
    ///     },
    ///     {
    ///       "productId": "987fcdeb-51a2-43d1-b789-123456789abc",
    ///       "quantity": 1
    ///     }
    ///   ]
    /// }
    /// ```
    ///    /// **Próximos passos após criar o pedido:**
    /// 1. Use `POST /{id}/checkout` para gerar QR Code de pagamento
    /// 2. Use `POST /{id}/confirm-payment` para confirmar o pagamento
    /// 3. Acompanhe o status com `GET /{id}`
    /// </summary>
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

    /// <summary>
    /// Lista todos os pedidos com opções de filtragem e paginação
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT (acesso administrativo).
    /// 
    /// **Funcionalidades:**
    /// - Lista todos os pedidos do sistema
    /// - Filtragem por cliente e status
    /// - Paginação para performance
    /// - Ordenação por data de criação
    /// 
    /// **Como usar:**
    /// 1. **Autenticação necessária**: Inclua o token JWT no header Authorization
    /// 2. **Filtros opcionais**: customerId, status para refinar a busca
    /// 3. **Paginação**: pageNumber e pageSize para controlar a quantidade de resultados
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /api/v1/orders?status=EmPreparacao&amp;pageNumber=1&amp;pageSize=10
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// ```
    /// 
    /// **Status válidos para filtro:**
    /// - `Recebido` - Pedidos aguardando pagamento
    /// - `EmPreparacao` - Pedidos sendo preparados
    /// - `Pronto` - Pedidos prontos para retirada
    /// - `Finalizado` - Pedidos entregues
    /// </summary>
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

    /// <summary>
    /// Obtém detalhes de um pedido específico pelo ID
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT.
    /// 
    /// **Funcionalidades:**
    /// - Exibe todos os detalhes do pedido
    /// - Inclui informações do cliente e itens
    /// - Mostra status atual e histórico
    /// - Calcula valores totais e subtotais
    /// 
    /// **Como usar:**
    /// 1. **Autenticação**: Inclua o token JWT no header Authorization
    /// 2. **ID do pedido**: Forneça o GUID do pedido na URL
    /// 3. **Resposta**: Retorna objeto completo com todos os detalhes
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /api/v1/orders/550e8400-e29b-41d4-a716-446655440000
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// ```
    /// 
    /// **Informações retornadas:**
    /// - Dados do pedido (ID, status, valor total, data de criação)
    /// - Informações do cliente (ID, nome)
    /// - Lista detalhada de itens (produto, quantidade, preços)
    /// - Timestamps de criação e atualizações
    /// </summary>
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
    }    /// <summary>
    /// Atualiza o status de um pedido específico
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT (acesso administrativo).
    /// 
    /// **Finalidade:** Permite que administradores atualizem o status dos pedidos conforme eles progridem no fluxo de preparação e entrega.
    ///    /// **Como usar:**
    /// 1. **Autenticação**: Inclua o token JWT de administrador no header Authorization
    /// 2. **ID do pedido**: Forneça o GUID do pedido na URL
    /// 3. **Status**: Envie o novo status no body da requisição
    /// 4. **Confirmação**: Receba confirmação da atualização
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// PUT /api/v1/orders/550e8400-e29b-41d4-a716-446655440000/status
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// Content-Type: application/json
    /// 
    /// {
    ///   "status": "EmPreparacao"
    /// }
    /// ```
    /// 
    /// **Status válidos:**
    /// - `Recebido` - Pedido criado, aguardando pagamento
    /// - `EmPreparacao` - Pedido pago, sendo preparado na cozinha
    /// - `Pronto` - Pedido pronto para retirada pelo cliente
    /// - `Finalizado` - Pedido entregue ao cliente
    /// 
    /// **Fluxo típico de status:**
    /// Recebido → EmPreparacao → Pronto → Finalizado
    /// </summary>
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
        }    }    /// <summary>
    /// Consulta o status de um pedido de forma pública (para acompanhamento de clientes anônimos)
    /// 
    /// **Endpoint público** - Não requer autenticação (permite acompanhamento de pedidos anônimos).
    /// 
    /// **Finalidade:** Permite que qualquer cliente (incluindo anônimos) consulte o status atual de um pedido usando apenas o OrderId.
    /// 
    /// **Como usar:**
    /// 1. **ID do pedido**: Use o ID retornado ao criar o pedido
    /// 2. **Sem autenticação**: Endpoint público, não precisa de token JWT
    /// 3. **Acompanhamento**: Consulte periodicamente para ver progresso
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /api/v1/orders/550e8400-e29b-41d4-a716-446655440000/status
    /// ```
    /// 
    /// **Resposta exemplo:**
    /// ```json
    /// {
    ///   "orderId": "550e8400-e29b-41d4-a716-446655440000",
    ///   "status": "EmPreparacao",
    ///   "statusDescription": "Seu pedido está sendo preparado",
    ///   "totalPrice": 45.50,
    ///   "createdAt": "2025-06-02T10:30:00Z",
    ///   "isAnonymous": true
    /// }
    /// ```
    /// 
    /// **Status possíveis:**
    /// - `Pending` - Pedido criado, aguardando pagamento
    /// - `AwaitingPayment` - Aguardando confirmação do pagamento
    /// - `Paid` - Pagamento confirmado
    /// - `Processing` - Sendo preparado na cozinha
    /// - `Ready` - Pronto para retirada
    /// - `Completed` - Entregue ao cliente
    /// 
    /// **Privacidade:** Este endpoint retorna apenas informações básicas do pedido, sem dados pessoais do cliente.
    /// </summary>
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

    /// <summary>
    /// Gera QR Code para pagamento do pedido via MercadoPago
    /// 
    /// **Endpoint público** - Não requer autenticação (facilita o fluxo de pagamento).
    /// 
    /// **Finalidade:** Gera QR Code e preferência do MercadoPago para que o cliente possa efetuar o pagamento do pedido.
    ///    /// **Como usar:**
    /// 1. **ID do pedido**: Use o ID retornado ao criar o pedido
    /// 2. **QR Code**: Escaneie o QR Code gerado com app do MercadoPago
    /// 3. **Próximo passo**: Use o endpoint de confirmação de pagamento
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// POST /api/v1/orders/550e8400-e29b-41d4-a716-446655440000/checkout
    /// Content-Type: application/json
    /// ```
    /// 
    /// **Resposta exemplo:**
    /// ```json
    /// {
    ///   "orderId": "550e8400-e29b-41d4-a716-446655440000",
    ///   "qrCode": "00020126580014BR.GOV.BCB.PIX...",
    ///   "preferenceId": "1234567890",
    ///   "totalAmount": 45.50,
    ///   "processedAt": "2025-06-02T10:30:00Z"
    /// }
    /// ```
    /// 
    /// **Fluxo de pagamento:**
    /// 1. Cliente cria pedido → 2. **Gera QR Code (este endpoint)** → 3. Cliente paga → 4. Confirma pagamento
    /// 
    /// **Integração MercadoPago:**
    /// - QR Code no formato PIX para pagamento instantâneo
    /// - PreferenceId para rastreamento do pagamento
    /// - Webhook automático para notificações
    /// </summary>
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
      /// <summary>
    /// Confirma o pagamento de um pedido após o cliente efetuar o pagamento
    /// 
    /// **Endpoint público** - Não requer autenticação (facilita confirmação automática).
    /// 
    /// **Finalidade:** Valida e confirma o pagamento do pedido, atualizando o status para "EmPreparacao" quando bem-sucedido.
    ///    /// **Como usar:**
    /// 1. **ID do pedido**: Use o mesmo ID do checkout
    /// 2. **Dados de validação**: Forneça PreferenceId OU QrCode
    /// 3. **Confirmação**: Sistema valida com MercadoPago e atualiza status
    /// 4. **Próximo passo**: Pedido entra na fila de preparação
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// POST /api/v1/orders/550e8400-e29b-41d4-a716-446655440000/confirm-payment
    /// Content-Type: application/json
    /// 
    /// {
    ///   "preferenceId": "1234567890",
    ///   "qrCode": "00020126580014BR.GOV.BCB.PIX..."
    /// }
    /// ```
    /// 
    /// **Resposta exemplo (sucesso):**
    /// ```json
    /// {
    ///   "orderId": "550e8400-e29b-41d4-a716-446655440000",
    ///   "status": "EmPreparacao",
    ///   "totalAmount": 45.50,
    ///   "confirmedAt": "2025-06-02T10:35:00Z",
    ///   "paymentConfirmed": true,
    ///   "message": "Pagamento confirmado com sucesso"
    /// }
    /// ```
    /// 
    /// **Fluxo após confirmação:**
    /// 1. Status atualizado para "EmPreparacao"
    /// 2. Notificação enviada para cozinha
    /// 3. Cliente pode acompanhar preparo
    /// 
    /// **Validação obrigatória:**
    /// - Pelo menos um dos campos é obrigatório: `preferenceId` ou `qrCode`
    /// - Sistema valida pagamento com MercadoPago automaticamente
    /// </summary>
    /// <param name="id">ID único do pedido para confirmar pagamento (formato GUID)</param>
    /// <param name="request">Dados para validação do pagamento (PreferenceId ou QrCode)</param>
    /// <returns>Resultado da confirmação do pagamento</returns>    /// <response code="200">Pagamento confirmado com sucesso - Pedido em preparação</response>
    /// <response code="400">Falha na confirmação - Pagamento não encontrado ou inválido</response>
    /// <response code="404">Pedido não encontrado - Verifique se o ID está correto</response>[HttpPost("{id:guid}/confirm-payment")]
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
