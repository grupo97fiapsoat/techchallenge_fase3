using FastFood.Domain.Products.Entities;
using FastFood.Domain.Products.Enums;
using FastFood.Domain.Products.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductCommandResult>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<CreateProductCommandResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Criar uma nova entidade de produto
        var product = new Product(
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.Images
        );

        // Persistir no banco de dados
        await _productRepository.CreateAsync(product);

        // Retornar o resultado
        return new CreateProductCommandResult
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            CategoryName = product.Category.ToString(),
            Price = product.Price,
            Images = product.Images,
            CreatedAt = product.CreatedAt
        };
    }
}
