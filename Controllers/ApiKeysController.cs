using Api_Seguridad.Application.ApiKeys;
using Microsoft.AspNetCore.Mvc;

namespace Api_Seguridad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _service;

	public ApiKeysController(IApiKeyService service)
    {
		_service = service;
    }

	// GET api/apikeys/{id}
	// Devuelve la api-key construida a partir de NombreCliente-Id-FechaCreacion
	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetApiKey(Guid id, CancellationToken cancellationToken)
    {
		var apiKey = await _service.GetApiKeyByIdAsync(id, cancellationToken);
		if (apiKey is null)
        {
            return NotFound();
        }

		return Ok(new
		{
			apiKey
		});
    }

	// POST api/apikeys
	// Crea un registro para el cliente, genera la api-key y el cifrado y los persiste
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.NombreCliente))
		{
			return BadRequest(new { error = "NombreCliente is required" });
		}

		var (entity, apiKey) = await _service.CreateAsync(request.NombreCliente, request.Permisos, cancellationToken);

		// Devuelve la api-key visible y el id para futuras consultas
		return CreatedAtAction(nameof(GetApiKey), new { id = entity.Id }, new
		{
			id = entity.Id,
			nombreCliente = entity.NombreCliente,
			apiKey
		});
	}
}

public sealed class CreateApiKeyRequest
{
	public string NombreCliente { get; set; } = string.Empty;
	public string? Permisos { get; set; }
}
