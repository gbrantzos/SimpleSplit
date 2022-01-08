using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleSplit.Infrastructure.Persistence;

namespace SimpleSplit.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.AddPersistenceServices(configuration);
        }
    }
}
