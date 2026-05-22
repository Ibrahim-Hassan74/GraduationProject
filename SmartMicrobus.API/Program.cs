using Hangfire;
using Microsoft.Extensions.Options;
using SmartMicrobus.API.Hubs;
using SmartMicrobus.API.Middleware;
using SmartMicrobus.API.StartupExtensions;
using SmartMicrobus.Core;
using SmartMicrobus.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configPath = Path.Combine(builder.Environment.ContentRootPath, "Configurations");
builder.Configuration
    .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(configPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true);

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.ConfigureInfrastructure(builder.Configuration);

builder.Services.ConfigureCore(builder.Configuration);

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

//app.UseCors("AllowFrontend");

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseRequestLocalization(localizationOptions);

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.MapHub<DriverQueueHub>("/hubs/driver-queue");

app.MapHub<DriverDashboardHub>("/hubs/driver-dashboard");

app.MapHub<LocationTrackingHub>("/hubs/location-tracking");

app.Run();
