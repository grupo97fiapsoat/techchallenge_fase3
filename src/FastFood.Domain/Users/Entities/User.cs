using FastFood.Domain.Shared.Entities;
using FastFood.Domain.Users.Exceptions;

namespace FastFood.Domain.Users.Entities;

/// <summary>
/// Representa um usuário do sistema.
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Nome de usuário único para login.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// Email do usuário.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Hash da senha do usuário.
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Roles do usuário (separadas por vírgula).
    /// </summary>
    public string Roles { get; private set; }

    /// <summary>
    /// Indica se o usuário está ativo.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Data da última vez que o usuário fez login.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Construtor privado para o EF Core.
    /// </summary>
    private User() : base()
    {
        Username = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        Roles = string.Empty;
        IsActive = true;
    }

    /// <summary>
    /// Cria uma nova instância de usuário.
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <param name="email">Email do usuário</param>
    /// <param name="passwordHash">Hash da senha</param>
    /// <param name="roles">Roles do usuário</param>
    /// <exception cref="UserDomainException">Lançada quando algum dos campos é inválido</exception>
    public User(string username, string email, string passwordHash, string roles) : base()
    {
        ValidateUsername(username);
        ValidateEmail(email);
        ValidatePasswordHash(passwordHash);
        ValidateRoles(roles);

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Roles = roles;
        IsActive = true;
    }

    /// <summary>
    /// Atualiza o hash da senha do usuário.
    /// </summary>
    /// <param name="passwordHash">Novo hash da senha</param>
    /// <exception cref="UserDomainException">Lançada quando o hash da senha é inválido</exception>
    public void UpdatePassword(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        PasswordHash = passwordHash;
        SetUpdatedAt();
    }

    /// <summary>
    /// Atualiza as roles do usuário.
    /// </summary>
    /// <param name="roles">Novas roles</param>
    /// <exception cref="UserDomainException">Lançada quando as roles são inválidas</exception>
    public void UpdateRoles(string roles)
    {
        ValidateRoles(roles);
        Roles = roles;
        SetUpdatedAt();
    }

    /// <summary>
    /// Atualiza o email do usuário.
    /// </summary>
    /// <param name="email">Novo email</param>
    /// <exception cref="UserDomainException">Lançada quando o email é inválido</exception>
    public void UpdateEmail(string email)
    {
        ValidateEmail(email);
        Email = email;
        SetUpdatedAt();
    }

    /// <summary>
    /// Atualiza o status de ativação do usuário.
    /// </summary>
    /// <param name="isActive">Novo status</param>
    public void UpdateActiveStatus(bool isActive)
    {
        IsActive = isActive;
        SetUpdatedAt();
    }

    /// <summary>
    /// Registra o login do usuário.
    /// </summary>
    public void RegisterLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    /// <summary>
    /// Verifica se o usuário possui uma determinada role.
    /// </summary>
    /// <param name="role">Role a ser verificada</param>
    /// <returns>true se o usuário possui a role; false caso contrário</returns>
    public bool HasRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        var roles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Valida o nome de usuário.
    /// </summary>
    /// <param name="username">Nome de usuário a ser validado</param>
    /// <exception cref="UserDomainException">Lançada quando o nome de usuário é inválido</exception>
    private void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new UserDomainException("O nome de usuário é obrigatório");

        if (username.Length < 3)
            throw new UserDomainException("O nome de usuário deve ter pelo menos 3 caracteres");

        if (username.Length > 50)
            throw new UserDomainException("O nome de usuário deve ter no máximo 50 caracteres");

        if (!username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.'))
            throw new UserDomainException("O nome de usuário deve conter apenas letras, números, pontos e sublinhados");
    }

    /// <summary>
    /// Valida o email do usuário.
    /// </summary>
    /// <param name="email">Email a ser validado</param>
    /// <exception cref="UserDomainException">Lançada quando o email é inválido</exception>
    private void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new UserDomainException("O email é obrigatório");

        if (email.Length > 100)
            throw new UserDomainException("O email deve ter no máximo 100 caracteres");

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
                throw new UserDomainException("O email é inválido");
        }
        catch
        {
            throw new UserDomainException("O email é inválido");
        }
    }

    /// <summary>
    /// Valida o hash da senha.
    /// </summary>
    /// <param name="passwordHash">Hash da senha a ser validado</param>
    /// <exception cref="UserDomainException">Lançada quando o hash da senha é inválido</exception>
    private void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new UserDomainException("O hash da senha é obrigatório");
    }

    /// <summary>
    /// Valida as roles do usuário.
    /// </summary>
    /// <param name="roles">Roles a serem validadas</param>
    /// <exception cref="UserDomainException">Lançada quando as roles são inválidas</exception>
    private void ValidateRoles(string roles)
    {
        if (string.IsNullOrWhiteSpace(roles))
            throw new UserDomainException("Pelo menos uma role é obrigatória");
    }
}
