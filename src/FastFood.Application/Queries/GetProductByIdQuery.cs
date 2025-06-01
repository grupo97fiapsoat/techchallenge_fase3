using MediatR;

namespace FastFood.Application.Queries;

/// <summary>
/// Query para buscar um produto pelo ID.
/// </summary>
public class GetProductByIdQuery : IRequest<GetProductByIdQueryResult>
{
    /// <summary>
    /// ID do produto a ser buscado.
    /// </summary>
    public Guid Id { get; set; }
}
