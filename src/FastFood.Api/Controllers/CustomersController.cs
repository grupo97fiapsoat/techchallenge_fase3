using FastFood.Application.Commands;
using FastFood.Application.DTOs;
using FastFood.Application.Queries;
using FastFood.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

[ApiController]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="request">Dados do cliente</param>
    /// <returns>Cliente criado</returns>
    /// <response code="201">Cliente criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [Authorize] // Exige autenticação para criar um cliente
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto createCustomerDto)
    {
        var command = new CreateCustomerCommand
        {
            Name = createCustomerDto.Name,
            Email = createCustomerDto.Email,
            Cpf = createCustomerDto.Cpf
        };

        var result = await _mediator.Send(command);

        var response = new CustomerDto
        {
            Id = result.Id,
            Name = result.Name,
            Email = result.Email,
            Cpf = result.Cpf,
            CreatedAt = result.CreatedAt
        };

        return Created($"/api/v1/customers/cpf/{response.Cpf}", response);
    }

    /// <summary>
    /// Busca um cliente pelo CPF
    /// </summary>
    /// <param name="cpf">CPF do cliente</param>
    /// <returns>Cliente encontrado</returns>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente não encontrado</response>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCpf(string cpf)
    {
        var query = new GetCustomerByCpfQuery { Cpf = cpf };
        var result = await _mediator.Send(query);

        if (result == null)
            throw new KeyNotFoundException("Cliente não encontrado");

        var response = new CustomerDto
        {
            Id = result.Id,
            Name = result.Name,
            Email = result.Email,
            Cpf = result.Cpf,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };

        return Ok(response);
    }
}
