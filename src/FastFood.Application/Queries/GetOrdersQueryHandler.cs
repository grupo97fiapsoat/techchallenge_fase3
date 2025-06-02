using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastFood.Application.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersQueryResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrdersQueryHandler> _logger;

    public GetOrdersQueryHandler(IOrderRepository orderRepository, ILogger<GetOrdersQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<GetOrdersQueryResult> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Parse status if provided
            OrderStatus? status = null;
            if (!string.IsNullOrWhiteSpace(request.Status) && 
                Enum.TryParse<OrderStatus>(request.Status, true, out var parsedStatus))
            {
                status = parsedStatus;
            }

            // Ensure valid pagination values
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            // Get orders with pagination and filtering
            var (orders, totalCount) = await _orderRepository.GetOrdersAsync(
                request.PageNumber, 
                request.PageSize,
                request.CustomerId,
                status);            // Map to DTO
            var orderItems = orders.Select(o => new OrderItem
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name, // Null para pedidos anônimos
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                ItemsCount = o.Items.Count
            }).ToList();

            // Build and return result
            return new GetOrdersQueryResult
            {
                Orders = orderItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter lista de pedidos");
            // Return empty result to avoid breaking the API
            return new GetOrdersQueryResult
            {
                Orders = new List<OrderItem>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            };
        }
    }
}
