using Asp.Versioning;
using Hangfire;
using Hangfire.Console;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using SmartMicrobus.API.Filters;
using SmartMicrobus.API.Identity;
using SmartMicrobus.API.Realtime;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.ServiceContracts.Notification;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.Services.Account;
using SmartMicrobus.Core.Services.Common;
using SmartMicrobus.Infrastructure.Data;
using System.Globalization;
using System.Threading.RateLimiting;

namespace SmartMicrobus.API.StartupExtensions
{
    public static class ConfigureServiceCollection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddSignalR();

            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    policybuilder => policybuilder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());

                //options.AddPolicy("AllowFrontend", policy =>
                //{
                //    policy
                //        .WithOrigins("http://localhost:5500")
                //        .AllowAnyHeader()
                //        .AllowAnyMethod()
                //        .AllowCredentials();
                //});
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite());
            });

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 15,
                            Window = TimeSpan.FromSeconds(1),
                            AutoReplenishment = true,
                            QueueLimit = 0
                        });
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
                    var response = ApiResponseFactory.TooManyRequests(localizer["TooManyRequests"].Value);
                    await context.HttpContext.Response.WriteAsJsonAsync(response);
                };
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var supportedCultures = new[]
            {
                new CultureInfo("ar"),
                new CultureInfo("en")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("ar");

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider(),
                    new QueryStringRequestCultureProvider()
                };
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
            })
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    var response = ApiResponseFactory.BadRequest("Validation failed.", errors);

                    return new BadRequestObjectResult(response);
                };
            });

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IQueueNotificationService, SignalRQueueNotificationService>();

            services.AddScoped<IDashboardNotificationService, SignalRDashboardNotificationService>();

            services.AddScoped<ILocationBroadcastService, LocationBroadcastService>();


            services.AddScoped<IQrTokenService, QrTokenService>();
            services.AddScoped<IRouteTrackingNotificationService, SignalRRouteTrackingNotificationService>();
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(
                    System.IO.Path.Combine(System.AppContext.BaseDirectory, "SmartMicrobus.API.xml"),
                    includeControllerXmlComments: true
                );
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityDefinition("Accept-Language", new OpenApiSecurityScheme
                {
                    Description = "Specify the request language. Supported values: 'ar' or 'en'.",
                    Name = "Accept-Language",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Accept-Language"
                            }
                        },
                        new string[] {}
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SmartMicrobus.API.xml"));
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Smart Microbus Web API", Version = "1.0" });
                options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Smart Microbus Web API", Version = "2.0" });
                options.OperationFilter<AddInternalServerErrorResponseOperationFilter>();
                options.UseInlineDefinitionsForEnums();
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });



            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );

            services.AddEndpointsApiExplorer();

            services.AddHttpClient();

            services.AddHangfire(x =>
                x.UseSqlServerStorage(
                    configuration.GetConnectionString("DefaultConnection")
                )
                .UseConsole());

            services.AddHangfireServer();

            return services;
        }
    }
}
