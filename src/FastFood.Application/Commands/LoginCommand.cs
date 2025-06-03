using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Comando para autenticação de usuário.
/// </summary>
public class LoginCommand : IRequest<LoginCommandResult>
{
    /// <summary>
    /// Nome de usuário ou email para login.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public required string Password { get; set; }
}

/// <summary>
/// Resultado do comando de autenticação.
/// </summary>
public class LoginCommandResult
{
    /// <summary>
    /// Indica se o login foi bem-sucedido.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de erro, caso ocorra algum problema.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Token JWT de autenticação.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Data e hora de expiração do token.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// ID do usuário logado.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Nome de usuário do usuário logado.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Roles do usuário logado.
    /// </summary>
    public List<string>? Roles { get; set; }
}
