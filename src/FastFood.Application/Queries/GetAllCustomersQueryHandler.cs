using FastFood.Domain.Customers.Repositories;
using MediatR;

namespace FastFood.Application.Queries;

/// <summary>
/// Handler para processar a query de listagem de todos os clientes
/// </summary>
public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, GetAllCustomersQueryResult>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<GetAllCustomersQueryResult> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        var result = new GetAllCustomersQueryResult
        {
            Customers = customers.Select(c => new GetAllCustomersQueryResult.CustomerResult
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Cpf = c.Cpf,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
        };

        return result;
    }
}
