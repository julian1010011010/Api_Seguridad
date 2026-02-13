using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Domain.Auth;
using Api_Seguridad.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Seguridad.Controllers;

/// <summary>
/// Endpoint de autenticación para clientes externos.
/// Realiza validación de credenciales y rota/genera una nueva API Key asociada.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
	private readonly IApiKeyService _servicioAutenticacionApiKey;

	/// <summary>
	/// Constructor del controlador de Login.
	/// </summary>
	/// <param name="servicioAutenticacionApiKey">Servicio de rotación/creación de API Key</param>
	public LoginController(IApiKeyService servicioAutenticacionApiKey)
	{
		_servicioAutenticacionApiKey = servicioAutenticacionApiKey;
	}

	/// <summary>
	/// Autentica un cliente externo y genera una nueva API Key.
	/// </summary>
	/// <param name="request">Credenciales del usuario (Usuario y Password)</param>
	/// <param name="cancellationToken">Token de cancelación</param>
	/// <returns>
	/// Respuesta estándar con el resultado de la operación.
	/// - 200: Éxito, retorna <c>data.apiKey</c> visible.
	/// - 400: Error de validación del formulario.
	/// - 401: Credenciales inválidas o cliente no registrado.
	/// </returns>
	/// <response code="200">Operación exitosa</response>
	/// <response code="400">Formulario no válido</response>
	/// <response code="401">No autorizado</response>
	[HttpPost]
	[Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)] 
	public async Task<IActionResult> Post([FromBody] LoginRequest request, CancellationToken cancellationToken)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Password))
		{
			return BadRequest(new ApiResponse<object>
			{
				Success = false,
				Code = CodigosResultado.RESULTADO_FORMULARIO_NO_VALIDO,
				Message = "El formulario contiene errores de validación",
				Errors = new List<string> { "Usuario y Password son obligatorios" }
			});
		}

		var result = await _servicioAutenticacionApiKey.AutenticarYGenerarApiKeyAsync(request.Usuario, request.Password, cancellationToken);
		if (result is null)
		{
			return Unauthorized(new ApiResponse<object>
			{
				Success = false,
				Code = CodigosResultado.RESULTADO_ERROR,
				Message = "Credenciales inválidas o cliente no registrado",
			});
		}

		var (apiKey, cliente, apiKeyVisible) = result.Value;
		return Ok(new ApiResponse<object>
		{
			Success = true,
			Code = CodigosResultado.RESULTADO_EXITOSO,
			Message = "Login exitoso y ApiKey generada",
			Data = new { apiKey = apiKeyVisible }
		});
	}
}
