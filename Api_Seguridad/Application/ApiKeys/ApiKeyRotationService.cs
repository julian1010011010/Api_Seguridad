using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Domain.ClientesExternos;
using Api_Seguridad.Infrastructure.ApiKeys;
using Microsoft.EntityFrameworkCore;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyRotationService
{
	Task<(ApiKey apiKey, ClienteExterno cliente, string apiKeyVisible)?> RotateOrCreateAsync(string usuario, string passwordHash, CancellationToken cancellationToken = default);
}

public sealed class ApiKeyRotationService : IApiKeyRotationService
{
	private readonly IApiKeyRepository _repository;
	private readonly IApiKeyFactory _factory;
	private readonly GatewayDbContext _db;

	public ApiKeyRotationService(IApiKeyRepository repository, IApiKeyFactory factory, GatewayDbContext db)
	{
		_repository = repository;
		_factory = factory;
		_db = db;
	}

	public async Task<(ApiKey apiKey, ClienteExterno cliente, string apiKeyVisible)?> RotateOrCreateAsync(string usuario, string passwordHash, CancellationToken cancellationToken = default)
	{
		// Buscar cliente externo por usuario y passwordHash
		var cliente = await _db.ClientesExternos.FirstOrDefaultAsync(x => x.Usuario == usuario && x.PasswordHash == passwordHash, cancellationToken);
		if (cliente is null)
		{
			// No se crea automáticamente por seguridad
			return null;
		}

		// Si tiene ApiKey asociada, desactivar
		if (cliente.IdApiKey != 0)
		{
			var existing = await _db.ApiKeys.FirstOrDefaultAsync(x => x.IdApiKey == cliente.IdApiKey, cancellationToken);
			if (existing is not null)
			{
				existing.Estado = false;
				existing.FechaActualizacion = DateTime.UtcNow;
				await _db.SaveChangesAsync(cancellationToken);
			}
		}

		// Crear nueva ApiKey
		var entity = new ApiKey
		{
			Id = Guid.NewGuid(),
			NombreCliente = cliente.Nombre,
			Permisos = "CONSULTA",
			Estado = true,
			FechaCreacion = DateTime.UtcNow.Date
		};

		var apiKeyString = _factory.BuildApiKey(entity);
		entity.Cifrado = _factory.ComputeHash(apiKeyString);

		_db.ApiKeys.Add(entity);
		await _db.SaveChangesAsync(cancellationToken);

		// Relacionar nuevo ApiKey con el cliente
		cliente.IdApiKey = entity.IdApiKey;
		cliente.FechaActualizacion = DateTime.UtcNow;
		await _db.SaveChangesAsync(cancellationToken);

		return (entity, cliente, apiKeyString);
	}
}
