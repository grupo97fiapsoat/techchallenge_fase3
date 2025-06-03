using System.Collections.Concurrent;
using System.Reflection;

namespace FastFood.Domain.Shared.ValueObjects;

/// <summary>
/// Classe base abstrata para todos os value objects do domínio.
/// </summary>
public abstract class ValueObject : IValueObject
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> TypeProperties = new();

    /// <summary>
    /// Comparação de value objects baseada em seus valores.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null) 
            return false;

        if (GetType() != obj.GetType())
            return false;

        return GetAtomicValues().SequenceEqual(((ValueObject)obj).GetAtomicValues());
    }

    /// <summary>
    /// Obtém um código hash baseado nos valores do value object.
    /// </summary>
    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !(left == right);

    /// <summary>
    /// Obtém os valores atômicos que compõem o value object.
    /// </summary>
    protected virtual IEnumerable<object?> GetAtomicValues()
    {
        return GetProperties(GetType())
            .Select(p => p.GetValue(this));
    }

    /// <summary>
    /// Obtém as propriedades do tipo usando cache.
    /// </summary>
    private static IReadOnlyCollection<PropertyInfo> GetProperties(Type type)
    {
        return TypeProperties.GetOrAdd(type, t => 
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .OrderBy(p => p.Name)
            .ToList()
            .AsReadOnly());
    }
}