using System.Text.RegularExpressions;
using FastFood.Domain.Customers.Exceptions;

namespace FastFood.Domain.Customers.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de e-mail válido.
/// </summary>
public record Email
{
    private const int MAX_LENGTH = 254; // RFC 5321
    private static readonly Regex EMAIL_REGEX = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Obtém o valor do e-mail.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="Email"/>.
    /// </summary>
    /// <param name="value">O valor do e-mail.</param>
    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria uma nova instância de Email após validar o valor fornecido.
    /// </summary>
    /// <param name="email">O valor do e-mail.</param>
    /// <returns>Uma nova instância de <see cref="Email"/> válido.</returns>
    /// <exception cref="CustomerDomainException">Lançada quando o e-mail é inválido.</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new CustomerDomainException("O e-mail não pode ser vazio");

        email = email.Trim();

        if (email.Length > MAX_LENGTH)
            throw new CustomerDomainException($"O e-mail não pode ter mais que {MAX_LENGTH} caracteres");

        if (!EMAIL_REGEX.IsMatch(email))
            throw new CustomerDomainException("O e-mail possui formato inválido");

        return new Email(email.ToLowerInvariant());
    }

    /// <summary>
    /// Retorna o e-mail como string.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Permite a conversão implícita de Email para string.
    /// </summary>
    public static implicit operator string(Email email) => email.Value;
}
