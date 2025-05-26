using System.Text.RegularExpressions;
using FastFood.Domain.Customers.Exceptions;
using System.IO;
using System.Linq;

namespace FastFood.Domain.Customers.ValueObjects;

/// <summary>
/// Value Object que representa um CPF válido.
/// </summary>
public sealed record Cpf
{
    private const int CPF_LENGTH = 11;
    private const string CPF_FORMAT = @"^(\d{3}\.?\d{3}\.?\d{3}\-?\d{2}|\d{11})$"; // Aceita CPF com ou sem formatação
    private const string NUMERIC_ONLY_FORMAT = @"[^\d]";
    private static readonly int[] MULTIPLIER1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] MULTIPLIER2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    private static string GetMultipliersString()
    {
        return $"MULTIPLIER1: [{string.Join(", ", MULTIPLIER1)}]\nMULTIPLIER2: [{string.Join(", ", MULTIPLIER2)}]";
    }

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

        // Primeiro normaliza o CPF
        var normalizedCpf = NormalizeCpf(value);

        // Depois valida o formato
        if (!CPF_REGEX.IsMatch(normalizedCpf))
            throw new CustomerDomainException("O CPF deve conter exatamente 11 dígitos");

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
        Console.WriteLine($"[DEBUG] Normalizando CPF: {cpf}");
        
        if (string.IsNullOrWhiteSpace(cpf))
        {
            Console.WriteLine("[DEBUG] CPF vazio ou nulo");
            return string.Empty;
        }

        var normalized = NUMERIC_REGEX.Replace(cpf.Trim(), "");
        Console.WriteLine($"[DEBUG] CPF normalizado: {normalized}");
        
        return normalized;
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
            // Usar um StringBuilder para coletar os logs
            var log = new System.Text.StringBuilder();
            log.AppendLine("\n=============================================");
            log.AppendLine("[DEBUG] ===== INÍCIO DA VALIDAÇÃO DO CPF =====");
            log.AppendLine($"[DEBUG] CPF a ser validado: {cpf}");
            log.AppendLine(GetMultipliersString());
            
            // Validações básicas
            if (string.IsNullOrWhiteSpace(cpf))
            {
                log.AppendLine("[ERRO] CPF não pode ser nulo ou vazio");
                Console.WriteLine(log.ToString());
                return false;
            }

            log.AppendLine($"[DEBUG] Tamanho do CPF: {cpf.Length} (esperado: {CPF_LENGTH})");
            
            if (cpf.Length != CPF_LENGTH)
            {
                log.AppendLine($"[ERRO] CPF inválido - Tamanho incorreto: {cpf.Length} (esperado: {CPF_LENGTH})");
                Console.WriteLine(log.ToString());
                return false;
            }

            // Valida se todos os caracteres são numéricos
            if (!cpf.All(char.IsDigit))
            {
                log.AppendLine("[ERRO] CPF contém caracteres não numéricos");
                Console.WriteLine(log.ToString());
                return false;
            }

            // Valida CPFs com dígitos iguais
            var distinctDigits = cpf.Distinct().Count();
            log.AppendLine($"[DEBUG] Número de dígitos distintos: {distinctDigits}");
            
            if (distinctDigits == 1)
            {
                log.AppendLine("[ERRO] CPF inválido - Todos os dígitos são iguais");
                Console.WriteLine(log.ToString());
                return false;
            }

            // Validação do primeiro dígito
            var tempCpf = cpf.Substring(0, 9);
            var sum = 0;

            log.AppendLine("\n[DEBUG] Validando primeiro dígito verificador...");
            log.AppendLine($"[DEBUG] Dígitos: {string.Join(" ", tempCpf.Select(c => c.ToString()))}");
            log.AppendLine($"[DEBUG] Pesos:    {string.Join(" ", MULTIPLIER1.Select(m => m.ToString().PadLeft(2)))}");
            
            for (int i = 0; i < 9; i++)
            {
                int digit = int.Parse(tempCpf[i].ToString());
                int mult = MULTIPLIER1[i];
                int product = digit * mult;
                sum += product;
                log.AppendLine($"[DEBUG]   {digit} * {mult} = {product.ToString().PadLeft(2)} (soma parcial: {sum})");
            }

            var remainder = sum % 11;
            var firstDigit = remainder < 2 ? 0 : 11 - remainder;
            tempCpf += firstDigit.ToString();
            
            log.AppendLine($"[DEBUG]   Soma total: {sum}, Resto: {remainder}, Primeiro dígito: {firstDigit}");
            log.AppendLine($"[DEBUG]   CPF com primeiro dígito: {tempCpf}");

            // Validação do segundo dígito
            sum = 0;
            log.AppendLine("\n[DEBUG] Validando segundo dígito verificador...");
            log.AppendLine($"[DEBUG] Dígitos: {string.Join(" ", tempCpf.Select(c => c.ToString()))}");
            log.AppendLine($"[DEBUG] Pesos:    {string.Join(" ", MULTIPLIER2.Select(m => m.ToString().PadLeft(2)))}");
            
            for (int i = 0; i < 10; i++)
            {
                int digit = int.Parse(tempCpf[i].ToString());
                int mult = MULTIPLIER2[i];
                int product = digit * mult;
                sum += product;
                log.AppendLine($"[DEBUG]   {digit} * {mult} = {product.ToString().PadLeft(2)} (soma parcial: {sum})");
            }

            remainder = sum % 11;
            var secondDigit = remainder < 2 ? 0 : 11 - remainder;
            tempCpf += secondDigit.ToString();
            
            log.AppendLine($"[DEBUG]   Soma total: {sum}, Resto: {remainder}, Segundo dígito: {secondDigit}");
            log.AppendLine($"[DEBUG]   CPF calculado: {tempCpf}");
            log.AppendLine($"[DEBUG]   CPF original: {cpf}");
            bool isValid = cpf == tempCpf;
            log.AppendLine($"[DEBUG]   CPF válido? {isValid}");
            log.AppendLine("[DEBUG] ===== FIM DA VALIDAÇÃO DO CPF =====");
            log.AppendLine("=============================================\n");
            
            // Escrever todos os logs de uma vez
            Console.WriteLine(log.ToString());
            
            return isValid;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Erro ao validar CPF: {ex.Message}");
            Console.WriteLine($"[ERRO] StackTrace: {ex.StackTrace}");
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
