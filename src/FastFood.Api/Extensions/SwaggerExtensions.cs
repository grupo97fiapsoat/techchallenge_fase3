using FastFood.Api.Examples.Swagger;
using FastFood.Api.Filters;
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
        services.AddSwaggerExamplesFromAssemblyOf<CreateOrderDtoExample>();        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FastFood API",
                Version = "v1",
                Description = BuildApiDescription(),
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
            options.DocInclusionPredicate((name, api) => true);            // Configura autenticação JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"**Sistema de Autenticação JWT**

Use o endpoint `/api/v1/auth/login` para obter seu token JWT.

**Como usar:**
1. Faça login no endpoint `/api/v1/auth/login` com suas credenciais
2. Copie o token retornado no campo 'token' da resposta
3. Clique no botão 'Authorize' acima
4. Digite 'Bearer ' (com espaço) seguido do seu token
5. Exemplo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**Endpoints Públicos (não requerem autenticação):**
- POST /api/v1/auth/login - Login de usuário
- POST /api/v1/auth/register - Registro de novo usuário  
- GET /api/v1/products - Listar produtos
- GET /api/v1/products/{id} - Obter produto por ID
- GET /api/v1/products/category/{category} - Produtos por categoria
- POST /api/v1/orders - Criar pedido
- POST /api/v1/webhook/payment - Webhook de pagamento (MercadoPago)

**Endpoints Protegidos (requerem autenticação):**
- GET /api/v1/orders - Listar pedidos (admin)
- GET /api/v1/orders/{id} - Detalhes do pedido
- PUT /api/v1/orders/{id}/status - Atualizar status do pedido
- POST /api/v1/customers - Criar cliente
- POST /api/v1/products - Criar produto
- PUT /api/v1/products/{id} - Atualizar produto
- DELETE /api/v1/products/{id} - Excluir produto",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            // Adiciona a exigência de segurança globalmente
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
            
            // Adiciona o filtro de segurança para documentar os endpoints que requerem autenticação
            options.OperationFilter<Filters.SecurityRequirementsOperationFilter>();

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

    /// <summary>
    /// Constrói a descrição detalhada da API
    /// </summary>
    private static string BuildApiDescription()
    {
        return @"# API FastFood - Sistema de Gerenciamento de Restaurante

## Visão Geral
Esta API oferece um sistema completo para gerenciamento de restaurantes FastFood, incluindo:
- **Gestão de Produtos**: Cadastro, edição e consulta de produtos por categoria (Lanches, Bebidas, Sobremesas, Acompanhamentos)
- **Gestão de Clientes**: Registro e autenticação de clientes
- **Gestão de Pedidos**: Criação, acompanhamento e atualização de status de pedidos
- **Sistema de Pagamento**: Integração com MercadoPago para processar pagamentos

## Autenticação
A API utiliza **JWT (JSON Web Token)** para autenticação. Alguns endpoints são públicos, outros requerem autenticação.

### Como Autenticar:
1. **Registre-se** usando `POST /api/v1/auth/register`

2. **Faça login** usando `POST /api/v1/auth/login`

3. **Use o token** retornado no header `Authorization: Bearer {token}`

## Fluxo de Uso Típico

### Para Clientes:
1. **Consultar Menu**: Use `GET /api/v1/products/category/{category}` para ver produtos

2. **Criar Pedido**: Use `POST /api/v1/orders` com os produtos escolhidos

3. **Acompanhar Pedido**: Use `GET /api/v1/orders/{id}` para ver status

### Para Administradores:
1. **Autentique-se** primeiro
2. **Gerenciar Produtos**: CRUD completo de produtos
3. **Gerenciar Pedidos**: Visualizar e atualizar status dos pedidos
4. **Gerenciar Clientes**: Visualizar informações de clientes

## Categorias de Produtos
- **Lanche** (0): Hambúrguers, sanduíches, etc.
- **Acompanhamento** (1): Batatas, anéis de cebola, etc.
- **Bebida** (2): Refrigerantes, sucos, águas, etc.
- **Sobremesa** (3): Sorvetes, tortas, cookies, etc.

## Status de Pedidos
- **Recebido** (0): Pedido criado e aguardando pagamento
- **EmPreparacao** (1): Pagamento confirmado, preparando pedido
- **Pronto** (2): Pedido pronto para retirada
- **Finalizado** (3): Pedido entregue ao cliente

## Sistema de Pagamento
A API integra com MercadoPago para processar pagamentos automaticamente. Em desenvolvimento, usa um serviço fake para simular pagamentos.";
    }
}
