namespace Api_Seguridad.Application.Jwt;

/// <summary>
/// Configuración para la generación de JWT internos firmados con HMAC-SHA256.
/// Se mapea desde la sección "JwtSettings" de appsettings.json.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Clave secreta HMAC (mínimo 32 caracteres).
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emisor del token (claim iss).
    /// </summary>
    public string Issuer { get; set; } = "Api_Seguridad";

    /// <summary>
    /// Tiempo de expiración en minutos (default 5 min para tokens de corta vida).
    /// </summary>
    public int ExpirationMinutes { get; set; } = 5;
}
