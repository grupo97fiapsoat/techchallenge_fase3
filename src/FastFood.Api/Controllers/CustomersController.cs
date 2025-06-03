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

    /// <param name="createCustomerDto">Dados do cliente (nome, email, CPF)</param>
    /// <returns>Cliente criado com ID único gerado</returns>
    /// <response code="201">Cliente criado com sucesso</response>
    /// <response code="400">Dados inválidos - Verifique CPF, email ou nome</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Apenas administradores podem criar clientes</response>
    /// <response code="409">CPF já cadastrado - Cliente já existe no sistema</response>
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
    /// <param name="cpf">CPF do cliente (com ou sem formatação)</param>
    /// <returns>Dados completos do cliente encontrado</returns>
    /// <response code="200">Cliente encontrado e dados retornados</response>
    /// <response code="404">Cliente não encontrado - CPF não cadastrado no sistema</response>
    /// <response code="400">CPF inválido - Formato ou dígitos verificadores incorretos</response>
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

   
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 10, máximo recomendado: 50)</param>
    /// <returns>Lista paginada de clientes cadastrados</returns>
    /// <response code="200">Lista de clientes retornada com sucesso</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Token válido mas sem permissões adequadas</response>
    [HttpGet]
    [Authorize] // Exige autenticação para listar todos os clientes
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCustomersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        var response = result.Customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Cpf = c.Cpf,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Ok(response);
    }

    /// <param name="id">ID do cliente a ser atualizado</param>
    /// <param name="updateCustomerDto">Novos dados do cliente</param>
    /// <returns>Cliente com dados atualizados</returns>
    /// <response code="200">Cliente atualizado com sucesso</response>
    /// <response code="400">Dados inválidos - Verifique o formato dos dados</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Token válido mas sem permissões adequadas</response>
    /// <response code="404">Cliente não encontrado - Verifique se o ID está correto</response>
    /// <response code="409">CPF já cadastrado para outro cliente</response>
    [HttpPut("{id}")]
    [Authorize] // Exige autenticação para atualizar um cliente
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto updateCustomerDto)
    {
        var command = new UpdateCustomerCommand
        {
            Id = id,
            Name = updateCustomerDto.Name,
            Email = updateCustomerDto.Email,
            Cpf = updateCustomerDto.Cpf
        };

        var result = await _mediator.Send(command);

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

   
    /// <param name="id">ID do cliente a ser excluído</param>
    /// <returns>Sem conteúdo em caso de sucesso</returns>
    /// <response code="204">Cliente excluído com sucesso</response>
    /// <response code="400">Cliente não pode ser excluído - Possui pedidos associados</response>
    /// <response code="401">Token JWT inválido ou ausente - Faça login primeiro</response>
    /// <response code="403">Acesso negado - Token válido mas sem permissões adequadas</response>
    /// <response code="404">Cliente não encontrado - Verifique se o ID está correto</response>
    [HttpDelete("{id}")]
    [Authorize] // Exige autenticação para excluir um cliente
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCustomerCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
