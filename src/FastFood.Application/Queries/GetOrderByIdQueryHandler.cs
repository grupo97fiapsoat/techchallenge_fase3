using FastFood.Application.DTOs;
using FastFood.Domain.Orders.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastFood.Application.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, ILogger<GetOrderByIdQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<GetOrderByIdQueryResult> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.Id);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", request.Id);
                return new GetOrderByIdQueryResult
                {
                    Success = false,
                    Error = $"Pedido com ID {request.Id} não encontrado"
                };
            }            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name, // Null para pedidos anônimos
                Status = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    SubTotal = item.SubTotal
                }).ToList()
            };

            return new GetOrderByIdQueryResult
            {
                Success = true,
                Order = orderDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido com ID {OrderId}", request.Id);
            return new GetOrderByIdQueryResult
            {
                Success = false,
                Error = "Ocorreu um erro ao buscar o pedido. Por favor, tente novamente."
            };
        }
    }
}
