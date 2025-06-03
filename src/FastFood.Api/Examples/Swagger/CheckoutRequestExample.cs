using FastFood.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace FastFood.Api.Examples.Swagger;

/// <summary>
/// Exemplo de request para checkout
/// </summary>
public class ProcessCheckoutDtoExample : IExamplesProvider<ProcessCheckoutDto>
{
    public ProcessCheckoutDto GetExamples()
    {
        return new ProcessCheckoutDto
        {
            OrderId = Guid.Parse("a47ac10b-58cc-4372-a567-0e02b2c3d789")
        };
    }
}

/// <summary>
/// Exemplo de response para checkout
/// </summary>
public class CheckoutResponseDtoExample : IExamplesProvider<CheckoutResponseDto>
{
    public CheckoutResponseDto GetExamples()
    {
        return new CheckoutResponseDto
        {
            OrderId = Guid.Parse("a47ac10b-58cc-4372-a567-0e02b2c3d789"),
            Status = "Paid",
            TotalAmount = 45.90m,
            QrCode = "ORDEM_a47ac10b-58cc-4372-a567-0e02b2c3d789_VALOR_45.90",
            ProcessedAt = DateTime.UtcNow
        };
    }
}
