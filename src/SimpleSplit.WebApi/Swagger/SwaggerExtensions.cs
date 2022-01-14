using System.ComponentModel;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Controllers;
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

            // Custom names
            options.CustomSchemaIds(i => i.FriendlyId());

            options.CustomOperationIds(i =>
            {
                var descriptor = (ControllerActionDescriptor)i.ActionDescriptor;
                return $"{descriptor.RouteValues["controller"]}_{descriptor.ActionName}";
            });

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

    public static class TypeHelpers
    {
        public static string FriendlyId(this Type type, bool fullyQualified = false)
        {
            var typeName = fullyQualified
                ? type.FullNameSansTypeArguments().Replace("+", ".")
                : type.NameForType();

            if (type.GetTypeInfo().IsGenericType)
            {
                var genericArgumentIds = type.GetGenericArguments()
                    .Select(t => t.FriendlyId(fullyQualified))
                    .ToArray();

                return new StringBuilder(typeName)
                    .Replace(String.Format("`{0}", genericArgumentIds.Count()), String.Empty)
                    .Append(String.Format(" [{0}]", String.Join(",", genericArgumentIds).TrimEnd(',')))
                    .ToString();
            }

            return typeName;
        }

        private static string FullNameSansTypeArguments(this Type type)
        {
            if (string.IsNullOrEmpty(type.FullName)) return string.Empty;

            var fullName = type.FullName;
            var chopIndex = fullName.IndexOf("[[");
            return (chopIndex == -1) ? fullName : fullName.Substring(0, chopIndex);
        }

        public static string NameForType(this Type type)
            => type
                .GetCustomAttributes(false)
                .OfType<DisplayNameAttribute>()
                .FirstOrDefault()?
                .DisplayName ?? type.Name;
    }
}
