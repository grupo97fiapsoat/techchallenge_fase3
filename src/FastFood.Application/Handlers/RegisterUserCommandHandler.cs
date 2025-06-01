using FastFood.Application.Commands;
using FastFood.Domain.Users.Entities;
using FastFood.Domain.Users.Repositories;
using FastFood.Domain.Users.Services;
using MediatR;

namespace FastFood.Application.Handlers;

/// <summary>
/// Manipulador para o comando de registro de usuário.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    /// <summary>
    /// Inicializa uma nova instância do manipulador de registro de usuário.
    /// </summary>
    /// <param name="userRepository">Repositório de usuários</param>
    /// <param name="authService">Serviço de autenticação</param>
    public RegisterUserCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    /// <summary>
    /// Manipula o comando de registro de usuário.
    /// </summary>
    /// <param name="request">Dados do registro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado do registro</returns>
    public async Task<RegisterUserCommandResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar se o nome de usuário já existe
            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUsername != null)
            {
                return new RegisterUserCommandResult 
                { 
                    Success = false, 
                    Error = "Nome de usuário já está em uso" 
                };
            }

            // Verificar se o email já existe
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return new RegisterUserCommandResult 
                { 
                    Success = false, 
                    Error = "Email já está em uso" 
                };
            }

            // Verificar se a senha e a confirmação de senha são iguais
            if (request.Password != request.ConfirmPassword)
            {
                return new RegisterUserCommandResult 
                { 
                    Success = false, 
                    Error = "A senha e a confirmação de senha não conferem" 
                };
            }

            // Gerar hash da senha
            string passwordHash = _authService.HashPassword(request.Password);

            // Criar novo usuário (role padrão: User)
            var user = new User(request.Username, request.Email, passwordHash, "User");            // Salvar usuário
            await _userRepository.CreateAsync(user);

            // Retornar resultado de sucesso
            return new RegisterUserCommandResult
            {
                Success = true,
                UserId = user.Id,
                Username = user.Username
            };
        }
        catch (Exception ex)
        {
            // Log do erro
            return new RegisterUserCommandResult 
            { 
                Success = false, 
                Error = "Erro ao registrar usuário: " + ex.Message 
            };
        }
    }
}
