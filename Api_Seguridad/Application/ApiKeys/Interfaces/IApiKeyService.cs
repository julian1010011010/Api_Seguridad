using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Domain.ClientesExternos;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyService
{
    Task<(ApiKey apiKey, ClienteExterno cliente, string apiKeyVisible)?> AutenticarYGenerarApiKeyAsync(string usuario, string password, CancellationToken cancellationToken = default);
    Task<ApiKeyValidationResult> BuscarPorApiKeyVisibleAsync(string apiKeyVisible, string requiredPermission, CancellationToken cancellationToken = default);
}
