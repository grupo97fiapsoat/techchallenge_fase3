using MediatR;

namespace FastFood.Application.Commands;

public class CreateOrderCommand : IRequest<CreateOrderCommandResult>
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemCommand> Items { get; set; }
}

public class CreateOrderItemCommand
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderCommandResult
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public List<CreateOrderItemCommandResult> Items { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderItemCommandResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
}
