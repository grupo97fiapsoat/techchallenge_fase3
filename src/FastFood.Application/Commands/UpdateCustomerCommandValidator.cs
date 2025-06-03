using FastFood.Domain.Customers.Repositories;
using FluentValidation;
using System.Text.RegularExpressions;

namespace FastFood.Application.Commands;

/// <summary>
/// Validador para o command de atualização de cliente
/// </summary>
public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do cliente é obrigatório");

        When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MinimumLength(3)
                .WithMessage("O nome deve ter pelo menos 3 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome não pode ter mais de 100 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("O e-mail informado não é válido")
                .MaximumLength(100)
                .WithMessage("O e-mail não pode ter mais de 100 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Cpf), () =>
        {
            RuleFor(x => x.Cpf)
                .Must(BeAValidCpf)
                .WithMessage("O CPF informado não é válido")
                .MustAsync(BeUniqueCpf)
                .WithMessage("Este CPF já está cadastrado para outro cliente");
        });
    }

    private bool BeAValidCpf(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = Regex.Replace(cpf, "[^0-9]", "");

        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        bool allDigitsEqual = true;
        for (int i = 1; i < cpf.Length; i++)
        {
            if (cpf[i] != cpf[0])
            {
                allDigitsEqual = false;
                break;
            }
        }

        if (allDigitsEqual)
            return false;

        // Cálculo do primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        int remainder = sum % 11;
        int firstVerifier = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != firstVerifier)
            return false;

        // Cálculo do segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        int secondVerifier = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == secondVerifier;
    }

    private async Task<bool> BeUniqueCpf(UpdateCustomerCommand command, string cpf, CancellationToken cancellationToken)
    {
        // Remove caracteres não numéricos
        cpf = Regex.Replace(cpf, "[^0-9]", "");

        var existingCustomer = await _customerRepository.GetByCpfAsync(cpf, cancellationToken);
        
        // CPF é único ou pertence ao próprio cliente
        return existingCustomer == null || existingCustomer.Id == command.Id;
    }
}
