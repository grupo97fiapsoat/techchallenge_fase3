using FluentValidation;

namespace FastFood.Application.Commands;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("O ID do pedido é obrigatório");

        // Pelo menos um dos campos deve ser fornecido
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.PreferenceId) || !string.IsNullOrWhiteSpace(x.QrCode))
            .WithMessage("É obrigatório fornecer o PreferenceId ou o QrCode para validação do pagamento");

        // Validação específica do PreferenceId quando fornecido
        RuleFor(x => x.PreferenceId)
            .MinimumLength(5).WithMessage("PreferenceId deve ter pelo menos 5 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.PreferenceId));

        // Validação específica do QrCode quando fornecido
        RuleFor(x => x.QrCode)
            .MinimumLength(10).WithMessage("QR Code deve ter pelo menos 10 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.QrCode));
    }
}
