using FastFood.Domain.Products.Repositories;
using MediatR;

namespace FastFood.Application.Queries;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, GetProductsByCategoryQueryResult>
{
    private readonly IProductRepository _productRepository;

    public GetProductsByCategoryQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductsByCategoryQueryResult> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        // Buscar produtos pela categoria
        var products = await _productRepository.GetByCategoryAsync(
            request.Category,
            request.PageNumber,
            request.PageSize
        );

        // Mapear para o resultado
        var result = new GetProductsByCategoryQueryResult
        {
            Products = products.Select(p => new ProductItem
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                CategoryName = p.Category.ToString(),
                Price = p.Price,
                Images = p.Images,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList()
        };

        return result;
    }
}
