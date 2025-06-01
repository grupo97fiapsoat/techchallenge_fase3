using FastFood.Api.Models;
using FastFood.Application.Commands;
using FastFood.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

/// <summary>
/// Controlador para autenticação de usuários
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Inicializa uma nova instância do controlador de autenticação
    /// </summary>
    /// <param name="mediator">Mediador para comandos e consultas</param>
    /// <param name="logger">Logger</param>
    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Realiza o login de um usuário
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token JWT e informações do usuário</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        _logger.LogInformation("Tentativa de login para usuário: {Username}", request.Username);

        var command = new LoginCommand
        {
            Username = request.Username,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Falha no login para usuário: {Username}. Erro: {Error}", 
                request.Username, result.Error);
              return Unauthorized(new ErrorResponse(result.Error ?? "Credenciais inválidas"));
        }

        _logger.LogInformation("Login bem-sucedido para usuário: {Username}", request.Username);
        return Ok(result);
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="request">Dados do novo usuário</param>
    /// <returns>Informações do usuário registrado</returns>
    /// <response code="201">Usuário registrado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterUserCommandResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        _logger.LogInformation("Tentativa de registro para usuário: {Username}", request.Username);

        var command = new RegisterUserCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _logger.LogWarning("Falha no registro para usuário: {Username}. Erro: {Error}", 
                request.Username, result.Error);
              return BadRequest(new ErrorResponse(result.Error ?? "Erro ao registrar usuário"));
        }

        _logger.LogInformation("Registro bem-sucedido para usuário: {Username}", request.Username);
        return CreatedAtAction(nameof(Login), result);
    }
}
