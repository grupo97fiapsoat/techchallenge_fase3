using FastFood.Domain.Products.Enums; 
 
namespace FastFood.Application.DTOs; 
 
/// <summary> 
/// DTO com os dados de um produto 
/// </summary> 
public class ProductDto 
{ 
    /// <summary> 
    /// Identificador �nico do produto 
    /// </summary> 
    public required Guid Id { get; set; } 
 
    /// <summary> 
    /// Nome do produto 
    /// </summary> 
    public required string Name { get; set; } 
 
    /// <summary> 
    /// Descri��o detalhada do produto 
    /// </summary> 
    public required string Description { get; set; } 
 
    /// <summary> 
    /// Categoria do produto (enum) 
    /// </summary> 
    public required ProductCategory Category { get; set; } 
 
    /// <summary> 
    /// Nome da categoria do produto (representa��o em string) 
    /// </summary> 
    public required string CategoryName { get; set; } 
 
    /// <summary> 
    /// Pre�o do produto 
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
    /// Data e hora de cria��o do produto 
    /// </summary> 
    public required DateTime CreatedAt { get; set; } 
 
    /// <summary> 
    /// Data e hora da �ltima atualiza��o do produto 
    /// </summary> 
    public DateTime? UpdatedAt { get; set; } 
} 
