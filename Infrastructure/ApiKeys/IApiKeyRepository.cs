using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Infrastructure.ApiKeys;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByHashAsync(string hash, CancellationToken cancellationToken = default);
    Task<ApiKey?> GetByIdAndClientAsync(Guid id, string nombreCliente, CancellationToken cancellationToken = default);
    Task<ApiKey> CreateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
}
