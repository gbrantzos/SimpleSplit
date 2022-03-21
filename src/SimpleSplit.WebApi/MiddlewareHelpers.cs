namespace SimpleSplit.WebApi
{
    public static class MiddlewareHelpers
    {
        public static IApplicationBuilder UseCors(this IApplicationBuilder builder,
            IEnumerable<KeyValuePair<string, string>> settings)
        {
            foreach (var (key, _) in settings)
            {
                builder.UseCors(key);
            }

            return builder;
        }
    }
}