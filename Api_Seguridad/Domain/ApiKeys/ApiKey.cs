namespace Api_Seguridad.Domain.ApiKeys;

/// <summary>
/// Representa una API Key emitida para un cliente externo y sus metadatos.
/// </summary>
public class ApiKey
{
    /// <summary>
    /// Identificador l�gico (GUID) usado en la construcci�n de la API Key visible.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador interno (PK) de la API Key en la base de datos (Gateway.ApiKey).
    /// </summary>
    public int IdApiKey { get; set; }

    /// <summary>
    /// Nombre del cliente asociado a la API Key.
    /// </summary>
    public string NombreCliente { get; set; } = string.Empty;

    /// <summary>
    /// Hash SHA-256 de la API Key visible (se persiste, no se expone).
    /// </summary>
    public string Cifrado { get; set; } = string.Empty; // SHA-256 hash

    /// <summary>
    /// Permisos (scopes) separados por coma, por ejemplo: CONSULTA,ESCRITURA.
    /// </summary>
    public string Permisos { get; set; } = string.Empty; // scopes separados por coma, p.ej: CONSULTA,ESCRITURA

    /// <summary>
    /// Estado de la API Key (true = activo, false = inactivo).
    /// </summary>
    public bool Estado { get; set; } // bit en BD: true = activo, false = inactivo

    /// <summary>
    /// Fecha de creaci�n de la API Key.
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de última actualización de la API Key.
    /// </summary>
    public DateTime? FechaActualizacion { get; set; }

    /// <summary>
    /// Fecha de expiración de la API Key (null = no expira).
    /// </summary>
    public DateTime? FechaExpiracion { get; set; }

    /// <summary>
    /// Fecha de la última validación/consulta realizada con esta API Key.
    /// </summary>
    public DateTime? FechaUltimaConsulta { get; set; }
}
