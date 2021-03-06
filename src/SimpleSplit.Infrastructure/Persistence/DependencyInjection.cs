using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Buildings;
using SimpleSplit.Domain.Features.Common;
using SimpleSplit.Domain.Features.Expenses;
using SimpleSplit.Domain.Features.Security;
using SimpleSplit.Infrastructure.Persistence.Base;
using SimpleSplit.Infrastructure.Persistence.Configuration;
using SimpleSplit.Infrastructure.Persistence.Repositories;

namespace SimpleSplit.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IEntityIDFactory, TwitterSnowflakeEntityIDFactory>();

            // DbContext
            services.AddDbContext<SimpleSplitDbContext>(options =>
            {
                var connectionString = configuration
                    .GetConnectionString(SimpleSplitDbContext.ConnectionString);
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            // Repositories - Unit of Work
            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddRepositories()
                .AddScoped<IImageRepository, ImageRepository>(); // Special case of repository
            
            // Configurations
            services.AddEntityTypeConfigurations();

            return services;
        }
    }
}
