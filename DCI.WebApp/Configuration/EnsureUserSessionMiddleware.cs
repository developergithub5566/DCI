using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DCI.WebApp.Configuration
{
    public class EnsureUserSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public EnsureUserSessionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var isApi = string.Equals(context.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var hasUser = context.Session.GetString("UserManager");
                if (string.IsNullOrEmpty(hasUser))
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    if (isApi)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return; // stop here for AJAX
                    }

                    context.Response.Redirect("/Account/Login?expired=1");
                    return; // IMPORTANT: stop the pipeline after redirect
                }
            }

            await _next(context);
        }
    }

}
