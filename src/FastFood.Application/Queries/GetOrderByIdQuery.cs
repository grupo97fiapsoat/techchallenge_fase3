using FastFood.Application.DTOs;
using MediatR;

namespace FastFood.Application.Queries;

public class GetOrderByIdQuery : IRequest<GetOrderByIdQueryResult>
{
    public Guid Id { get; set; }
}

public class GetOrderByIdQueryResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public OrderDto? Order { get; set; }
}
