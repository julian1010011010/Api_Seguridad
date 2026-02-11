using System.Security.Cryptography;
using System.Text;

namespace Api_Seguridad.Application.ApiKeys;

public static class GenerateApiKeyConsole
{
    public static void Run()
    {
        Console.WriteLine("Nombre del cliente:");
        var nombreCliente = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Permisos (scope, por ejemplo: CONSULTA):");
        var permisos = Console.ReadLine() ?? string.Empty;

        var empresa = "EMPRESA"; // ajustar si necesitas multi-empresa
        var codigo = nombreCliente.Replace(" ", string.Empty).ToUpperInvariant();
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd");
        var guid = Guid.NewGuid().ToString("N");

        var apiKey = $"{empresa}-{codigo}-{fecha}-{guid}";

        var hash = ComputeSha256Hash(apiKey);

        // TODO: persistir en BD usando tu infraestructura (EF Core / Dapper / SP)
        Console.WriteLine("\nApiKey generada (ENTREGAR AL CLIENTE, NO SE GUARDA EN BD):");
        Console.WriteLine(apiKey);
        Console.WriteLine("\nHash (se guarda en la tabla Gateway.ApiKey.Cifrado):");
        Console.WriteLine(hash);
    }

    public static string ComputeSha256Hash(string rawData)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
