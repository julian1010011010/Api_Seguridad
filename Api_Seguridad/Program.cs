using Api_Seguridad.Api.Security;
using Api_Seguridad.Application.ApiKeys;
using Api_Seguridad.Infrastructure.ApiKeys;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<IApiKeyFactory, ApiKeyFactory>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IApiKeyRotationService, ApiKeyRotationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Enforce HTTPS
app.UseHttpsRedirection();

// API Key middleware before authorization/endpoint execution
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();