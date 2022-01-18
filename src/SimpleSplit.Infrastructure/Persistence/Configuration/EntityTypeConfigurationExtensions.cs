using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleSplit.Infrastructure.Persistence.Configuration
{
    public static class EntityTypeConfigurationExtensions
    {
        public static IServiceCollection AddEntityTypeConfigurations(this IServiceCollection services)
            => AddEntityTypeConfigurations(services, typeof(SimpleSplitDbContext).Assembly);

        public static IServiceCollection AddEntityTypeConfigurations(this IServiceCollection services, params Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && typeof(EntityTypeConfiguration).IsAssignableFrom(t))
                .ToArray();
            foreach (var type in types)
            {
                services.AddSingleton(typeof(EntityTypeConfiguration), type);
            }
            return services;
        }
    }
}
