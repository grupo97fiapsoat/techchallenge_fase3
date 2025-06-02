using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace FastFood.Api.Filters
{
    /// <summary>
    /// Filtro para adicionar suporte a autenticação JWT no Swagger
    /// </summary>
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica se o endpoint tem o atributo [AllowAnonymous]
            var hasAllowAnonymous = (context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? Enumerable.Empty<object>())
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>()
                .Any();

            if (hasAllowAnonymous)
                return;

            // Verifica se o endpoint requer autenticação
            var hasAuthorize = (context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? Enumerable.Empty<object>())
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Any();            if (hasAuthorize)
            {
                // Adiciona resposta 401 apenas se não existir
                if (!operation.Responses.ContainsKey("401"))
                {
                    operation.Responses.Add("401", new OpenApiResponse 
                    { 
                        Description = "**Não autorizado** - Token JWT inválido, expirado ou ausente. Faça login em `/api/v1/auth/login` para obter um token válido." 
                    });
                }
                
                // Adiciona resposta 403 apenas se não existir
                if (!operation.Responses.ContainsKey("403"))
                {
                    operation.Responses.Add("403", new OpenApiResponse 
                    { 
                        Description = "**Acesso negado** - Token válido, mas permissões insuficientes para esta operação." 
                    });
                }

                // Adiciona uma descrição mais clara sobre a necessidade de autenticação
                if (string.IsNullOrEmpty(operation.Description))
                {
                    operation.Description = "**Este endpoint requer autenticação JWT.**";
                }
                else
                {
                    operation.Description = $"**Este endpoint requer autenticação JWT.**\n\n{operation.Description}";
                }

                var jwtbearerScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [jwtbearerScheme] = new string[] {}
                    }
                };
            }
            else
            {
                // Adiciona informação para endpoints públicos
                if (string.IsNullOrEmpty(operation.Description))
                {
                    operation.Description = "**Endpoint público** - Não requer autenticação.";
                }
                else
                {
                    operation.Description = $"**Endpoint público** - Não requer autenticação.\n\n{operation.Description}";
                }
            }
        }
    }
}
