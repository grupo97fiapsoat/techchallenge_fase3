using FastFood.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace FastFood.Api.Examples.Swagger;

/// <summary>
/// Exemplo de request para criação de cliente
/// </summary>
public class CreateCustomerDtoExample : IExamplesProvider<CreateCustomerDto>
{
    public CreateCustomerDto GetExamples()
    {
        return new CreateCustomerDto
        {
            Name = "João da Silva",
            Email = "joao.silva@example.com",
            Cpf = "12345678900"
        };
    }
}

/// <summary>
/// Exemplo de response para cliente
/// </summary>
public class CustomerDtoExample : IExamplesProvider<CustomerDto>
{
    public CustomerDto GetExamples()
    {
        return new CustomerDto
        {
            Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            Name = "João da Silva",
            Email = "joao.silva@example.com",
            Cpf = "12345678900",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
