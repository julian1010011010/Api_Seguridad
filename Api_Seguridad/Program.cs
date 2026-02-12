using System.Threading.RateLimiting;
using Api_Seguridad.Api.Security;
using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Application.ApiKeys.Services;
using Api_Seguridad.Application.Jwt;
using Api_Seguridad.Infrastructure.ApiKeys;
using Api_Seguridad.Infrastructure.Db;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog: reemplaza el logger por defecto de .NET
builder.Host.UseSerilog((context, loggerConfig) =>
{
	loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.

builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext + Infrastructure / Application registrations
builder.Services.AddDbContext<GatewayDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Gateway"));
});

builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IApiKeyValidator, ApiKeyValidator>();

// Helpers
builder.Services.AddSingleton<IApiKeyFactory, ApiKeyFactory>();

// Unified service
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

// JWT internal token (RSA RS256)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// Rate limiting defensivo para el endpoint de validación de API Key
builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	options.AddFixedWindowLimiter("apikey-validate", limiter =>
	{
		limiter.PermitLimit = 30;
		limiter.Window = TimeSpan.FromMinutes(1);
		limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		limiter.QueueLimit = 0;
	});

	options.OnRejected = async (context, cancellationToken) =>
	{
		context.HttpContext.Response.ContentType = "application/json";
		await context.HttpContext.Response.WriteAsJsonAsync(new
		{
			success = false,
			code = 0,
			message = "Demasiadas solicitudes. Intente nuevamente en unos momentos."
		}, cancellationToken);
	};
});

var app = builder.Build();

// Serilog: log automático de cada request HTTP (método, ruta, status, duración)
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Enforce HTTPS
app.UseHttpsRedirection();

// Rate limiting middleware (antes de auth y endpoints)
app.UseRateLimiter();

// API Key middleware before authorization/endpoint execution
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();