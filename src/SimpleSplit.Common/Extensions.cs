using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleSplit.Common
{
    public static class Extensions
    {
        public static JsonSerializerOptions DefaultJsonSerializerSettings = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        /// <summary>
        /// Convert to camel case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        /// <summary>
        /// The JSON representation of given object
        /// </summary>
        /// <param name="object">Object to represent as JSON</param>
        /// <param name="serializerOptions">JSON serializer settings</param>
        /// <returns></returns>
        public static string ToJson(this object @object, JsonSerializerOptions serializerOptions = null)
        {
            var settings = serializerOptions ?? DefaultJsonSerializerSettings;
            return JsonSerializer.Serialize(@object, settings);
        }
    }
}