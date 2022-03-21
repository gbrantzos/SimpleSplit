﻿namespace SimpleSplit.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection SetupCors(this IServiceCollection services,
            IEnumerable<KeyValuePair<string, string>> corsSettings)
        {
            foreach (var (key, value) in corsSettings)
                services.AddCors(options =>
                {
                    options.AddPolicy(key, policyBuilder =>
                    {
                        policyBuilder
                            .WithOrigins(value)
                            .SetIsOriginAllowed(isOriginAllowed: _ => true)
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .AllowAnyMethod();
                    });
                });

            return services;
        }
    }
}