using FluentValidation;

namespace FastFood.Application.Queries;

public class GetCustomerByCpfQueryValidator : AbstractValidator<GetCustomerByCpfQuery>
{
    public GetCustomerByCpfQueryValidator()
    {
        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF é obrigatório")
            .Length(11).WithMessage("O CPF deve ter 11 dígitos");
    }
}
