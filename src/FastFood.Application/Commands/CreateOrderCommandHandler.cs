using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Customers.Repositories;
using FastFood.Domain.Orders.Entities;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.ValueObjects;
using FastFood.Domain.Products.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }    public async Task<CreateOrderCommandResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Verificar se o cliente existe (apenas se CustomerId foi informado)
        Customer? customer = null;
        if (request.CustomerId.HasValue)
        {
            customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
            if (customer == null)
                throw new NotFoundException($"Cliente com ID {request.CustomerId.Value} não encontrado");
        }

        // Criar os itens do pedido
        var orderItems = new List<OrderItem>();
        
        foreach (var item in request.Items)
        {
            // Verificar se o produto existe
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new NotFoundException($"Produto com ID {item.ProductId} não encontrado");

            // Criar o item do pedido
            var orderItem = OrderItem.Create(
                item.ProductId,
                product.Name,
                product.Price,
                item.Quantity
            );

            orderItems.Add(orderItem);
        }

        // Criar o pedido (pode ser anônimo)
        var order = Order.Create(request.CustomerId, orderItems);

        // Persistir o pedido
        await _orderRepository.CreateAsync(order);

        // Retornar o resultado
        return new CreateOrderCommandResult
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = customer?.Name, // Null para pedidos anônimos
            Status = order.Status.ToString(),
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(item => new CreateOrderItemCommandResult
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                SubTotal = item.SubTotal
            }).ToList()
        };
    }
}
