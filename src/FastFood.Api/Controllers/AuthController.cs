using FastFood.Api.Models;
using FastFood.Application.Commands;
using FastFood.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

/// <summary>
/// Controlador para autenticação e registro de usuários
/// 
/// **Finalidade:** Gerencia o sistema de autenticação JWT da aplicação.
/// 
/// **Endpoints disponíveis:**
/// - **Login**: Autentica um usuário e retorna um token JWT
/// - **Registro**: Registra um novo usuário no sistema
/// 
/// **Fluxo típico:**
/// 1. Registre-se usando o endpoint `/register` 
/// 2. Faça login usando o endpoint `/login`
/// 3. Use o token retornado no header `Authorization: Bearer {token}` para acessar endpoints protegidos
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
    }    /// <summary>
    /// Realiza o login de um usuário existente
    /// 
    /// **Como usar:**
    /// 1. Envie as credenciais (username e password) no corpo da requisição
    /// 2. Se as credenciais estiverem corretas, receberá um token JWT na resposta
    /// 3. Use este token no header `Authorization: Bearer {token}` para acessar endpoints protegidos
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "username": "admin",
    ///   "password": "admin123"
    /// }
    /// ```
    /// 
    /// **Resposta de sucesso inclui:**
    /// - Token JWT válido por 24 horas
    /// - Informações básicas do usuário
    /// - Data de expiração do token
    /// </summary>
    /// <param name="request">Dados de login (username e password)</param>
    /// <returns>Token JWT e informações do usuário autenticado</returns>
    /// <response code="200">Login realizado com sucesso - Retorna token JWT</response>
    /// <response code="400">Dados inválidos - Verifique o formato dos dados enviados</response>
    /// <response code="401">Credenciais inválidas - Username ou password incorretos</response>
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
    }    /// <summary>
    /// Registra um novo usuário no sistema
    /// 
    /// **Como usar:**
    /// 1. Envie os dados do novo usuário no corpo da requisição
    /// 2. A senha deve ter pelo menos 6 caracteres
    /// 3. O username deve ser único no sistema
    /// 4. Após o registro, use o endpoint `/login` para obter um token
    /// 
    /// **Validações aplicadas:**
    /// - Username: obrigatório e único
    /// - Email: formato válido e único
    /// - Password: mínimo 6 caracteres
    /// - ConfirmPassword: deve ser igual ao password
    /// 
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "username": "novouser",
    ///   "email": "user@email.com",
    ///   "password": "senha123",
    ///   "confirmPassword": "senha123"
    /// }
    /// ```
    /// </summary>
    /// <param name="request">Dados do novo usuário (username, email, password, confirmPassword)</param>
    /// <returns>Informações do usuário registrado (sem token - faça login para obter)</returns>
    /// <response code="201">Usuário registrado com sucesso - Faça login para obter o token</response>
    /// <response code="400">Dados inválidos - Verifique se todos os campos estão preenchidos corretamente</response>
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
