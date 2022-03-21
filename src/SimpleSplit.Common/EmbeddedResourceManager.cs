using System.Reflection;

namespace SimpleSplit.Common
{
    public static class EmbeddedResourceManager
    {
        public static string GetText(Assembly assembly, string nameSpace, string name)
        {
            var resourceName = $"{nameSpace}.{name}";
            if (!assembly.GetManifestResourceNames().Contains(resourceName))
                throw new ArgumentException($"Resource '{resourceName}' does not exist!");

            using var stream = assembly.GetManifestResourceStream(resourceName) ??
                throw new NullReferenceException($"Could not read from stream, resource {resourceName}");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Trim();
        }
    }
}