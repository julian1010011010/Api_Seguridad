using System.Security.Cryptography;
using System.Text;
using Api_Seguridad.Infrastructure.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys;

public interface IApiKeyValidator
{
    Task<ApiKeyValidationResult> ValidateAsync(string? apiKey, CancellationToken cancellationToken = default);
}

public sealed class ApiKeyValidator : IApiKeyValidator
{
    private readonly IApiKeyRepository _repository;
    private readonly ILogger<ApiKeyValidator> _logger;

    public ApiKeyValidator(IApiKeyRepository repository, ILogger<ApiKeyValidator> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiKeyValidationResult> ValidateAsync(string? apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return new ApiKeyValidationResult
            {
                IsValid = false,
                Error = "API key missing"
            };
        }

        var hash = ComputeSha256Hash(apiKey);

        var entity = await _repository.GetByHashAsync(hash, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("API key no valida");
            return new ApiKeyValidationResult
            {
                IsValid = false,
                Error = "API key invalid"
            };
        }

        return new ApiKeyValidationResult
        {
            IsValid = true,
            ApiKey = entity
        };
    }

    private static string ComputeSha256Hash(string rawData)
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
