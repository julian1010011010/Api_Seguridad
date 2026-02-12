using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Application.Jwt;
using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api_Seguridad.Controllers;

/// <summary>
/// Consulta de existencia de ApiKey a partir de su valor visible.
/// </summary>
[ApiController]
[Route("api/apikey")]
[EnableRateLimiting("apikey-validate")]
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<ApiKeyController> _logger;

    public ApiKeyController(
        IApiKeyService apiKeyService,
        IJwtTokenService jwtTokenService,
        ILogger<ApiKeyController> logger)
    {
        _apiKeyService = apiKeyService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Valida una API Key y genera un JWT interno de corta vida.
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<ValidateApiKeyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Post([FromBody] ValidateApiKeyRequest request, CancellationToken cancellationToken)
    {
        // 1. Validar modelo de entrada
        if (request is null || string.IsNullOrWhiteSpace(request.ApiKey)
            || string.IsNullOrWhiteSpace(request.Permission)
            || string.IsNullOrWhiteSpace(request.Audience))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = CodigosResultado.RESULTADO_FORMULARIO_NO_VALIDO,
                Message = "El formulario contiene errores de validación",
                Errors = new List<string> { "apiKey, permission y audience son obligatorios" }
            });
        }

        var requestId = request.Context?.RequestId ?? "N/A";
        var clientIp = request.Context?.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = await _apiKeyService.BuscarPorApiKeyVisibleAsync(
            request.ApiKey,
            request.Permission,
            cancellationToken);

        if (!result.IsValid || result.ApiKey is null)
        {
            _logger.LogWarning("API Key rechazada | Motivo={Error} RequestId={RequestId} IP={Ip}",
                result.Error, requestId, clientIp);

            var statusCode = result.Error?.Contains("permiso") == true
                ? StatusCodes.Status403Forbidden
                : StatusCodes.Status401Unauthorized;

            return StatusCode(statusCode, new ApiResponse<object>
            {
                Success = false,
                Code = CodigosResultado.RESULTADO_ERROR,
                Message = result.Error ?? "API Key no válida"
            });
        }
        
        var tokenResponse = _jwtTokenService.GenerateToken(
            result.ApiKey,
            request.Audience,
            request.Permission);

        _logger.LogInformation("JWT emitido | ClientId={ClientId} Scope={Scope} Audience={Audience} RequestId={RequestId} IP={Ip}",
            result.ApiKey.IdApiKey, request.Permission, request.Audience, requestId, clientIp);

        return Ok(new ApiResponse<ValidateApiKeyResponse>
        {
            Success = true,
            Code = CodigosResultado.RESULTADO_EXITOSO,
            Message = "Token generado exitosamente",
            Data = tokenResponse
        });
    }
}
