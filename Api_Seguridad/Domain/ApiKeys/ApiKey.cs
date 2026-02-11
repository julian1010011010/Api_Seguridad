namespace Api_Seguridad.Domain.ApiKeys;

public class ApiKey
{
    public Guid Id { get; set; }
    public int IdApiKey { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string Cifrado { get; set; } = string.Empty; // SHA-256 hash
    public string Permisos { get; set; } = string.Empty; // scopes separados por coma, p.ej: CONSULTA,ESCRITURA
    public bool Estado { get; set; } // bit en BD: true = activo, false = inactivo
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaUltimaConsulta { get; set; }
}
