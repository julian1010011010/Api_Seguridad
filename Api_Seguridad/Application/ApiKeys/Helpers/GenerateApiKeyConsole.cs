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

        var hash = ApiKeyHelpers.ComputeSha256Hash(apiKey);

        // TODO: persistir en BD usando tu infraestructura (EF Core / Dapper / SP)
        Console.WriteLine("\nApiKey generada (ENTREGAR AL CLIENTE, NO SE GUARDA EN BD):");
        Console.WriteLine(apiKey);
        Console.WriteLine("\nHash (se guarda en la tabla Gateway.ApiKey.Cifrado):");
        Console.WriteLine(hash);
    }
}
