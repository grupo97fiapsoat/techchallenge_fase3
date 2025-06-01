using FastFood.Api.Examples.Swagger;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace FastFood.Api.Extensions;

/// <summary>
/// Extensões para configuração do Swagger/OpenAPI
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configura o Swagger/OpenAPI com documentação XML e outras configurações
    /// </summary>
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddSwaggerExamplesFromAssemblyOf<CreateOrderDtoExample>();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FastFood API",
                Version = "v1",
                Description = "API para gerenciamento do sistema FastFood, incluindo cadastro de clientes, produtos, pedidos e checkout.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe SOAT",
                    Email = "soat@example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Configura documentação XML da API
            ConfigureXmlComments(options);

            // Agrupa endpoints por controller
            options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            options.DocInclusionPredicate((name, api) => true);

            // Configura autenticação (caso seja implementada no futuro)
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Configura exemplos de requests/responses
            ConfigureExamples(options);
        });

        return services;
    }

    private static void ConfigureXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        // API XML
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Application XML
        var applicationXmlFilename = "FastFood.Application.xml";
        var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFilename);
        if (File.Exists(applicationXmlPath))
        {
            options.IncludeXmlComments(applicationXmlPath);
        }

        // Domain XML
        var domainXmlFilename = "FastFood.Domain.xml";
        var domainXmlPath = Path.Combine(AppContext.BaseDirectory, domainXmlFilename);
        if (File.Exists(domainXmlPath))
        {
            options.IncludeXmlComments(domainXmlPath);
        }
    }

    private static void ConfigureExamples(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        options.ExampleFilters();

        // Exemplos de request/response
        options.UseAllOfForInheritance();
        options.UseOneOfForPolymorphism();
        options.SelectSubTypesUsing(baseType =>
        {
            return typeof(CreateOrderDtoExample).Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(baseType));
        });
    }
}
