namespace FastFood.Application.DTOs;

/// <summary>
/// DTO com os dados de um cliente
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Identificador único do cliente
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do cliente
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Endereço de e-mail do cliente
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// CPF do cliente (apenas números)
    /// </summary>
    public string Cpf { get; set; }

    /// <summary>
    /// Data e hora de criação do registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
