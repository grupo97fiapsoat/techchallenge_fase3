using MediatR;

namespace FastFood.Application.Commands;

public class CreateProductCommand : IRequest<CreateProductCommandResult>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Images { get; set; }
}

public class CreateProductCommandResult
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public required List<string> Images { get; set; }
    public required DateTime CreatedAt { get; set; }
}
