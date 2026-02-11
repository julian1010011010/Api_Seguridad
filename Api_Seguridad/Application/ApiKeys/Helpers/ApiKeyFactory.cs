using System.Security.Cryptography;
using System.Text;
using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyFactory
{
    string BuildApiKey(ApiKey entity);
    string ComputeHash(string apiKey);
}

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
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
