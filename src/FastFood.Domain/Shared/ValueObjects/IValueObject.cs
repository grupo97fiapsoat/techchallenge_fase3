namespace FastFood.Domain.Shared.ValueObjects;

/// <summary>
/// Interface base para todos os value objects do domínio.
/// </summary>
public interface IValueObject
{
    /// <summary>
    /// Compara se este value object é igual a outro.
    /// A comparação é feita pelos valores, não pela referência.
    /// </summary>
    /// <param name="other">O value object a ser comparado.</param>
    /// <returns>True se os value objects são iguais, false caso contrário.</returns>
    bool Equals(object? other);

    /// <summary>
    /// Obtém um código hash para o value object baseado em seus valores.
    /// </summary>
    /// <returns>O código hash do value object.</returns>
    int GetHashCode();
}