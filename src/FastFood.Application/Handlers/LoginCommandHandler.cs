using FastFood.Application.Commands;
using FastFood.Domain.Users.Repositories;
using FastFood.Domain.Users.Services;
using MediatR;

namespace FastFood.Application.Handlers;

/// <summary>
/// Manipulador para o comando de login.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    /// <summary>
    /// Inicializa uma nova instância do manipulador de login.
    /// </summary>
    /// <param name="userRepository">Repositório de usuários</param>
    /// <param name="authService">Serviço de autenticação</param>
    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    /// <summary>
    /// Manipula o comando de login.
    /// </summary>
    /// <param name="request">Dados do login</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado do login</returns>
    public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar usuário por nome de usuário ou email
            var user = await _userRepository.GetByUsernameOrEmailAsync(request.Username);
            
            if (user == null)
            {
                return new LoginCommandResult 
                { 
                    Success = false, 
                    Error = "Usuário ou senha inválidos" 
                };
            }

            // Verificar se o usuário está ativo
            if (!user.IsActive)
            {
                return new LoginCommandResult 
                { 
                    Success = false, 
                    Error = "Usuário desativado" 
                };
            }

            // Verificar se a senha está correta
            if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new LoginCommandResult 
                { 
                    Success = false, 
                    Error = "Usuário ou senha inválidos" 
                };
            }

            // Registrar o login
            user.RegisterLogin();
            await _userRepository.UpdateAsync(user);

            // Gerar token JWT
            string token = _authService.GenerateJwtToken(user);

            // Calcular data de expiração (60 minutos a partir de agora)
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            // Retornar resultado de sucesso
            return new LoginCommandResult
            {
                Success = true,
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Username = user.Username,
                Roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            // Log do erro
            return new LoginCommandResult 
            { 
                Success = false, 
                Error = "Erro ao realizar login: " + ex.Message 
            };
        }
    }
}
