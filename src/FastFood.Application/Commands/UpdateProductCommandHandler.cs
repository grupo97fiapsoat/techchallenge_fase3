using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Products.Enums;
using FastFood.Domain.Products.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductCommandResult>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<UpdateProductCommandResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Buscar o produto pelo ID
        var product = await _productRepository.GetByIdAsync(request.Id);

        // Verificar se o produto existe
        if (product == null)
        {
            throw new NotFoundException($"Produto com ID {request.Id} não encontrado");
        }

        // Atualizar os dados do produto
        product.Update(
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.Images
        );

        // Persistir as alterações no banco de dados
        await _productRepository.UpdateAsync(product);

        // Retornar o resultado
        return new UpdateProductCommandResult
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            CategoryName = product.Category.ToString(),
            Price = product.Price,
            Images = product.Images,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
