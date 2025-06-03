using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Products.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Buscar o produto pelo ID
        var product = await _productRepository.GetByIdAsync(request.Id);

        // Verificar se o produto existe
        if (product == null)
        {
            throw new NotFoundException($"Produto com ID {request.Id} não encontrado");
        }

        // Excluir o produto do banco de dados
        await _productRepository.DeleteAsync(product);
    }
}
