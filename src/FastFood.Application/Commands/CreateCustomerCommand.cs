using MediatR;

namespace FastFood.Application.Commands;

public class CreateCustomerCommand : IRequest<CreateCustomerCommandResult>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
}

public class CreateCustomerCommandResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cpf { get; set; }
    public DateTime CreatedAt { get; set; }
}
