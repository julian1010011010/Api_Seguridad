using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyService
{
    Task<string?> GetApiKeyByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(ApiKey entity, string apiKey)> CreateAsync(string nombreCliente, string? permisos, CancellationToken cancellationToken = default);
}
