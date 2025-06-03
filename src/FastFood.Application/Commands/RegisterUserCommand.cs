using MediatR;

namespace FastFood.Application.Commands;

/// <summary>
/// Comando para registro de novo usuário.
/// </summary>
public class RegisterUserCommand : IRequest<RegisterUserCommandResult>
{
    /// <summary>
    /// Nome de usuário para login.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Email do usuário.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Confirmação da senha.
    /// </summary>
    public required string ConfirmPassword { get; set; }
}

/// <summary>
/// Resultado do comando de registro de usuário.
/// </summary>
public class RegisterUserCommandResult
{
    /// <summary>
    /// Indica se o registro foi bem-sucedido.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de erro, caso ocorra algum problema.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// ID do usuário registrado.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Nome de usuário do usuário registrado.
    /// </summary>
    public string? Username { get; set; }
}
