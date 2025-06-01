using FastFood.Api.Models;
using FastFood.Domain.Customers.Exceptions;
using System.Net;
using System.Text.Json;

namespace FastFood.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (error)
            {
                case CustomerDomainException e:
                    // Erros de validação de domínio (400 Bad Request)
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Errors.Add(e.Message);
                    break;

                case InvalidOperationException e:
                    // Erros de operação inválida (400 Bad Request)
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Errors.Add(e.Message);
                    break;

                case KeyNotFoundException e:
                    // Recurso não encontrado (404 Not Found)
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Errors.Add(e.Message);
                    break;

                default:
                    // Erros não tratados (500 Internal Server Error)
                    _logger.LogError(error, "Erro não tratado: {Message}", error.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Errors.Add("Um erro interno ocorreu. Por favor, tente novamente mais tarde.");
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
}
