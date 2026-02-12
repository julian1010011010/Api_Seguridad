namespace Api_Seguridad.Domain.ApiKeys;

/// <summary>
/// Contexto mínimo de auditoría enviado por el consumidor.
/// </summary>
public sealed class RequestContext
{
    /// <summary>
    /// Identificador único de la solicitud para correlación de logs.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// IP real del cliente original (el Gateway la reenvía porque él es el intermediario).
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
}
