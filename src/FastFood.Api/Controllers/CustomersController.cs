using FastFood.Application.Commands;
using FastFood.Application.DTOs;
using FastFood.Application.Queries;
using FastFood.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

/// <summary>
/// Controlador para gerenciamento de clientes do restaurante
/// 
/// **Finalidade:** Gerencia o cadastro e consulta de clientes que fazem pedidos no sistema.
/// 
/// **Funcionalidades principais:**
/// - Cadastro de novos clientes
/// - Consulta de clientes por CPF
/// - Listagem de todos os clientes
/// - Atualização de dados de clientes
/// - Exclusão de clientes
/// - Validação de dados pessoais
/// 
/// **Níveis de acesso:**
/// - **Protegidos**: Criar, listar, atualizar e excluir clientes (apenas administradores)
/// - **Públicos**: Buscar cliente por CPF (para facilitar pedidos)
/// 
/// **Validações implementadas:**
/// - CPF válido conforme algoritmo oficial
/// - Email em formato válido
/// - Nome obrigatório e não vazio
/// </summary>
[ApiController]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }    /// <summary>
    /// Cria um novo cliente no sistema
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT (acesso administrativo).
    /// 
    /// **Finalidade:** Registra um novo cliente no sistema para que possa fazer pedidos.
    /// 
    /// **Como usar:**
    /// 1. **Autenticação**: Inclua o token JWT de administrador no header Authorization
    /// 2. **Dados**: Envie nome, email e CPF válidos
    /// 3. **Validação**: Sistema valida CPF e email automaticamente
    /// 4. **Retorno**: Recebe ID do cliente criado
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// POST /api/v1/customers
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// Content-Type: application/json
    /// 
    /// {
    ///   "name": "João Silva",
    ///   "email": "joao.silva@email.com",
    ///   "cpf": "12345678901"
    /// }
    /// ```
    /// 
    /// **Validações aplicadas:**
    /// - **CPF**: Formato e dígitos verificadores válidos
    /// - **Email**: Formato de email válido
    /// - **Nome**: Obrigatório, não pode estar vazio
    /// - **Unicidade**: CPF deve ser único no sistema
    /// 
    /// **Próximos passos:**
    /// - Use o ID retornado para associar pedidos ao cliente
    /// - Cliente pode ser encontrado pelo CPF em consultas futuras
    /// </summary>
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
    }    /// <summary>
    /// Busca um cliente específico pelo CPF
    /// 
    /// **Endpoint público** - Não requer autenticação (facilita criação de pedidos).
    /// 
    /// **Finalidade:** Permite consultar se um cliente já está cadastrado usando seu CPF, facilitando o processo de criação de pedidos.
    /// 
    /// **Como usar:**
    /// 1. **CPF**: Forneça o CPF no formato com ou sem formatação (123.456.789-01 ou 12345678901)
    /// 2. **Consulta**: Sistema busca cliente cadastrado
    /// 3. **Retorno**: Dados completos do cliente se encontrado
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /api/v1/customers/cpf/12345678901
    /// Content-Type: application/json
    /// ```
    /// 
    /// **Resposta exemplo:**
    /// ```json
    /// {
    ///   "id": "550e8400-e29b-41d4-a716-446655440000",
    ///   "name": "João Silva",
    ///   "email": "joao.silva@email.com",
    ///   "cpf": "12345678901",
    ///   "createdAt": "2025-06-01T10:00:00Z",
    ///   "updatedAt": "2025-06-01T10:00:00Z"
    /// }
    /// ```
    /// 
    /// **Casos de uso:**
    /// - **Cliente encontrado**: Use o ID para criar pedidos
    /// - **Cliente não encontrado**: Cadastre novo cliente antes de criar pedido
    /// - **Integração**: Perfeito para validar clientes em interfaces de pedido
    /// 
    /// **Formatos de CPF aceitos:**
    /// - `12345678901` (apenas números)
    /// - `123.456.789-01` (formatado)
    /// </summary>
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

    /// <summary>
    /// Lista todos os clientes cadastrados no sistema com suporte a paginação
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT (acesso administrativo).
    /// 
    /// **Finalidade:** Permite listar todos os clientes cadastrados, facilitando a gestão de campanhas promocionais.
    /// 
    /// **Como usar:**
    /// 1. **Autenticação**: Inclua o token JWT de administrador no header Authorization    /// 2. **Paginação**: Use os parâmetros pageSize e pageNumber para controlar a quantidade de resultados
    /// 3. **Ordenação**: Os resultados são ordenados por data de cadastro (mais recentes primeiro)
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// GET /api/v1/customers?pageNumber=1&amp;pageSize=20
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// ```
    /// 
    /// **Resposta exemplo:**
    /// ```json
    /// [
    ///   {
    ///     "id": "550e8400-e29b-41d4-a716-446655440000",
    ///     "name": "João Silva",
    ///     "email": "joao.silva@email.com",
    ///     "cpf": "12345678901",
    ///     "createdAt": "2025-06-01T10:00:00Z",
    ///     "updatedAt": "2025-06-01T10:00:00Z"
    ///   },
    ///   {
    ///     "id": "550e8400-e29b-41d4-a716-446655440001",
    ///     "name": "Maria Oliveira",
    ///     "email": "maria.oliveira@email.com",
    ///     "cpf": "98765432109",
    ///     "createdAt": "2025-06-01T11:00:00Z",
    ///     "updatedAt": null
    ///   }
    /// ]
    /// ```
    /// 
    /// **Caso de uso:**
    /// - Gestão de campanhas promocionais
    /// - Relatórios administrativos
    /// - Análise de base de clientes
    /// </summary>
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

    /// <summary>
    /// Atualiza os dados de um cliente existente
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT (acesso administrativo).
    /// 
    /// **Finalidade:** Permite atualizar informações de um cliente já cadastrado no sistema.
    /// 
    /// **Como usar:**
    /// 1. **Autenticação**: Inclua o token JWT de administrador no header Authorization
    /// 2. **ID do cliente**: Forneça o ID do cliente a ser atualizado na URL
    /// 3. **Dados**: Envie os novos dados no formato JSON
    /// 4. **Parcial**: Apenas os campos enviados serão atualizados
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// PUT /api/v1/customers/550e8400-e29b-41d4-a716-446655440000
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// Content-Type: application/json
    /// 
    /// {
    ///   "name": "João Silva Atualizado",
    ///   "email": "joao.silva.novo@email.com"
    /// }
    /// ```
    /// 
    /// **Validações aplicadas:**
    /// - **Email**: Formato de email válido
    /// - **Nome**: Não pode estar vazio
    /// - **CPF**: Se enviado, deve ser válido e não pode já estar em uso por outro cliente
    /// </summary>
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

    /// <summary>
    /// Exclui um cliente do sistema
    /// 
    /// **Endpoint protegido** - Requer autenticação JWT (acesso administrativo).
    /// 
    /// **Finalidade:** Permite remover um cliente do sistema, caso necessário.
    /// 
    /// **Como usar:**
    /// 1. **Autenticação**: Inclua o token JWT de administrador no header Authorization
    /// 2. **ID do cliente**: Forneça o ID do cliente a ser excluído na URL
    /// 
    /// **Restrições:**
    /// - Clientes com pedidos associados não podem ser excluídos, para preservar o histórico
    /// - Operação não pode ser desfeita
    /// 
    /// **Exemplo de uso:**
    /// ```
    /// DELETE /api/v1/customers/550e8400-e29b-41d4-a716-446655440000
    /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// ```
    /// </summary>
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
