using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Command para atualizar um cliente existente
/// </summary>
public class UpdateCustomerCommand : IRequest<UpdateCustomerCommandResult>
{
    /// <summary>
    /// Identificador único do cliente a ser atualizado
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Novo nome do cliente (opcional)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Novo email do cliente (opcional)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Novo CPF do cliente (opcional)
    /// </summary>
    public string? Cpf { get; set; }
}

/// <summary>
/// Resultado do command de atualização de cliente
/// </summary>
public class UpdateCustomerCommandResult
{
    /// <summary>
    /// Identificador único do cliente
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome atualizado do cliente
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email atualizado do cliente
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// CPF atualizado do cliente
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de criação do registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
