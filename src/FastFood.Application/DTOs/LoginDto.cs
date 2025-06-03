using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para requisição de login do usuário.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Nome de usuário ou email para login.
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    public required string Username { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <example>Admin@123</example>
    [Required(ErrorMessage = "A senha é obrigatória")]
    public required string Password { get; set; }
}
