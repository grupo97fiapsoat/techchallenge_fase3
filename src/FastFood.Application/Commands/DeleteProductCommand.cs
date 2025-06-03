using MediatR;

namespace FastFood.Application.Commands;

public class DeleteProductCommand : IRequest
{
    public Guid Id { get; set; }
}
