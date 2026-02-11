using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api_Seguridad.Controllers;

/// <summary>
/// Consulta de existencia de ApiKey a partir de su valor visible.
/// </summary>
[ApiController]
[Route("api/apikey")] // Ruta explícita
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeyController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    /// <summary>
    /// Verifica si una ApiKey visible existe en la base de datos.
    /// </summary>
    /// <param name="apiKey">ApiKey visible generada (ej: Cliente-Guid-YYYYMMDD)</param>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ApiKeyRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.ApiKey))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Code = CodigosResultado.RESULTADO_FORMULARIO_NO_VALIDO,
                Message = "El formulario contiene errores de validación",
                Errors = new List<string> { "ApiKey es obligatoria" }
            });
        }

        var entity = await _apiKeyService.BuscarPorApiKeyVisibleAsync(request.ApiKey, cancellationToken);
        var exists = entity is not null;

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Code = CodigosResultado.RESULTADO_EXITOSO,
            Message = exists ? "ApiKey existe" : "ApiKey no encontrada",
            Data = new { existe = exists }
        });
    }
}

public sealed class ApiKeyRequest
{
    public string ApiKey { get; set; } = string.Empty;
}
