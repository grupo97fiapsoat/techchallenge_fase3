using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using FastFood.Application;
using FastFood.Application.Queries;
using FastFood.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FastFood.CpfFunction;

/// <summary>
/// AWS Lambda Function para identificar cliente por CPF
/// </summary>
public class Function
{
    private readonly IMediator _mediator;
    private readonly ILogger<Function> _logger;

    public Function()
    {
        // Configurar DI
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        
        // Adicionar logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Adicionar infraestrutura (DbContext, repositórios, MediatR)
        services.AddInfrastructure(configuration);
        services.AddApplication();

        var serviceProvider = services.BuildServiceProvider();
        
        _mediator = serviceProvider.GetRequiredService<IMediator>();
        _logger = serviceProvider.GetRequiredService<ILogger<Function>>();
    }

    /// <summary>
    /// Ponto de entrada para execução local
    /// </summary>
    public static void Main(string[] args)
    {
        // Este método é necessário para compilação, mas não é usado em Lambda
        Console.WriteLine("FastFood CPF Function - Use AWS Lambda para execução");
    }

    /// <summary>
    /// Handler para identificar cliente por CPF
    /// Rota: GET /identify?cpf=12345678900
    /// </summary>
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/identify")]
    public async Task<IHttpResult> IdentifyByCpf(
        [FromQuery] string cpf,
        ILambdaContext context)
    {
        try
        {
            _logger.LogInformation("Identificando cliente por CPF: {Cpf}", cpf);

            // Validar CPF
            if (string.IsNullOrWhiteSpace(cpf))
            {
                _logger.LogWarning("CPF não fornecido");
                return HttpResults.BadRequest(new { error = "CPF é obrigatório" });
            }

            // Remover formatação do CPF
            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

            // Validar formato básico do CPF
            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            {
                _logger.LogWarning("CPF inválido: {Cpf}", cpf);
                return HttpResults.BadRequest(new { error = "CPF deve conter 11 dígitos numéricos" });
            }

            // Consultar cliente
            var query = new GetCustomerByCpfQuery { Cpf = cpf };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                _logger.LogInformation("Cliente não encontrado para CPF: {Cpf}", cpf);
                return HttpResults.NotFound(new { error = "Cliente não encontrado" });
            }

            _logger.LogInformation("Cliente encontrado: {CustomerId}", result.Id);
            return HttpResults.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao identificar cliente por CPF: {Cpf}", cpf);
            return HttpResults.InternalServerError(new { error = "Erro interno do servidor" });
        }
    }
}