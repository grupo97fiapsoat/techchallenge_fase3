using FastFood.Domain.Users.Entities;

namespace FastFood.Domain.Users.Services;

/// <summary>
/// Interface para o serviço de autenticação.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Cria um hash para a senha fornecida.
    /// </summary>
    /// <param name="password">Senha em texto puro</param>
    /// <returns>Hash da senha</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado.
    /// </summary>
    /// <param name="password">Senha em texto puro</param>
    /// <param name="passwordHash">Hash da senha armazenado</param>
    /// <returns>true se a senha corresponde ao hash; false caso contrário</returns>
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Gera um token JWT para o usuário.
    /// </summary>
    /// <param name="user">Usuário para o qual gerar o token</param>
    /// <returns>Token JWT</returns>
    string GenerateJwtToken(User user);

    /// <summary>
    /// Valida um token JWT.
    /// </summary>
    /// <param name="token">Token JWT a ser validado</param>
    /// <returns>true se o token é válido; false caso contrário</returns>
    bool ValidateToken(string token);
}
