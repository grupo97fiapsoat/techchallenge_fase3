using FastFood.Domain.Users.Entities;

namespace FastFood.Domain.Users.Repositories;

/// <summary>
/// Interface para o repositório de usuários.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Obtém um usuário pelo ID.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>O usuário encontrado ou null</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém um usuário pelo nome de usuário.
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <returns>O usuário encontrado ou null</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Obtém um usuário pelo email.
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>O usuário encontrado ou null</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Obtém um usuário pelo nome de usuário ou email.
    /// </summary>
    /// <param name="usernameOrEmail">Nome de usuário ou email</param>
    /// <returns>O usuário encontrado ou null</returns>
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="user">Usuário a ser criado</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task CreateAsync(User user);

    /// <summary>
    /// Atualiza um usuário existente.
    /// </summary>
    /// <param name="user">Usuário a ser atualizado</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task UpdateAsync(User user);

    /// <summary>
    /// Exclui um usuário.
    /// </summary>
    /// <param name="id">ID do usuário a ser excluído</param>
    /// <returns>Task representando a operação assíncrona</returns>
    Task DeleteAsync(Guid id);
}
