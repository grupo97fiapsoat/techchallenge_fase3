using FluentValidation;

namespace FastFood.Application.Queries;

/// <summary>
/// Validador para a query de listagem de clientes
/// </summary>
public class GetAllCustomersQueryValidator : AbstractValidator<GetAllCustomersQuery>
{
    public GetAllCustomersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("O número da página deve ser maior que zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("O tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(50)
            .WithMessage("O tamanho da página não pode ser maior que 50");
    }
}
