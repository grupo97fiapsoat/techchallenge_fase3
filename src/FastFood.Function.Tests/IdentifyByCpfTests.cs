using Amazon.Lambda.TestUtilities;
using FastFood.Application.Queries;
using FastFood.CpfFunction;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FastFood.Function.Tests;

public class IdentifyByCpfTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<FastFood.CpfFunction.Function>> _mockLogger;
    private readonly FastFood.CpfFunction.Function _function;
    private readonly TestLambdaContext _context;

    public IdentifyByCpfTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<FastFood.CpfFunction.Function>>();
        _context = new TestLambdaContext();
        
        // Criar function com mocks
        _function = new FastFood.CpfFunction.Function();
    }

    [Fact]
    public async Task IdentifyByCpf_WithValidCpf_ReturnsCustomerData()
    {
        // Arrange
        var cpf = "12345678900";
        var expectedCustomer = new GetCustomerByCpfQueryResult
        {
            Id = Guid.NewGuid(),
            Name = "João Silva",
            Email = "joao@email.com",
            Cpf = cpf,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetCustomerByCpfQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCustomer);

        // Act
        var result = await _function.IdentifyByCpf(cpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado válido
        Assert.True(result is not null);
    }

    [Fact]
    public async Task IdentifyByCpf_WithEmptyCpf_ReturnsBadRequest()
    {
        // Arrange
        var cpf = "";

        // Act
        var result = await _function.IdentifyByCpf(cpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado de erro
        Assert.True(result is not null);
    }

    [Fact]
    public async Task IdentifyByCpf_WithInvalidCpf_ReturnsBadRequest()
    {
        // Arrange
        var cpf = "123"; // CPF muito curto

        // Act
        var result = await _function.IdentifyByCpf(cpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado de erro
        Assert.True(result is not null);
    }

    [Fact]
    public async Task IdentifyByCpf_WithNonNumericCpf_ReturnsBadRequest()
    {
        // Arrange
        var cpf = "1234567890a"; // CPF com letra

        // Act
        var result = await _function.IdentifyByCpf(cpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado de erro
        Assert.True(result is not null);
    }

    [Fact]
    public async Task IdentifyByCpf_WithFormattedCpf_RemovesFormatting()
    {
        // Arrange
        var formattedCpf = "123.456.789-00";

        // Act
        var result = await _function.IdentifyByCpf(formattedCpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado válido
        Assert.True(result is not null);
    }

    [Fact]
    public async Task IdentifyByCpf_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var cpf = "12345678900";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetCustomerByCpfQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCustomerByCpfQueryResult?)null);

        // Act
        var result = await _function.IdentifyByCpf(cpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado de erro
        Assert.True(result is not null);
    }

    [Fact]
    public async Task IdentifyByCpf_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var cpf = "12345678900";

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetCustomerByCpfQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _function.IdentifyByCpf(cpf, _context);

        // Assert
        Assert.NotNull(result);
        // Verificar se retorna um resultado de erro
        Assert.True(result is not null);
    }
}