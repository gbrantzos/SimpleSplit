using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleSplit.WebApi.Swagger
{
    // https://github.com/domaindrivendev/Swashbuckle.WebApi/issues/834
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc
                .Paths
                .ToDictionary(entry => LowercaseEverythingButParameters(entry.Key), entry => entry.Value);
            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var (key, value) in paths)
            {
                swaggerDoc.Paths.Add(key, value);
            }
        }

        private static string LowercaseEverythingButParameters(string key) 
            => string.Join('/', key.Split('/').Select(x => x.Contains("{") ? x : x.ToLower()));
    }
}
