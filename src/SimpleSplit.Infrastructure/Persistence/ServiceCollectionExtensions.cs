using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleSplit.Infrastructure.Persistence.Base;
using SimpleSplit.Infrastructure.Persistence.Configuration;

namespace SimpleSplit.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityTypeConfigurations(this IServiceCollection services)
            => services.AddEntityTypeConfigurations(typeof(SimpleSplitDbContext).Assembly);

        public static IServiceCollection AddEntityTypeConfigurations(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => !t.IsAbstract
                            && !t.IsGenericTypeDefinition
                            && typeof(EntityTypeConfiguration).IsAssignableFrom(t))
                .ToArray();
            foreach (var type in types)
            {
                services.AddSingleton(typeof(EntityTypeConfiguration), type);
            }

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
            => services.AddRepositories(typeof(SimpleSplitDbContext).Assembly);

        public static IServiceCollection AddRepositories(this IServiceCollection services, params Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.BaseType is {IsGenericType: true}
                            && t.BaseType.GetGenericTypeDefinition() == typeof(GenericRepository<,>))
                .SelectMany(t => t.GetInterfaces(), (t, i) => new {Service = i, Implementation = t})
                .ToArray();
            foreach (var typeInfo in types)
            {
                services.AddScoped(typeInfo.Service, typeInfo.Implementation);
            }

            return services;
        }
    }
}
