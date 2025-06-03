using FastFood.Domain.Customers.Entities;
using FastFood.Domain.Customers.Repositories;
using MediatR;

namespace FastFood.Application.Commands;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerCommandResult>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CreateCustomerCommandResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validação de CPF duplicado
        var existingCustomer = await _customerRepository.GetByCpfAsync(request.Cpf);
        if (existingCustomer != null)
            throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF");

        var customer = Customer.Create(request.Name, request.Email, request.Cpf);
        
        await _customerRepository.CreateAsync(customer);

        return new CreateCustomerCommandResult
        {
            Id = customer.Id,
            Name = customer.Name.Value,
            Email = customer.Email.Value,
            Cpf = customer.Cpf.Value,
            CreatedAt = customer.CreatedAt
        };
    }
}
