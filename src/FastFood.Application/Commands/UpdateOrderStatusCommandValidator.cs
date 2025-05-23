using FluentValidation;
using FastFood.Domain.Orders.ValueObjects;

namespace FastFood.Application.Commands;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID do pedido é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("O status é obrigatório")
            .Must(BeValidStatus).WithMessage("Status inválido. Valores válidos: Pending, Processing, Ready, Completed, Cancelled");
    }    private bool BeValidStatus(string status)
    {
        return Enum.TryParse(typeof(OrderStatus), status, true, out _);
    }
}
