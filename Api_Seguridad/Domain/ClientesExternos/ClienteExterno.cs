namespace Api_Seguridad.Domain.ClientesExternos;

/// <summary>
/// Representa un cliente externo autenticable en el sistema y su asociación con una API Key.
/// </summary>
public class ClienteExterno
{
    /// <summary>
    /// Identificador único del cliente externo (GUID).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre descriptivo del cliente externo.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Usuario de acceso configurado para el cliente externo.
    /// </summary>
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña del cliente externo.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Estado del cliente (1 activo, 0 inactivo).
    /// </summary>
    public byte Estado { get; set; } = 1; // tinyint

    /// <summary>
    /// Identificador de la API Key asociada (PK de la tabla Gateway.ApiKey). Puede ser nulo.
    /// </summary>
    public int? IdApiKey { get; set; }

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de última actualización del registro.
    /// </summary>
    public DateTime? FechaActualizacion { get; set; }

    /// <summary>
    /// Fecha del último acceso del cliente externo.
    /// </summary>
    public DateTime? FechaUltimoAcceso { get; set; }

    // Navegación opcional (si quieres modelar la relación en dominio)
    // public ApiKey ApiKey { get; set; } = null!;
}
