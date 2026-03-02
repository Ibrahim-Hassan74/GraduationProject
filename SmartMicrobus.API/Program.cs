using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using SmartMicrobus.API.Filters;
using SmartMicrobus.API.Hubs;
using SmartMicrobus.API.Realtime;
using SmartMicrobus.Core;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Account;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.Services.Account;
using SmartMicrobus.Infrastructure;
using SmartMicrobus.Infrastructure.Data;
using SmartMicrobus.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

var configPath = Path.Combine(builder.Environment.ContentRootPath, "Configurations");
builder.Configuration
    .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(configPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true);

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policybuilder => policybuilder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
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

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IQueueNotificationService, SignalRQueueNotificationService>();

builder.Services.AddSwaggerGen(options =>
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

builder.Services.AddApiVersioning(options =>
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



builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureInfrastructure(builder.Configuration);

builder.Services.ConfigureCore(builder.Configuration);

builder.Services.AddHttpClient();


var app = builder.Build();

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
    }); 
}

app.UseStaticFiles();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<DriverQueueHub>("/hubs/driver-queue");

app.Run();
