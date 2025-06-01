using FastFood.Domain.Customers.Repositories;
using MediatR;

namespace FastFood.Application.Queries;

public class GetCustomerByCpfQueryHandler : IRequestHandler<GetCustomerByCpfQuery, GetCustomerByCpfQueryResult>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByCpfQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<GetCustomerByCpfQueryResult> Handle(GetCustomerByCpfQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByCpfAsync(request.Cpf);

        if (customer == null)
            return null;

        return new GetCustomerByCpfQueryResult
        {
            Id = customer.Id,
            Name = customer.Name.Value,
            Email = customer.Email.Value,
            Cpf = customer.Cpf.Value,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
