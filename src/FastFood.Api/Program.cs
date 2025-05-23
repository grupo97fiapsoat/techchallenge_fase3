using FastFood.Api.Extensions;
using FastFood.Api.Filters;
using FastFood.Api.Middlewares;
using FastFood.Application;
using FastFood.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseMiddleware<ExceptionHandlerMiddleware>(); // Temporarily disabled to test filter

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Torna a classe Program pública para testes de integração
public partial class Program { }
