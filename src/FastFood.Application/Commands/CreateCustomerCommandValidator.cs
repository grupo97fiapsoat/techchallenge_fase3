using FluentValidation;

namespace FastFood.Application.Commands;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório")
            .EmailAddress().WithMessage("O e-mail informado não é válido")
            .MaximumLength(100).WithMessage("O e-mail não pode ter mais de 100 caracteres");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF é obrigatório")
            .Length(11).WithMessage("O CPF deve ter 11 dígitos");
    }
}
