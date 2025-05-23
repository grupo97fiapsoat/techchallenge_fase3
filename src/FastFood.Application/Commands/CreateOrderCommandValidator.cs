using FluentValidation;
using FastFood.Application.Commands;

namespace FastFood.Application.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("O ID do cliente é obrigatório");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Os itens do pedido são obrigatórios")
            .Must(items => items != null && items.Count > 0).WithMessage("O pedido deve ter pelo menos um item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero");
        });
    }
}
