using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs;

/// <summary>
/// DTO para requisição de registro de novo usuário.
/// </summary>
public class RegisterUserDto
{
    /// <summary>
    /// Nome de usuário para login.
    /// </summary>
    /// <example>novousuario</example>
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "O nome de usuário deve conter apenas letras, números, pontos e sublinhados")]
    public required string Username { get; set; }

    /// <summary>
    /// Email do usuário.
    /// </summary>
    /// <example>usuario@exemplo.com</example>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email informado não é válido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public required string Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <example>Senha@123</example>
    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", 
        ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial")]
    public required string Password { get; set; }

    /// <summary>
    /// Confirmação da senha.
    /// </summary>
    /// <example>Senha@123</example>
    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não conferem")]
    public required string ConfirmPassword { get; set; }
}
