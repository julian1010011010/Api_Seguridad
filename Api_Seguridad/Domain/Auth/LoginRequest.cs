namespace Api_Seguridad.Domain.Auth;

public sealed class LoginRequest
{
	public string Usuario { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}
