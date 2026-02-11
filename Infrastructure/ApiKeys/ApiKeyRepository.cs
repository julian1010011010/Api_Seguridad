using Api_Seguridad.Domain.ApiKeys;
using Microsoft.EntityFrameworkCore;

namespace Api_Seguridad.Infrastructure.ApiKeys;

// DbContext para el esquema Gateway y la tabla ApiKey
public sealed class GatewayDbContext : DbContext
{
	public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options)
	{
	}

	public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<ApiKey>(entity =>
		{
			entity.ToTable("ApiKey", "Gateway");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Cifrado).HasMaxLength(64).IsRequired();
			entity.Property(e => e.NombreCliente).HasMaxLength(200).IsRequired();
			entity.Property(e => e.Permisos).HasMaxLength(500);
		});
	}
}

public class ApiKeyRepository : IApiKeyRepository
{
	private readonly GatewayDbContext _dbContext;

	public ApiKeyRepository(GatewayDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<ApiKey?> GetByHashAsync(string hash, CancellationToken cancellationToken = default)
	{
		// Consulta usando EF Core sobre el hash cifrado
		return await _dbContext.ApiKeys
			.AsNoTracking()
			.Where(x => x.Cifrado == hash && x.Estado == 1)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApiKey?> GetByIdAndClientAsync(Guid id, string nombreCliente, CancellationToken cancellationToken = default)
	{
		return await _dbContext.ApiKeys
			.AsNoTracking()
			.Where(x => x.Id == id && x.NombreCliente == nombreCliente && x.Estado == 1)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApiKey> CreateAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
	{
		_dbContext.ApiKeys.Add(apiKey);
		await _dbContext.SaveChangesAsync(cancellationToken);
		return apiKey;
	}
}
