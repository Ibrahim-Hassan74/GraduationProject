using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;
using SmartMicrobus.Infrastructure.Repository;
using StackExchange.Redis;

namespace SmartMicrobus.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("RedisConnection")!);
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            return services;
        }
    }
}
