using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Infrastructure.ApiKeys;

namespace Api_Seguridad.Application.ApiKeys.Services;

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
        try 
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new ApiKeyValidationResult
                {
                    IsValid = false,
                    Error = "API key missing"
                };
            }

            var hash = ApiKeyHelpers.ComputeSha256Hash(apiKey);

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
        catch (Exception ex)
        {
            _logger.LogError(ex,"db error");
            return new ApiKeyValidationResult
            {
                IsValid = false,
                Error = "Database error while validating API key"
            };  
        }
       
    }
}
