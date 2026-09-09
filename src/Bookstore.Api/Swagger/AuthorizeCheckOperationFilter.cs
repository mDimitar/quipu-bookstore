using Bookstore.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bookstore.Api.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var policies = context.MethodInfo.GetCustomAttributes(true)
            .Concat(context.MethodInfo.DeclaringType!.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>()
            .Select(a => a.Policy)
            .Where(p => p is not null)
            .Distinct();

        var scopes = policies.SelectMany(MapPolicyToScopes).Distinct().ToList();
        if (scopes.Count == 0) return;

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" } }] = scopes
            }
        };
    }

    private static IEnumerable<string> MapPolicyToScopes(string? policy) => policy switch
    {
        PolicyNames.CrudAccess => new[] { ApiScopes.Crud },
        PolicyNames.SearchAccess => new[] { ApiScopes.Search },
        _ => Array.Empty<string>()
    };
}
