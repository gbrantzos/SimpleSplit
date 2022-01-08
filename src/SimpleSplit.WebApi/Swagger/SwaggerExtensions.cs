using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SimpleSplit.WebApi.Swagger
{
    public static class SwaggerExtensions
    {
        public static SwaggerGenOptions SetupSwagger(this SwaggerGenOptions options)
        {
            // Base information
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SimpleSplit API",
                Version = "v1",
                Description = "Manage building's shared expenses application"
            });
            options.DocumentFilter<LowercaseDocumentFilter>();

            // Add XML documentation
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? String.Empty;
            var xmlFiles = Directory.GetFiles(basePath, "SimpleSplit*.xml", SearchOption.TopDirectoryOnly).ToList();
            xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));

            // Enable examples
            options.ExampleFilters();

            return options;
        }

        public static SwaggerUIOptions SetupSwaggerUI(this SwaggerUIOptions options)
        {
            options.DocumentTitle = "SimpleSplit API";
            options.InjectStylesheet("/swagger/ui/css");
            options.DocExpansion(DocExpansion.Full);
            options.DefaultModelsExpandDepth(-1);

            return options;
        }
    }
}
