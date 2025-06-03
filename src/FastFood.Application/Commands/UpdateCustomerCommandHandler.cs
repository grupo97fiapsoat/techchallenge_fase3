using FastFood.Domain.Customers.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Handler para processar o command de atualização de cliente
/// </summary>
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerCommandResult>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }    public async Task<UpdateCustomerCommandResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Cliente com ID {request.Id} não encontrado");

        // Atualiza os dados do cliente usando o método de domínio
        // Usa os valores atuais se os novos não forem fornecidos
        var newName = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : customer.Name.Value;
        var newEmail = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : customer.Email.Value;

        // O CPF não pode ser alterado após a criação, conforme definido na entidade
        customer.Update(newName, newEmail);

        await _customerRepository.UpdateAsync(customer);

        return new UpdateCustomerCommandResult
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Cpf = customer.Cpf,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
