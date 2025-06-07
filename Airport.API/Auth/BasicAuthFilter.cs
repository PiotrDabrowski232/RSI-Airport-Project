using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Airport.API.Auth
{
    public class BasicAuthFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encoded = authHeader.Substring("Basic ".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encoded)).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                if (username == "admin" && password == "password")
                {
                    return;
                }
            }

            context.Result = new UnauthorizedResult();
            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
        }
    }
}
