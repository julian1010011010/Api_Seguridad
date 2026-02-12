using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api_Seguridad.Domain.ApiKeys;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api_Seguridad.Application.Jwt;

/// <summary>
/// Genera JWT internos firmados con HMAC-SHA256 a partir de una API Key validada.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
    
    public ValidateApiKeyResponse GenerateToken(ApiKey apiKey, string audience, string permission)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iss, _settings.Issuer),
            new(JwtRegisteredClaimNames.Aud, audience),
            new(JwtRegisteredClaimNames.Sub, apiKey.NombreCliente),
            new("client_id", apiKey.IdApiKey.ToString()),
            new("scope", permission.ToUpperInvariant()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: _signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new ValidateApiKeyResponse
        {
            AccessToken = accessToken,
            ExpiresIn = _settings.ExpirationMinutes * 60
        };
    }
}
