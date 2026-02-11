using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Domain.ClientesExternos;
using Microsoft.EntityFrameworkCore;

namespace Api_Seguridad.Infrastructure.ApiKeys;

// DbContext para el esquema Gateway y la tabla ApiKey
public sealed class GatewayDbContext : DbContext
{
	public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options)
	{
	}

	public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
	public DbSet<ClienteExterno> ClientesExternos => Set<ClienteExterno>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<ApiKey>(entity =>
		{
			entity.ToTable("ApiKey", "Gateway");
			entity.HasKey(e => e.IdApiKey); // PK en la tabla
			entity.Property(e => e.IdApiKey).HasColumnName("IdApiKey");
			entity.Property(e => e.Id).HasColumnName("Id");
			entity.Property(e => e.Cifrado).HasColumnType("char(64)").IsRequired();
			entity.Property(e => e.NombreCliente).HasMaxLength(200).IsRequired();
			entity.Property(e => e.Permisos).HasMaxLength(500).IsRequired();
			entity.Property(e => e.Estado).HasColumnType("bit");
			entity.Property(e => e.FechaCreacion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaUltimaConsulta).HasColumnType("datetime2(3)");
		});

		modelBuilder.Entity<ClienteExterno>(entity =>
		{
			entity.ToTable("ClientesExternos", "Gateway");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id).HasColumnName("Id");
			entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
			entity.Property(e => e.Usuario).HasMaxLength(100).IsRequired();
			entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
			entity.Property(e => e.Estado).HasColumnType("tinyint");
			entity.Property(e => e.IdApiKey).HasColumnName("IdApiKey");
			entity.Property(e => e.FechaCreacion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaUltimoAcceso).HasColumnType("datetime2(3)");
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
			.Where(x => x.Cifrado == hash && x.Estado)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApiKey?> GetByIdAndClientAsync(Guid id, string nombreCliente, CancellationToken cancellationToken = default)
	{
		return await _dbContext.ApiKeys
			.AsNoTracking()
			.Where(x => x.Id == id && x.NombreCliente == nombreCliente && x.Estado)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<ApiKey> CreateAsync(ApiKey apiKey, CancellationToken cancellationToken = default)
	{
		_dbContext.ApiKeys.Add(apiKey);
		await _dbContext.SaveChangesAsync(cancellationToken);
		return apiKey;
	}

	public async Task<ApiKey?> DeactivateAsync(int idApiKey, CancellationToken cancellationToken = default)
	{
		var entity = await _dbContext.ApiKeys.FirstOrDefaultAsync(x => x.IdApiKey == idApiKey, cancellationToken);
		if (entity is null) return null;
		entity.Estado = false;
		entity.FechaActualizacion = DateTime.UtcNow;
		await _dbContext.SaveChangesAsync(cancellationToken);
		return entity;
	}
}
