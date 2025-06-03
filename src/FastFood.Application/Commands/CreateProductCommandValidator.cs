using FluentValidation;

namespace FastFood.Application.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória")
            .MaximumLength(500).WithMessage("A descrição não pode ter mais de 500 caracteres");        RuleFor(x => x.Category)
            .NotNull().WithMessage("A categoria é obrigatória")
            .IsInEnum().WithMessage("A categoria deve ser uma categoria válida");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero");

        When(x => x.Images != null, () => {
            RuleForEach(x => x.Images)
                .NotEmpty().WithMessage("As URLs das imagens não podem ser vazias");
        });
    }
}
