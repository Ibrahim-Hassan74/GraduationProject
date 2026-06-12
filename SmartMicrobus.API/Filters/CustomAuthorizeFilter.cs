using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using System.Security.Claims;

namespace SmartMicrobus.API.Filters
{
    public class CustomAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CustomAuthorizeFilter(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new ObjectResult(ApiResponseFactory.Unauthorized(_localizer["Unauthorized"]))
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (role != UserRole.Passenger.ToString() && 
                role != UserRole.Manager.ToString() && 
                role != UserRole.Admin.ToString())
            {
                context.Result = new ObjectResult(ApiResponseFactory.Forbidden(_localizer["Forbidden"]))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
