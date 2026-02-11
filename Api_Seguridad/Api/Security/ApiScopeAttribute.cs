using Microsoft.AspNetCore.Mvc;

namespace Api_Seguridad.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class ApiScopeAttribute : TypeFilterAttribute
{
    public ApiScopeAttribute(string scope) : base(typeof(ApiScopeFilter))
    {
        Arguments = new object[] { scope };
    }
}
