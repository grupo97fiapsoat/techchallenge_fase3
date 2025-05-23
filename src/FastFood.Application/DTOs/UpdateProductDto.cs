using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para atualização de um produto existente
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// Novo nome do produto
    /// </summary>
    /// <example>X-Bacon Duplo</example>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public required string Name { get; set; }

    /// <summary>
    /// Nova descrição do produto
    /// </summary>
    /// <example>Hambúrguer duplo com bacon, queijo, alface, tomate e maionese</example>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres")]
    public required string Description { get; set; }

    /// <summary>
    /// Nova categoria do produto
    /// </summary>
    /// <example>Lanche</example>
    [Required(ErrorMessage = "A categoria é obrigatória")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "A categoria deve ter entre 3 e 50 caracteres")]
    public required string Category { get; set; }

    /// <summary>
    /// Novo preço do produto (maior que zero)
    /// </summary>
    /// <example>19.90</example>
    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public required decimal Price { get; set; }

    /// <summary>
    /// Nova URL principal da imagem do produto
    /// </summary>
    /// <example>https://example.com/images/x-bacon-duplo.jpg</example>
    [Url(ErrorMessage = "O URL da imagem não é válido")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Novas URLs das imagens do produto
    /// </summary>
    /// <example>["https://example.com/images/x-bacon-duplo.jpg"]</example>
    public List<string>? Images { get; set; }
}
