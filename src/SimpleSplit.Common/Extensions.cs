using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleSplit.Common
{
    public static class Extensions
    {
        private static readonly JsonSerializerOptions DefaultJsonSerializerSettings = new JsonSerializerOptions()
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

        // https://stackoverflow.com/a/9314733

        /// <summary>
        /// Get hierarchical structure (parent - child)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="nextItem"></param>
        /// <param name="canContinue"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
                yield return current;
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
            => FromHierarchy(source, nextItem, s => s != null);

        /// <summary>
        /// Get all distinct messages from inner <see cref="Exception"/> hierarchy
        /// </summary>
        /// <param name="x">Root <see cref="Exception"/></param>
        /// <returns><see cref="IEnumerable{sting}"/> with all distinct messages</returns>
        public static IEnumerable<string> GetAllMessages(this Exception x)
            => x.FromHierarchy(x => x.InnerException)
                .Select(x => x.Message)
                .Distinct()
                .ToList();

    }
}