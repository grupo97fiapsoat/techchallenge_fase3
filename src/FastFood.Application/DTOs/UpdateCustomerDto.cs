using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para atualização de dados de um cliente existente
/// </summary>
public class UpdateCustomerDto
{
    /// <summary>
    /// Nome completo do cliente
    /// </summary>
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string? Name { get; set; }

    /// <summary>
    /// Endereço de e-mail do cliente
    /// </summary>
    [EmailAddress(ErrorMessage = "O e-mail informado não é válido")]
    [StringLength(100, ErrorMessage = "O e-mail não pode ter mais de 100 caracteres")]
    public string? Email { get; set; }

    /// <summary>
    /// CPF do cliente (apenas números)
    /// </summary>
    public string? Cpf { get; set; }
}
