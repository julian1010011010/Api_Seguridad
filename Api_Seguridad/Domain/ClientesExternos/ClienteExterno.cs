namespace Api_Seguridad.Domain.ClientesExternos;

public class ClienteExterno
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public byte Estado { get; set; } = 1; // tinyint
    public int IdApiKey { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaUltimoAcceso { get; set; }

    // Navegación opcional (si quieres modelar la relación en dominio)
    // public ApiKey ApiKey { get; set; } = null!;
}
