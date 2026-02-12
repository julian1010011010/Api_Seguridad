namespace Api_Seguridad.Domain.ApiKeys;

/// <summary>
/// Respuesta con el JWT interno generado tras validar la API Key.
/// </summary>
public sealed class ValidateApiKeyResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
