using System.IdentityModel.Tokens.Jwt;

namespace SmartMicrobus.API.Middleware
{
    /// <summary>
    /// Intercepts the initial Hangfire dashboard request containing a JWT in the query string,
    /// stores it in an HttpOnly cookie, and redirects to strip the token from the URL.
    /// Must be registered before UseHangfireDashboard.
    /// </summary>
    public class HangfireTokenCookieMiddleware
    {
        private const string TokenCookieName = "hangfire_token";
        private const string DashboardPath = "/dashboard";

        private readonly RequestDelegate _next;

        public HangfireTokenCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(DashboardPath) &&
                context.Request.Query.ContainsKey("token"))
            {
                var token = context.Request.Query["token"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    DateTimeOffset cookieExpiry = DateTimeOffset.UtcNow.AddMinutes(30); // fallback
                    if (handler.CanReadToken(token))
                    {
                        var jwt = handler.ReadJwtToken(token);
                        if (jwt.ValidTo > DateTime.MinValue)
                        {
                            cookieExpiry = new DateTimeOffset(jwt.ValidTo);
                        }
                    }

                    context.Response.Cookies.Append(TokenCookieName, token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = context.Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        Path = DashboardPath,
                        Expires = cookieExpiry
                    });

                    context.Response.Redirect(DashboardPath);
                    return;
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to register the Hangfire token cookie middleware.
    /// </summary>
    public static class HangfireTokenCookieMiddlewareExtensions
    {
        /// <summary>
        /// Stores the Hangfire dashboard JWT from the query string into an HttpOnly cookie
        /// and redirects to strip the token from the URL.
        /// </summary>
        public static IApplicationBuilder UseHangfireTokenCookie(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HangfireTokenCookieMiddleware>();
        }
    }
}
