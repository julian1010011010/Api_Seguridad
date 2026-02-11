using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Domain.Auth;
using Api_Seguridad.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api_Seguridad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
	private readonly IApiKeyRotationService _rotationService;

	public LoginController(IApiKeyRotationService rotationService)
	{
		_rotationService = rotationService;
	}

	// POST api/login
	[HttpPost]
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

		var result = await _rotationService.RotateOrCreateAsync(request.Usuario, request.Password, cancellationToken);
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
