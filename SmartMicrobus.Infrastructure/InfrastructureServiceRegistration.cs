using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartMicrobus.Infrastructure.Data;
using StackExchange.Redis;

namespace SmartMicrobus.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("RedisConnection")!);
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            return services;
        }
    }
}
