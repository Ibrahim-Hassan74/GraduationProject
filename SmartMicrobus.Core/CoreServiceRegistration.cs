using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartMicrobus.Core.Domain.Options;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.ServiceContracts.Driver;
using SmartMicrobus.Core.ServiceContracts.Drivers;
using SmartMicrobus.Core.ServiceContracts.Report;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.ServiceContracts.Staff;
using SmartMicrobus.Core.ServiceContracts.Stations;
using SmartMicrobus.Core.Services.Account;
using SmartMicrobus.Core.Services.Common;
using SmartMicrobus.Core.Services.Drivers;
using SmartMicrobus.Core.Services.Report;
using SmartMicrobus.Core.Services.Route;
using SmartMicrobus.Core.Services.Staff;
using SmartMicrobus.Core.Services.Stations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SmartMicrobus.Core
{
    public static class CoreServiceRegistration
    {
        public static IServiceCollection ConfigureCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudiences = configuration.GetSection("Jwt:Audiences").Get<List<string>>(),
                    RoleClaimType = ClaimTypes.Role,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = JsonSerializer.Serialize(
                            ApiResponseFactory.Unauthorized("You are not authorized.")
                        );

                        return context.Response.WriteAsync(result);
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy($"${nameof(UserRole.Driver)}", policy =>
                    policy.RequireRole($"${nameof(UserRole.Driver)}"));

                options.AddPolicy($"${nameof(UserRole.Passenger)}", policy =>
                    policy.RequireRole($"${nameof(UserRole.Passenger)}"));

                options.AddPolicy($"${nameof(UserRole.Staff)}", policy =>
                    policy.RequireRole($"${nameof(UserRole.Staff)}"));

                options.AddPolicy($"${nameof(UserRole.Manager)}", policy =>
                    policy.RequireRole($"${nameof(UserRole.Manager)}"));

                options.AddPolicy("NotAuthorized", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });

            services.Configure<WhatsAppSettings>(configuration.GetSection("WhatsAppSettings"));
            services.AddHttpClient<IWhatsAppService, WhatsAppService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IQrTokenService, QrTokenService>();
            services.AddScoped<IRoutesService, RoutesService>();
            services.AddScoped<IFavoriteRouteService, FavoriteRouteService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<DriverDashboardRealtimeService>();
            services.AddScoped<IOsrmRouteService, OsrmRouteService>();
            services.AddScoped<IStationsService, StationsService>();

            return services;
        }
    }
}
