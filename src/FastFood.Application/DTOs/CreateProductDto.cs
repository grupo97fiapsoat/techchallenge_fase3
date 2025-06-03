using FastFood.Domain.Products.Enums;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para criação de um novo produto
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// Nome do produto
    /// </summary>
    /// <example>X-Bacon</example>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public required string Name { get; set; }

    /// <summary>
    /// Descrição detalhada do produto
    /// </summary>
    /// <example>Hambúrguer com bacon, queijo, alface, tomate e maionese</example>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres")]
    public required string Description { get; set; }

    /// <summary>
    /// Categoria do produto (Lanche, Acompanhamento, Bebida, Sobremesa)
    /// </summary>
    /// <example>Lanche</example>
    [Required(ErrorMessage = "A categoria é obrigatória")]
    [EnumDataType(typeof(ProductCategory), ErrorMessage = "Categoria inválida. Valores permitidos: Lanche, Acompanhamento, Bebida, Sobremesa")]
    public required ProductCategory Category { get; set; }    /// <summary>
    /// Preço do produto (maior que zero)
    /// </summary>
    /// <example>15.90</example>
    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public required decimal Price { get; set; }

    /// <summary>
    /// URL principal da imagem do produto
    /// </summary>
    /// <example>https://example.com/images/x-bacon.jpg</example>
    [Url(ErrorMessage = "O URL da imagem não é válido")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URLs das imagens do produto
    /// </summary>
    /// <example>["https://example.com/images/x-bacon.jpg"]</example>
    public List<string>? Images { get; set; }
}
