using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Infrastructure.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public sealed class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;
    private readonly IApiKeyFactory _factory;

    public ApiKeyService(IApiKeyRepository repository, IApiKeyFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }

    public async Task<string?> GetApiKeyByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Si no quieres filtrar por nombre de cliente, se puede usar string.Empty
        var entity = await _repository.GetByIdAndClientAsync(id, nombreCliente: string.Empty, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        return _factory.BuildApiKey(entity);
    }

    public async Task<(ApiKey entity, string apiKey)> CreateAsync(string nombreCliente, string? permisos, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.Date; // sin horas/minutos/segundos

		var entity = new ApiKey
		{
			Id = Guid.NewGuid(),
			NombreCliente = nombreCliente,
			Permisos = permisos ?? string.Empty,
			Estado = true,
			FechaCreacion = now
		};

        var apiKeyString = _factory.BuildApiKey(entity);
        entity.Cifrado = _factory.ComputeHash(apiKeyString);

        await _repository.CreateAsync(entity, cancellationToken);

        return (entity, apiKeyString);
    }
}
