using System.Text.RegularExpressions;
using FastFood.Domain.Customers.Exceptions;

namespace FastFood.Domain.Customers.ValueObjects;

/// <summary>
/// Value Object que representa um CPF válido.
/// </summary>
public sealed record Cpf
{
    private const int CPF_LENGTH = 11;
    private const string CPF_FORMAT = @"^\d{11}$"; // Regex para CPF normalizado
    private const string NUMERIC_ONLY_FORMAT = @"[^\d]";
    private static readonly int[] MULTIPLIER1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] MULTIPLIER2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly Regex CPF_REGEX = new(CPF_FORMAT, RegexOptions.Compiled);
    private static readonly Regex NUMERIC_REGEX = new(NUMERIC_ONLY_FORMAT, RegexOptions.Compiled);

    /// <summary>
    /// Obtém o valor do CPF.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="Cpf"/>.
    /// </summary>
    /// <param name="value">O valor do CPF.</param>
    /// <exception cref="ArgumentNullException">Lançada quando o valor é nulo.</exception>
    /// <exception cref="CustomerDomainException">Lançada quando o CPF é inválido.</exception>
    private Cpf(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if (!CPF_REGEX.IsMatch(value))
            throw new CustomerDomainException("O CPF deve conter exatamente 11 dígitos");

        if (!IsCpfValid(value))
            throw new CustomerDomainException("O CPF informado não é válido");

        Value = value;
    }

    /// <summary>
    /// Cria uma nova instância de CPF após validar o valor fornecido.
    /// </summary>
    /// <param name="cpf">O valor do CPF.</param>
    /// <returns>Uma nova instância de <see cref="Cpf"/> válido.</returns>
    /// <exception cref="CustomerDomainException">Lançada quando o CPF é inválido.</exception>
    public static Cpf Create(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new CustomerDomainException("O CPF não pode ser vazio");

        // Normaliza o CPF primeiro
        var normalizedCpf = NormalizeCpf(cpf);

        if (string.IsNullOrEmpty(normalizedCpf))
            throw new CustomerDomainException("O CPF deve conter apenas dígitos");

        // Validação do CPF normalizado
        return new Cpf(normalizedCpf);
    }

    /// <summary>
    /// Normaliza o CPF removendo pontuação e espaços.
    /// </summary>
    /// <param name="cpf">O CPF a ser normalizado.</param>
    /// <returns>O CPF apenas com dígitos.</returns>
    private static string NormalizeCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return string.Empty;

        return NUMERIC_REGEX.Replace(cpf.Trim(), "");
    }

    /// <summary>
    /// Valida se o CPF é válido através do cálculo dos dígitos verificadores.
    /// </summary>
    /// <param name="cpf">O CPF a ser validado.</param>
    /// <returns>True se o CPF é válido, False caso contrário.</returns>
    private static bool IsCpfValid(string cpf)
    {
        // Validações básicas
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != CPF_LENGTH)
            return false;

        // Valida se todos os caracteres são numéricos
        if (!cpf.All(char.IsDigit))
            return false;

        // Valida CPFs com dígitos iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Validação do primeiro dígito
        var tempCpf = cpf.Substring(0, 9);
        var sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * MULTIPLIER1[i];

        var remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;

        tempCpf += remainder.ToString();

        // Validação do segundo dígito
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * MULTIPLIER2[i];

        remainder = sum % 11;
        remainder = remainder < 2 ? 0 : 11 - remainder;

        tempCpf += remainder.ToString();

        return cpf == tempCpf;
    }

    /// <summary>
    /// Retorna o CPF formatado no padrão XXX.XXX.XXX-XX.
    /// </summary>
    public override string ToString()
    {
        if (Value.Length != CPF_LENGTH)
            return Value;

        return $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9, 2)}";
    }

    /// <summary>
    /// Permite a conversão implícita de Cpf para string.
    /// </summary>
    public static implicit operator string(Cpf cpf) => cpf.Value;
}
