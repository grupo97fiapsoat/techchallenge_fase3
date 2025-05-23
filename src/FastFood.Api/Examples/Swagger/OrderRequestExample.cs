using FastFood.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace FastFood.Api.Examples.Swagger;

/// <summary>
/// Exemplo de request para criação de pedido
/// </summary>
public class CreateOrderDtoExample : IExamplesProvider<CreateOrderDto>
{
    public CreateOrderDto GetExamples()
    {
        return new CreateOrderDto
        {
            CustomerId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.Parse("e47ac10b-58cc-4372-a567-0e02b2c3d123"),
                    Quantity = 2,
                    Observation = "Sem cebola"
                },
                new()
                {
                    ProductId = Guid.Parse("d47ac10b-58cc-4372-a567-0e02b2c3d456"),
                    Quantity = 1,
                    Observation = null
                }
            }
        };
    }
}

/// <summary>
/// Exemplo de response para pedido
/// </summary>
public class OrderDtoExample : IExamplesProvider<OrderDto>
{
    public OrderDto GetExamples()
    {
        return new OrderDto
        {
            Id = Guid.Parse("a47ac10b-58cc-4372-a567-0e02b2c3d789"),
            CustomerId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            Status = "Pending",
            TotalPrice = 45.90m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<OrderItemDto>
            {
                new()
                {
                    Id = Guid.Parse("b47ac10b-58cc-4372-a567-0e02b2c3d111"),
                    ProductId = Guid.Parse("e47ac10b-58cc-4372-a567-0e02b2c3d123"),
                    ProductName = "X-Bacon",
                    Quantity = 2,
                    UnitPrice = 15.95m,
                    SubTotal = 31.90m,
                    Observation = "Sem cebola"
                },
                new()
                {
                    Id = Guid.Parse("c47ac10b-58cc-4372-a567-0e02b2c3d222"),
                    ProductId = Guid.Parse("d47ac10b-58cc-4372-a567-0e02b2c3d456"),
                    ProductName = "Coca-Cola 350ml",
                    Quantity = 1,
                    UnitPrice = 14.00m,
                    SubTotal = 14.00m,
                    Observation = null
                }
            }
        };
    }
}
