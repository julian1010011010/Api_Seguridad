using Api_Seguridad.Domain.ApiKeys;
using Api_Seguridad.Domain.ClientesExternos;
using Microsoft.EntityFrameworkCore;

namespace Api_Seguridad.Infrastructure.Db;

/// <summary>
/// DbContext para el esquema Gateway y las tablas relacionadas.
/// </summary>
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
			entity.Property(e => e.FechaExpiracion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaUltimaConsulta).HasColumnType("datetime2(3)");
		});

		modelBuilder.Entity<ClienteExterno>(entity =>
		{
			entity.ToTable("ClientesExternos", "Gateway");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id).HasColumnName("Id");
			entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
			entity.Property(e => e.Usuario).HasMaxLength(150).IsRequired();
			entity.Property(e => e.PasswordHash).HasMaxLength(150).IsRequired();
			entity.Property(e => e.Estado).HasColumnType("tinyint");
			entity.Property(e => e.IdApiKey).HasColumnName("IdApiKey");
			entity.Property(e => e.FechaCreacion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2(3)");
			entity.Property(e => e.FechaUltimoAcceso).HasColumnType("datetime2(3)");

			// FK opcional a ApiKey.IdApiKey
			entity.HasOne<ApiKey>()
				.WithMany()
				.HasForeignKey(e => e.IdApiKey)
				.OnDelete(DeleteBehavior.NoAction);
		});
	}
}
