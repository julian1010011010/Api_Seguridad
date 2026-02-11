using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyFactory
{
    string BuildApiKey(ApiKey entity);
    string ComputeHash(string apiKey);
}
