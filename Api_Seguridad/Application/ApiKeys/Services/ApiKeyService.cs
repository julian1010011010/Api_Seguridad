using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Domain.ClientesExternos;
using Api_Seguridad.Infrastructure.ApiKeys;
using Api_Seguridad.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Api_Seguridad.Application.ApiKeys;

public sealed class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;
    private readonly IApiKeyFactory _factory;
    private readonly GatewayDbContext _db;

    public ApiKeyService(IApiKeyRepository repository, IApiKeyFactory factory, GatewayDbContext db)
    {
        _repository = repository;
        _factory = factory;
        _db = db;
    }

    public async Task<(ApiKey apiKey, ClienteExterno cliente, string apiKeyVisible)?> AutenticarYGenerarApiKeyAsync(string usuario, string password, CancellationToken cancellationToken = default)
    {
        var hash = ApiKeyHelpers.ComputeSha256Hash(password);
        var cliente = await _db.ClientesExternos.FirstOrDefaultAsync(x => x.Usuario == usuario && x.PasswordHash == hash, cancellationToken);
        if (cliente is null)
        {
            return null;
        }

        if (cliente.IdApiKey.HasValue)
        {
            var existing = await _db.ApiKeys.FirstOrDefaultAsync(x => x.IdApiKey == cliente.IdApiKey.Value, cancellationToken);
            if (existing is not null)
            {
                existing.Estado = false;
                existing.FechaActualizacion = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        var entity = new ApiKey
        {
            Id = Guid.NewGuid(),
            NombreCliente = cliente.Nombre,
            Permisos = "CONSULTA",
            Estado = true,
            FechaCreacion = DateTime.UtcNow.Date
        };

        var apiKeyString = _factory.BuildApiKey(entity);
        entity.Cifrado = ApiKeyHelpers.ComputeSha256Hash(apiKeyString);

        _db.ApiKeys.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        cliente.IdApiKey = entity.IdApiKey;
        cliente.FechaActualizacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return (entity, cliente, apiKeyString);
    }

    public async Task<ApiKeyValidationResult> BuscarPorApiKeyVisibleAsync(string apiKeyVisible, string requiredPermission, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKeyVisible))
        {
            return new ApiKeyValidationResult { IsValid = false, Error = "API Key es obligatoria" };
        }

        // 1. Hashear con SHA-256 y buscar por hash
        var hash = ApiKeyHelpers.ComputeSha256Hash(apiKeyVisible);
        var entity = await _repository.GetByHashAsync(hash, cancellationToken);

        if (entity is null)
        {
            return new ApiKeyValidationResult { IsValid = false, Error = "API Key no encontrada" };
        }

        // 2. Validar estado activo
        if (!entity.Estado)
        {
            return new ApiKeyValidationResult { IsValid = false, Error = "API Key inactiva" };
        }

        // 3. Validar expiración
        if (entity.FechaExpiracion.HasValue && entity.FechaExpiracion.Value < DateTime.UtcNow)
        {
            return new ApiKeyValidationResult { IsValid = false, Error = "API Key expirada" };
        }

        // 4. Validar permiso solicitado
        if (!string.IsNullOrWhiteSpace(requiredPermission))
        {
            var scopes = (entity.Permisos ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!scopes.Contains(requiredPermission))
            {
                return new ApiKeyValidationResult { IsValid = false, Error = $"API Key no tiene el permiso '{requiredPermission}'" };
            }
        }

        return new ApiKeyValidationResult { IsValid = true, ApiKey = entity };
    }
}
