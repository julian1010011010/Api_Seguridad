using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public sealed class ApiKeyValidationResult
{
    public bool IsValid { get; init; }
    public ApiKey? ApiKey { get; init; }
    public string? Error { get; init; }
}
