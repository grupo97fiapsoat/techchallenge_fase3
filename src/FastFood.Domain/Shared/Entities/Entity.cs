using System;
using System.Runtime.CompilerServices;

namespace FastFood.Domain.Shared.Entities;

/// <summary>
/// Classe base abstrata para todas as entidades do domínio.
/// </summary>
public abstract class Entity : IEntity
{
    private static readonly TimeZoneInfo BrazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Data de criação da entidade.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Data da última atualização da entidade.
    /// Pode ser nula se a entidade nunca foi atualizada.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Construtor protegido para inicializar uma nova entidade.
    /// </summary>
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = GetBrasilDateTime();
    }

    /// <summary>
    /// Atualiza a data de última modificação da entidade para a data/hora atual no fuso do Brasil.
    /// </summary>
    public void SetUpdatedAt()
    {
        UpdatedAt = GetBrasilDateTime();
    }

    /// <summary>
    /// Obtém a data e hora atuais no fuso horário do Brasil (UTC-3).
    /// </summary>
    /// <returns>Data e hora atuais no fuso do Brasil</returns>
    public static DateTime GetBrasilDateTime()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrazilTimeZone);
    }

    #region Equals e GetHashCode

    /// <summary>
    /// Determina se esta entidade é igual a outra entidade.
    /// A comparação é feita pelo Id da entidade.
    /// </summary>
    /// <param name="obj">Objeto a ser comparado</param>
    /// <returns>true se as entidades são iguais, false caso contrário</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is not Entity entity)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return Id == entity.Id;
    }

    /// <summary>
    /// Obtém o código hash desta entidade.
    /// O código hash é gerado a partir do Id da entidade.
    /// </summary>
    /// <returns>O código hash da entidade</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }

    #endregion
}
