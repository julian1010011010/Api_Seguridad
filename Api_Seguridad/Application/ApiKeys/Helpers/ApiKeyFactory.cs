using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public sealed class ApiKeyFactory : IApiKeyFactory
{
    // Construye la API Key visible para el cliente a partir de la entidad
    // Formato: NombreCliente-Id-FechaCreacion(yyyyMMdd)
    public string BuildApiKey(ApiKey entity)
    {
        var fecha = entity.FechaCreacion.ToString("yyyyMMdd");
        return $"{entity.NombreCliente}-{entity.Id}-{fecha}";
    }

    public string ComputeHash(string apiKey)
    {
        return ApiKeyHelpers.ComputeSha256Hash(apiKey);
    }
}
