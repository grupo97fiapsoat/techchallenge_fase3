using MediatR;

namespace FastFood.Application.Queries;

public class GetCustomerByCpfQuery : IRequest<GetCustomerByCpfQueryResult>
{
    public string Cpf { get; set; }
}

public class GetCustomerByCpfQueryResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
