using Api_Seguridad.Domain.ApiKeys;

namespace Api_Seguridad.Infrastructure.ApiKeys;

/// <summary>
/// Contrato de acceso a datos para la entidad <see cref="ApiKey"/> sobre el esquema Gateway.
/// Encapsula las operaciones básicas de consulta y persistencia relacionadas con API Keys.
/// </summary>
public interface IApiKeyRepository
{
	/// <summary>
	/// Obtiene una API Key activa a partir de su hash cifrado almacenado en base de datos.
	/// </summary>
	/// <param name="hash">Valor SHA-256 almacenado en <c>Gateway.ApiKey.Cifrado</c>.</param>
	/// <param name="cancellationToken">Token de cancelación.</param>
	/// <returns>
	/// Instancia de <see cref="ApiKey"/> si existe y está activa; en caso contrario, <c>null</c>.
	/// </returns>
	Task<ApiKey?> GetByHashAsync(string hash, CancellationToken cancellationToken = default);

	/// <summary>
	/// Obtiene una API Key activa por su identificador lógico y nombre de cliente.
	/// </summary>
	/// <param name="id">Identificador lógico (<see cref="ApiKey.Id"/>).</param>
	/// <param name="nombreCliente">Nombre del cliente asociado.</param>
	/// <param name="cancellationToken">Token de cancelación.</param>
	/// <returns>
	/// Instancia de <see cref="ApiKey"/> si coincide con el cliente y está activa; en caso contrario, <c>null</c>.
	/// </returns>
	Task<ApiKey?> GetByIdAndClientAsync(Guid id, string nombreCliente, CancellationToken cancellationToken = default);

	/// <summary>
	/// Crea un nuevo registro de API Key en la base de datos.
	/// </summary>
	/// <param name="apiKey">Entidad de dominio a persistir.</param>
	/// <param name="cancellationToken">Token de cancelación.</param>
	/// <returns>Entidad <see cref="ApiKey"/> persistida, incluyendo su <c>IdApiKey</c> generado.</returns>
	Task<ApiKey> CreateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
}
