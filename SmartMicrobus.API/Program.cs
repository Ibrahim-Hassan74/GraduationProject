using SmartMicrobus.API.Hubs;
using SmartMicrobus.API.Middleware;
using SmartMicrobus.API.StartupExtensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SmartMicrobus.API.Filters;
using SmartMicrobus.Core;
using SmartMicrobus.Infrastructure;
using SmartMicrobus.Infrastructure.Data;
using SmartMicrobus.Infrastructure.Repository;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var configPath = Path.Combine(builder.Environment.ContentRootPath, "Configurations");
builder.Configuration
    .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(configPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true);

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.ConfigureInfrastructure(builder.Configuration);

builder.Services.ConfigureCore(builder.Configuration);

builder.Services.AddHttpClient();



builder.Services.AddLocalization();

var supportedCultures = new[]
{
    new CultureInfo("ar"),
    new CultureInfo("en")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new AcceptLanguageHeaderRequestCultureProvider(), 
        new QueryStringRequestCultureProvider()           //culture=ar
    };
});

var app = builder.Build();

var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
    }); 
}
app.UseExceptionHandlingMiddleware();

app.UseStaticFiles();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<DriverQueueHub>("/hubs/driver-queue");

app.Run();
