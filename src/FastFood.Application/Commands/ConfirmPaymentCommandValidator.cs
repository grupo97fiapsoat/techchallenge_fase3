using FluentValidation;

namespace FastFood.Application.Commands;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("O ID do pedido é obrigatório");

        RuleFor(x => x.QrCode)
            .NotEmpty().WithMessage("O QR Code é obrigatório")
            .MinimumLength(10).WithMessage("QR Code deve ter pelo menos 10 caracteres");
    }
}
