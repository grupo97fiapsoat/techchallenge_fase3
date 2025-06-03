using FastFood.Domain.Users.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FastFood.Domain.Users.Repositories;

namespace FastFood.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de usuários usando Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly FastFoodDbContext _context;
    private readonly DbSet<User> _users;

    /// <summary>
    /// Inicializa uma nova instância do repositório.
    /// </summary>
    /// <param name="context">Contexto do EF Core</param>
    public UserRepository(FastFoodDbContext context)
    {
        _context = context;
        _users = context.Set<User>();
    }

    /// <summary>
    /// Obtém um usuário pelo ID.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>O usuário encontrado ou null</returns>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _users.FindAsync(id);
    }

    /// <summary>
    /// Obtém um usuário pelo nome de usuário.
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <returns>O usuário encontrado ou null</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _users.FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Obtém um usuário pelo email.
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>O usuário encontrado ou null</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Obtém um usuário pelo nome de usuário ou email.
    /// </summary>
    /// <param name="usernameOrEmail">Nome de usuário ou email</param>
    /// <returns>O usuário encontrado ou null</returns>
    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _users.FirstOrDefaultAsync(u => 
            u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="user">Usuário a ser criado</param>
    public async Task CreateAsync(User user)
    {
        await _users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza um usuário existente.
    /// </summary>
    /// <param name="user">Usuário a ser atualizado</param>
    public async Task UpdateAsync(User user)
    {
        _users.Update(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Exclui um usuário.
    /// </summary>
    /// <param name="id">ID do usuário a ser excluído</param>
    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
