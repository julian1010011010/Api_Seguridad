using System.Net;
using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Api_Seguridad.Api.Security;

public class ApiKeyMiddleware
{
    private const string ApiKeyHeaderName = "X-API-KEY";
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyValidator validator)
    {
        // Rutas que no requieren API Key (p.ej., login y apiKey check)
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        if (path.StartsWith("/api/login") || path.StartsWith("/api/apikey"))
        {
            await _next(context);
            return;
        }

        var apiKey = context.Request.Headers[ApiKeyHeaderName].FirstOrDefault();

        var result = await validator.ValidateAsync(apiKey, context.RequestAborted);
        if (!result.IsValid || result.ApiKey == null)
        {
            _logger.LogWarning("Solicitud no autorizada por API Key desde {Ip} a {Path}",
                context.Connection.RemoteIpAddress?.ToString(),
                context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                Success = false,
                Code = CodigosResultado.RESULTADO_ERROR,
                Message = result.Error ?? "API key invalid or missing",
                Data = null,
                Errors = new List<string>()
            });
            return;
        }

        // Guardar datos del cliente en Items para uso posterior (scopes, logs, etc.)
        context.Items["ApiClient"] = result.ApiKey;

        await _next(context);
    }
}
