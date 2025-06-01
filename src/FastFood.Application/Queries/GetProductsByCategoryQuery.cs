using FastFood.Domain.Products.Enums;
using MediatR;

namespace FastFood.Application.Queries;

public class GetProductsByCategoryQuery : IRequest<GetProductsByCategoryQueryResult>
{
    public ProductCategory Category { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}

public class GetProductsByCategoryQueryResult
{
    public List<ProductItem> Products { get; set; } = new List<ProductItem>();
}

public class ProductItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductCategory Category { get; set; }
    public string CategoryName { get; set; }
    public decimal Price { get; set; }
    public List<string> Images { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
