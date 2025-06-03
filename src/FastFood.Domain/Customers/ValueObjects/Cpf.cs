using System.Text.RegularExpressions;
using FastFood.Domain.Customers.Exceptions;
using System.Linq;

namespace FastFood.Domain.Customers.ValueObjects;

/// <summary>
/// Value Object que representa um CPF válido.
/// </summary>
public sealed record Cpf
{    private const int CPF_LENGTH = 11;
    private const string CPF_FORMAT = @"^[\d\.\-\s]{11,14}$"; // Aceita 11-14 caracteres com dígitos, pontos, hífen e espaços
    private const string NUMERIC_ONLY_FORMAT = @"[^\d]";
    private static readonly int[] MULTIPLIER1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] MULTIPLIER2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

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
    /// <exception cref="ArgumentNullException">Lançada quando o valor é nulo.</exception>    /// <exception cref="CustomerDomainException">Lançada quando o CPF é inválido.</exception>
    private Cpf(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        // Primeiro valida o formato ANTES da normalização (aceita com ou sem formatação)
        if (!CPF_REGEX.IsMatch(value.Trim()))
            throw new CustomerDomainException("O CPF deve estar no formato correto (11 dígitos ou XXX.XXX.XXX-XX)");

        // Depois normaliza o CPF
        var normalizedCpf = NormalizeCpf(value);

        // Por fim, valida os dígitos verificadores
        if (!IsCpfValid(normalizedCpf))
            throw new CustomerDomainException("O CPF informado não é válido");

        // Armazena o CPF normalizado (apenas dígitos)
        Value = normalizedCpf;
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

        // Deixa o construtor fazer toda a validação e normalização
        return new Cpf(cpf);
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
        try
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            if (cpf.Length != CPF_LENGTH)
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
            {
                sum += int.Parse(tempCpf[i].ToString()) * MULTIPLIER1[i];
            }

            var remainder = sum % 11;
            var firstDigit = remainder < 2 ? 0 : 11 - remainder;
            tempCpf += firstDigit.ToString();

            // Validação do segundo dígito
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(tempCpf[i].ToString()) * MULTIPLIER2[i];
            }

            remainder = sum % 11;
            var secondDigit = remainder < 2 ? 0 : 11 - remainder;
            tempCpf += secondDigit.ToString();

            return cpf == tempCpf;
        }
        catch
        {
            return false;
        }
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
