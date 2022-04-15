namespace SimpleSplit.WebApi
{
    public static class MiddlewareHelpers
    {
        public static IApplicationBuilder UseCors(this IApplicationBuilder builder,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection("CorsSettings").Get<Dictionary<string, string>>();
            foreach (var (key, _) in settings)
            {
                builder.UseCors(key);
            }

            return builder;
        }
    }
}