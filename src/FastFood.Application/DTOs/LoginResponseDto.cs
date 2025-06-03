namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para resposta de login do usuário.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Token JWT de autenticação.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Data e hora de expiração do token.
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// ID do usuário logado.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Nome de usuário do usuário logado.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Roles do usuário logado.
    /// </summary>
    public required List<string> Roles { get; set; }
}
