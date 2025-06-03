using System.Text.Json;
using FastFood.Application.Common.Exceptions;
using FastFood.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFood.Api.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var exception = context.Exception;
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case NotFoundException notFoundException:
                HandleNotFoundException(context, notFoundException, problemDetails);
                break;

            case ValidationException validationException:
                HandleValidationException(context, validationException, problemDetails);
                break;

            case DomainException domainException:
                HandleDomainException(context, domainException, problemDetails);
                break;

            default:
                HandleUnknownException(context, exception, problemDetails);
                break;
        }

        context.Result = new ObjectResult(problemDetails);
        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context, NotFoundException exception, ProblemDetails problemDetails)
    {
        problemDetails.Status = StatusCodes.Status404NotFound;
        problemDetails.Title = exception.Message;
        problemDetails.Type = "NotFound";
        problemDetails.Detail = exception.Message;

        context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
    }    private void HandleValidationException(ExceptionContext context, ValidationException exception, ProblemDetails problemDetails)
    {
        problemDetails.Status = StatusCodes.Status400BadRequest;
        problemDetails.Title = exception.Message;
        problemDetails.Type = "ValidationFailure";
        problemDetails.Detail = JsonSerializer.Serialize(exception.Errors);

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }

    private void HandleDomainException(ExceptionContext context, DomainException exception, ProblemDetails problemDetails)
    {
        problemDetails.Status = StatusCodes.Status400BadRequest;
        problemDetails.Title = exception.Message;
        problemDetails.Type = "DomainError";
        problemDetails.Detail = exception.Message;

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }

    private void HandleUnknownException(ExceptionContext context, Exception exception, ProblemDetails problemDetails)
    {
        problemDetails.Status = StatusCodes.Status500InternalServerError;
        problemDetails.Title = "Um erro ocorreu ao processar sua requisição.";
        problemDetails.Type = "ServerError";
        problemDetails.Detail = exception.Message;

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    }
}
