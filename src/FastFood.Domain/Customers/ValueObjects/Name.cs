using System.Text.RegularExpressions;
using FastFood.Domain.Customers.Exceptions;

namespace FastFood.Domain.Customers.ValueObjects;

/// <summary>
/// Value Object que representa um nome válido.
/// </summary>
public record Name
{
    private const int MIN_LENGTH = 3;
    private const int MAX_LENGTH = 100;
    private static readonly Regex NAME_REGEX = new(
        @"^[a-zA-ZÀ-ÿ\s'-]+$",
        RegexOptions.Compiled);

    /// <summary>
    /// Obtém o valor do nome.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="Name"/>.
    /// </summary>
    /// <param name="value">O valor do nome.</param>
    private Name(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria uma nova instância de Name após validar o valor fornecido.
    /// </summary>
    /// <param name="name">O valor do nome.</param>
    /// <returns>Uma nova instância de <see cref="Name"/> válido.</returns>
    /// <exception cref="CustomerDomainException">Lançada quando o nome é inválido.</exception>
    public static Name Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CustomerDomainException("O nome não pode ser vazio");

        name = name.Trim();

        if (name.Length < MIN_LENGTH)
            throw new CustomerDomainException($"O nome deve ter no mínimo {MIN_LENGTH} caracteres");

        if (name.Length > MAX_LENGTH)
            throw new CustomerDomainException($"O nome não pode ter mais que {MAX_LENGTH} caracteres");

        if (!NAME_REGEX.IsMatch(name))
            throw new CustomerDomainException("O nome possui caracteres inválidos");

        // Converte primeira letra de cada palavra para maiúscula
        var properName = string.Join(" ",
            name.Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => char.ToUpper(x[0]) + x[1..].ToLower()));

        return new Name(properName);
    }

    /// <summary>
    /// Retorna o nome como string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Permite a conversão implícita de Name para string.
    /// </summary>
    public static implicit operator string(Name name) => name.Value;
}
