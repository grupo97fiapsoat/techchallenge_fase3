using FastFood.Api.Extensions;
using FastFood.Api.Filters;
using FastFood.Api.Middlewares;
using FastFood.Application;
using FastFood.Infrastructure;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

// Handle command line arguments
var commandLineArgs = Environment.GetCommandLineArgs();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add health checks
builder.Services.AddHealthChecks();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (commandLineArgs.Contains("--migrate"))
{
    Console.WriteLine("🔧 Executando migrations...");
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FastFoodDbContext>();
        
        // Aplicar migrations pendentes
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations executadas com sucesso!");
        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao executar migrations: {ex.Message}");
        return 1;
    }
}

if (commandLineArgs.Contains("--check-db"))
{
    Console.WriteLine("🔍 Verificando conexão com banco de dados...");
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FastFoodDbContext>();
        
        // Verificar se consegue conectar
        var canConnect = await context.Database.CanConnectAsync();
        if (canConnect)
        {
            Console.WriteLine("✅ Conexão com banco de dados OK!");
            return 0;
        }
        else
        {
            Console.WriteLine("❌ Não foi possível conectar ao banco de dados!");
            return 1;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao verificar banco: {ex.Message}");
        return 1;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseMiddleware<ExceptionHandlerMiddleware>(); // Temporarily disabled to test filter

app.UseHttpsRedirection();
app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

// Return 0 for successful execution when using top-level statements
return 0;

// Torna a classe Program pública para testes de integração
public partial class Program { }
