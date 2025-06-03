using FastFood.Domain.Customers.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Handler para processar o command de exclusão de cliente
/// </summary>
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com ID {request.Id} não encontrado");

        await _customerRepository.DeleteAsync(customer, cancellationToken);
    }
}
