using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleSplit.Domain.Features.Expenses;
using SimpleSplit.Infrastructure.Persistence.Repositories;

namespace SimpleSplit.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<SimpleSplitDbContext>(options =>
            {
                var connectionString = configuration
                    .GetConnectionString(SimpleSplitDbContext.ConnectionString);
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            // Repositories
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            return services;
        }
    }
}
