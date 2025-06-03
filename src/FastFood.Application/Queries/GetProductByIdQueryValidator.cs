using FluentValidation;

namespace FastFood.Application.Queries;

/// <summary>
/// Validador para a query de busca de produto por ID.
/// </summary>
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    /// <summary>
    /// Inicializa uma nova instância do validador.
    /// </summary>
    public GetProductByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("O ID do produto é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O ID do produto não pode ser vazio.");
    }
}
