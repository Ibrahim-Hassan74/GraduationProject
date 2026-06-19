using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SmartMicrobus.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _environment;

        public HangfireAuthorizationFilter(IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            _serviceProvider = serviceProvider;
            _environment = environment;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            if (_environment.IsDevelopment())
                return true;

            var httpContext = context.GetHttpContext();

            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return false;

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var isAuthorized = Task.Run(async () =>
                {
                    string? userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId)) return false;

                    var user = await userManager.FindByIdAsync(userId);
                    if (user == null) return false;

                    var isAdmin = await userManager.IsInRoleAsync(user,UserRole.Owner.ToString());
                    return isAdmin;
                }).GetAwaiter().GetResult();

                return isAuthorized;
            }
            catch
            {
                return false;
            }
        }
    }
}
