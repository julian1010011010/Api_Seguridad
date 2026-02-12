namespace Api_Seguridad.Domain.ApiKeys;

/// <summary>
/// Request para validar una API Key y obtener un JWT interno.
/// </summary>
public sealed class ValidateApiKeyRequest
{
    public string ApiKey { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public RequestContext? Context { get; set; }
}
