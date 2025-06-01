using MediatR;

namespace FastFood.Application.Queries;

public class GetOrdersQuery : IRequest<GetOrdersQueryResult>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? CustomerId { get; set; }
    public string? Status { get; set; }
}

public class GetOrdersQueryResult
{
    public List<OrderItem> Orders { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ItemsCount { get; set; }
}
