namespace Api_Seguridad.Domain.Common;

public static class CodigosResultado
{
    public const int RESULTADO_EXITOSO = 1;
    public const int RESULTADO_ERROR = 0;
    public const int RESULTADO_INDETERMINADO = -1;
    public const int RESULTADO_FORMULARIO_NO_VALIDO = -2;
}

/// <summary>
/// Objeto estándar de respuesta para APIs REST institucionales.
/// </summary>
/// <typeparam name="T">Tipo de datos retornados</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica si la operación fue exitosa a nivel funcional.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Código de resultado institucional.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Mensaje funcional para el consumidor.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Datos retornados por el servicio.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Lista de errores de validación o negocio.
    /// </summary>
    public List<string> Errors { get; set; }

    public ApiResponse()
    {
        Success = false;
        Code = CodigosResultado.RESULTADO_ERROR;
        Message = string.Empty;
        Data = default;
        Errors = new List<string>();
    }
}