using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.Jwt;

/// <summary>
/// Servicio para la generación de JWT internos firmados con HMAC-SHA256.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Genera un JWT interno firmado con HS256.
    /// </summary>
    ValidateApiKeyResponse GenerateToken(ApiKey apiKey, string audience, string permission);
}
