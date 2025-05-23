namespace FastFood.Application.DTOs;

/// <summary>
/// DTO com os dados de um produto
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Identificador único do produto
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Descrição detalhada do produto
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Categoria do produto (ex: Lanche, Bebida, Sobremesa)
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    /// Preço do produto
    /// </summary>
    public required decimal Price { get; set; }

    /// <summary>
    /// URL principal da imagem do produto
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URLs das imagens do produto
    /// </summary>
    public required List<string> Images { get; set; }

    /// <summary>
    /// Data e hora de criação do produto
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização do produto
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
