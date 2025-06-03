namespace FastFood.Domain.Shared.Entities;

/// <summary>
/// Interface base para todas as entidades do domínio.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Data da última atualização da entidade.
    /// </summary>
    DateTime? UpdatedAt { get; }

    /// <summary>
    /// Atualiza a data de última modificação da entidade.
    /// </summary>
    void SetUpdatedAt();
}