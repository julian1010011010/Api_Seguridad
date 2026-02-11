using Api_Seguridad.Domain.ApiKeys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api_Seguridad.Api.Security;

public sealed class ApiScopeFilter : IAuthorizationFilter
{
    private readonly string _requiredScope;
    private readonly ILogger<ApiScopeFilter> _logger;

    public ApiScopeFilter(string requiredScope, ILogger<ApiScopeFilter> logger)
    {
        _requiredScope = requiredScope;
        _logger = logger;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Items.TryGetValue("ApiClient", out var apiClientObj) || apiClientObj is not ApiKey apiKey)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var scopes = (apiKey.Permisos ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToUpperInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!scopes.Contains(_requiredScope.ToUpperInvariant()))
        {
            _logger.LogWarning("API Key de {Cliente} sin scope requerido {Scope}", apiKey.NombreCliente, _requiredScope);
            context.Result = new ForbidResult();
        }
    }
}
