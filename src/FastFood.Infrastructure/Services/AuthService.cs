using FastFood.Domain.Users.Entities;
using FastFood.Domain.Users.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FastFood.Infrastructure.Services;

/// <summary>
/// ⚠️ USO EXCLUSIVO EM DESENVOLVIMENTO ⚠️
/// 
/// Este serviço é APENAS para desenvolvimento local!
/// Em produção, a autenticação deve ser feita via IdP externo (Cognito/Google/Azure AD).
/// 
/// O pipeline de autenticação da API usa Authority/Audience para validação via JWKS
/// e NÃO depende deste serviço quando Auth:Authority/Audience estiverem configurados.
/// 
/// Este serviço gera tokens JWT locais usando chave simétrica (Jwt:Secret)
/// e deve ser usado APENAS para testes e desenvolvimento.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;

    /// <summary>
    /// Inicializa uma nova instância do serviço de autenticação.
    /// ⚠️ DEV ONLY - Em produção use IdP externo
    /// </summary>
    /// <param name="configuration">Configuração da aplicação</param>
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado");
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? "fastfood-api";
        _jwtAudience = _configuration["Jwt:Audience"] ?? "fastfood-clients";
        _jwtExpirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out int exp) ? exp : 60;
    }

    /// <summary>
    /// Cria um hash para a senha fornecida.
    /// </summary>
    /// <param name="password">Senha em texto puro</param>
    /// <returns>Hash da senha</returns>
    public string HashPassword(string password)
    {
        // Use BCrypt ou PBKDF2 para criação de hash de senha
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado.
    /// </summary>
    /// <param name="password">Senha em texto puro</param>
    /// <param name="passwordHash">Hash da senha armazenado</param>
    /// <returns>true se a senha corresponde ao hash; false caso contrário</returns>
    public bool VerifyPassword(string password, string passwordHash)
    {
        byte[] hashBytes = Convert.FromBase64String(passwordHash);
        
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(20);
        
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }
        
        return true;
    }

    /// <summary>
    /// Gera um token JWT para o usuário.
    /// ⚠️ DEV ONLY - Em produção use IdP externo
    /// </summary>
    /// <param name="user">Usuário para o qual gerar o token</param>
    /// <returns>Token JWT</returns>
    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        // Adicionar roles como claims
        foreach (var role in user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Valida um token JWT.
    /// ⚠️ DEV ONLY - Em produção use IdP externo
    /// </summary>
    /// <param name="token">Token JWT a ser validado</param>
    /// <returns>true se o token é válido; false caso contrário</returns>
    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtIssuer,
            ValidAudience = _jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret))
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}