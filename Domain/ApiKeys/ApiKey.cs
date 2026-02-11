namespace Api_Seguridad.Domain.ApiKeys;

public class ApiKey
{
    public Guid Id { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string Cifrado { get; set; } = string.Empty; // SHA-256 hash
    public string Permisos { get; set; } = string.Empty; // scopes separados por coma, p.ej: CONSULTA,ESCRITURA
    public int Estado { get; set; } // 1 = activo, 0 = inactivo
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaUltimaConsulta { get; set; }
}
