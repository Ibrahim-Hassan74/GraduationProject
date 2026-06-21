using Hangfire.Dashboard;
using SmartMicrobus.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SmartMicrobus.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            try
            {
                // HttpContext.User is already populated by the JWT Bearer middleware
                // (via OnMessageReceived reading from query string or cookie).
                if (!httpContext.User.Identity?.IsAuthenticated ?? true)
                    return false;

                return httpContext.User.IsInRole(UserRole.Admin.ToString());
            }
            catch
            {
                return false;
            }
        }
    }
}
