using FastFood.Domain.Products.Enums;

namespace FastFood.Application.Queries;

/// <summary>
/// Resultado da query para buscar um produto pelo ID.
/// </summary>
public class GetProductByIdQueryResult
{
    /// <summary>
    /// Indica se o produto foi encontrado.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de erro, caso ocorra algum problema.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// ID do produto.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do produto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do produto.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Categoria do produto (enum).
    /// </summary>
    public ProductCategory Category { get; set; }
    
    /// <summary>
    /// Nome da categoria do produto (representação em string).
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Preço do produto.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// URL da imagem principal do produto (legado).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URLs das imagens do produto.
    /// </summary>
    public List<string> Images { get; set; } = new List<string>();

    /// <summary>
    /// Data de criação do produto.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização do produto.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
