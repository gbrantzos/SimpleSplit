using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SimpleSplit.Application.Behaviors;
using SimpleSplit.Application.Services;

namespace SimpleSplit.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
            services.AddTransient<ISortingParser, SortingParser>();
            services.AddTransient<IConditionParser, ConditionParser>();

            return services;
        }
    }
}
