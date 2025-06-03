using FluentValidation;
using FastFood.Application.Commands;

namespace FastFood.Application.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{    public CreateOrderCommandValidator()
    {
        // CustomerId é opcional para pedidos anônimos
        // Quando informado, deve ser um GUID válido (não vazio)
        RuleFor(x => x.CustomerId)
            .Must(customerId => !customerId.HasValue || customerId.Value != Guid.Empty)
            .WithMessage("Quando informado, o ID do cliente deve ser um GUID válido");

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
