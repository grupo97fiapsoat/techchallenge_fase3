using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para criação de um novo usuário.
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Nome de usuário.
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres")]
    public required string Username { get; set; }

    /// <summary>
    /// Email do usuário.
    /// </summary>
    /// <example>admin@example.com</example>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email é inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public required string Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <example>Admin@123</example>
    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", 
        ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
    public required string Password { get; set; }

    /// <summary>
    /// Roles do usuário (separadas por vírgula).
    /// </summary>
    /// <example>Admin,User</example>
    [Required(ErrorMessage = "Pelo menos uma role é obrigatória")]
    public required string Roles { get; set; }
}
