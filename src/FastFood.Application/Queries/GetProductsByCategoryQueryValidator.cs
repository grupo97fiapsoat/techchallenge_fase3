using FluentValidation;

namespace FastFood.Application.Queries;

public class GetProductsByCategoryQueryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    public GetProductsByCategoryQueryValidator()
    {        RuleFor(x => x.Category)
            .NotNull().WithMessage("A categoria é obrigatória")
            .IsInEnum().WithMessage("A categoria deve ser uma categoria válida");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("O número da página deve ser maior que zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("O tamanho da página deve ser maior que zero")
            .LessThanOrEqualTo(100).WithMessage("O tamanho da página não pode ser maior que 100");
    }
}
