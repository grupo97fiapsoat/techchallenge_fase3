using FastFood.Api.Extensions;
using FastFood.Api.Filters;
using FastFood.Api.Middlewares;
using FastFood.Application;
using FastFood.Infrastructure;
using FastFood.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

// Handle command line arguments
var commandLineArgs = Environment.GetCommandLineArgs();

var builder = WebApplication.CreateBuilder(args);

// Configurar o fuso horário padrão para o Brasil (UTC-3) com fallback cross-platform
TimeZoneInfo brazilTimeZone;
try
{
    // Windows
    brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
}
catch
{
    // Linux / containers
    brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
}
AppDomain.CurrentDomain.SetData("TimeZone", brazilTimeZone);

// Configurar a cultura padrão para pt-BR
var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Configurar autenticação JWT para IdP externo (Cognito/Google/etc)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority = builder.Configuration["Auth:Authority"];
        var audience  = builder.Configuration["Auth:Audience"];
        if (string.IsNullOrWhiteSpace(authority) || string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Auth:Authority e Auth:Audience devem ser configurados");

        options.Authority = authority;
        options.Audience  = audience;

        // Em DEV podemos aceitar HTTP no issuer local; em PROD exige HTTPS
        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = false;
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ClockSkew                = TimeSpan.FromMinutes(5),
            ValidIssuers   = new[] { authority },
            ValidAudiences = new[] { audience }
        };
    });

// Configurar políticas de autorização
builder.Services.AddAuthorization(options =>
{
    // Política para administradores (roles do token)
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            // Verificar roles do token (cognito:groups, roles, etc.)
            var roles = context.User.FindAll("cognito:groups")
                .Concat(context.User.FindAll("roles"))
                .Concat(context.User.FindAll("role"))
                .Select(c => c.Value);
            
            return roles.Any(role => role.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                                   role.Equals("administrator", StringComparison.OrdinalIgnoreCase));
        });
    });
});

// Add health checks
builder.Services.AddHealthChecks();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (commandLineArgs.Contains("--migrate"))
{
    Console.WriteLine("Executando migrations...");
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FastFoodDbContext>();
        
        // Aplicar migrations pendentes
        await context.Database.MigrateAsync();
        Console.WriteLine("Migrations executadas com sucesso!");
        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao executar migrations: {ex.Message}");
        return 1;
    }
}

if (commandLineArgs.Contains("--check-db"))
{
    Console.WriteLine("Verificando conexão com banco de dados...");
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FastFoodDbContext>();
        
        // Verificar se consegue conectar
        var canConnect = await context.Database.CanConnectAsync();
        if (canConnect)
        {
            Console.WriteLine("Conexão com banco de dados OK!");
            return 0;
        }
        else
        {
            Console.WriteLine("Não foi possível conectar ao banco de dados!");
            return 1;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao verificar banco: {ex.Message}");
        return 1;
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// app.UseMiddleware<ExceptionHandlerMiddleware>(); // Temporarily disabled to test filter

app.UseHttpsRedirection();

// Configurar os middlewares de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Bloquear rotas do auth "caseiro" fora de DEV
if (!app.Environment.IsDevelopment())
{
    app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/api/v1/auth"), branch =>
    {
        branch.Run(async c =>
        {
            c.Response.StatusCode = StatusCodes.Status404NotFound;
            await Task.CompletedTask;
        });
    });
}

app.Use(async (context, next) =>
{
    if (string.IsNullOrEmpty(context.Request.Path) || context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Aplicar migrations no startup da aplicação em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FastFoodDbContext>();
        
        // Aplicar migrations pendentes
        await context.Database.MigrateAsync();
        Console.WriteLine("Migrations aplicadas automaticamente no startup!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao aplicar migrations no startup: {ex.Message}");
        // Não falhar o startup por causa de erros de migração
    }
}

app.Run();

// Return 0 for successful execution when using top-level statements
return 0;

// Torna a classe Program pública para testes de integração
public partial class Program { }