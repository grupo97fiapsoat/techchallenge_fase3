using FastFood.Domain.Products.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastFood.Application.Queries;

/// <summary>
/// Handler para a query de busca de produto por ID.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do handler.
    /// </summary>
    /// <param name="productRepository">Repositório de produtos.</param>
    /// <param name="logger">Serviço de logging.</param>
    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Manipula a query para buscar um produto pelo ID.
    /// </summary>
    /// <param name="request">A query com o ID do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O resultado contendo os dados do produto, se encontrado.</returns>
    public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando produto com ID: {ProductId}", request.Id);

        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            _logger.LogWarning("Produto não encontrado. ID: {ProductId}", request.Id);
            return new GetProductByIdQueryResult
            {
                Success = false,
                Error = "Produto não encontrado"
            };
        }

        _logger.LogInformation("Produto encontrado. ID: {ProductId}, Nome: {ProductName}", product.Id, product.Name);        return new GetProductByIdQueryResult
        {
            Success = true,
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
