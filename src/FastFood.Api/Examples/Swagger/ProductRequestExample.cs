using FastFood.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace FastFood.Api.Examples.Swagger;

/// <summary>
/// Exemplo de request para criação de produto
/// </summary>
public class CreateProductDtoExample : IExamplesProvider<CreateProductDto>
{
    public CreateProductDto GetExamples()
    {
        return new CreateProductDto
        {
            Name = "X-Bacon",
            Description = "Hambúrguer com bacon, queijo, alface, tomate e maionese",
            Category = "Lanche",
            Price = 15.95m,
            ImageUrl = "https://example.com/images/x-bacon.jpg",
            Images = new List<string> { "https://example.com/images/x-bacon.jpg" }
        };
    }
}

/// <summary>
/// Exemplo de response para produto
/// </summary>
public class ProductDtoExample : IExamplesProvider<ProductDto>
{
    public ProductDto GetExamples()
    {
        return new ProductDto
        {
            Id = Guid.Parse("e47ac10b-58cc-4372-a567-0e02b2c3d123"),
            Name = "X-Bacon",
            Description = "Hambúrguer com bacon, queijo, alface, tomate e maionese",
            Category = "Lanche",
            Price = 15.95m,
            ImageUrl = "https://example.com/images/x-bacon.jpg",
            Images = new List<string> { "https://example.com/images/x-bacon.jpg" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
