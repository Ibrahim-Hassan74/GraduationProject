using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Helper;
using System.Net;
using System.Text.Json;

namespace SmartMicrobus.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _host;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment host, IStringLocalizer<SharedResource> localizer)
        {
            _next = next;
            _host = host;
            _localizer = localizer;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                ApplySecurity(httpContext);

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                var response = _host.IsDevelopment() ?
                    ApiResponseFactory.InternalServerError(ex.Message, new List<string> { ex.StackTrace }) :
                    ApiResponseFactory.InternalServerError(_localizer["InternalServerError"]);

                var json = JsonSerializer.Serialize(response);
                await httpContext.Response.WriteAsync(json);
            }
        }

        private void ApplySecurity(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1;mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }

    }
    /// <summary>
    /// Exception Handling Middleware Extensions
    /// </summary>
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Use Exception Handling Middleware
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
