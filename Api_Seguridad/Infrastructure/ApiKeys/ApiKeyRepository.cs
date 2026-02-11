using Api_Seguridad.Domain.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Api_Seguridad.Infrastructure.Db;

namespace Api_Seguridad.Infrastructure.ApiKeys;

public class ApiKeyRepository : IApiKeyRepository
{
	private readonly GatewayDbContext _dbContext;

	public ApiKeyRepository(GatewayDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<ApiKey?> GetByHashAsync(string hash, CancellationToken cancellationToken = default)
	{
		// Consulta usando EF Core sobre el hash cifrado
		return await _dbContext.ApiKeys
			.AsNoTracking()
			.Where(x => x.Cifrado == hash && x.Estado)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApiKey?> GetByIdAndClientAsync(Guid id, string nombreCliente, CancellationToken cancellationToken = default)
	{
		return await _dbContext.ApiKeys
			.AsNoTracking()
			.Where(x => x.Id == id && x.NombreCliente == nombreCliente && x.Estado)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApiKey> CreateAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
	{
		_dbContext.ApiKeys.Add(apiKey);
		await _dbContext.SaveChangesAsync(cancellationToken);
		return apiKey;
	}

	public async Task<ApiKey?> DeactivateAsync(int idApiKey, CancellationToken cancellationToken = default)
	{
		var entity = await _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.IdApiKey == idApiKey, cancellationToken);
		if (entity is null) return null;
		entity.Estado = false;
		entity.FechaActualizacion = DateTime.UtcNow;
		await _dbContext.SaveChangesAsync(cancellationToken);
		return entity;
	}
}
