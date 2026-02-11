using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyValidator
{
    Task<ApiKeyValidationResult> ValidateAsync(string? apiKey, CancellationToken cancellationToken = default);
}
