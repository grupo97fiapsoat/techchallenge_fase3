using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Command para excluir um cliente
/// </summary>
public class DeleteCustomerCommand : IRequest
{
    /// <summary>
    /// Identificador único do cliente a ser excluído
    /// </summary>
    public Guid Id { get; set; }
}
