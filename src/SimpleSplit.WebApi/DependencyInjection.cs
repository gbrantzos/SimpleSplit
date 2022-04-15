namespace SimpleSplit.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection SetupCors(this IServiceCollection services,
            IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection("CorsSettings").Get<Dictionary<string, string>>();
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