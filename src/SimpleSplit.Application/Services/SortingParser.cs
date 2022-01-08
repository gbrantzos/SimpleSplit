using System.Linq.Expressions;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    public class SortingParser : ISortingParser
    {
        public IEnumerable<Sorting<T>> BuildSorting<T>(IEnumerable<string> sortingDetails)
        {
            var toReturn = new List<Sorting<T>>();
            var type = typeof(T);
            var typeProperties = type.GetProperties();
            var sortingTerms = sortingDetails
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Select(s => s.Trim())
                .ToList();
            foreach (var sorting in sortingTerms)
            {
                var direction = sorting[0] == '-'
                    ? Sorting.SortDirection.Descending
                    : Sorting.SortDirection.Ascending;
                var sortingProperty = sorting.TrimStart(new char[] { '-', '+' });
                var prop = Array.Find(typeProperties, p => p.Name.Equals(sortingProperty, StringComparison.CurrentCultureIgnoreCase));
                if (prop == null) continue;

                var expParam = Expression.Parameter(type);
                Expression expProp = Expression.Property(expParam, prop.Name);

                // https://stackoverflow.com/a/8974880/3410871
                if (expProp.Type.IsValueType)
                    expProp = Expression.Convert(expProp, typeof(object));
                var expression = Expression.Lambda<Func<T, object>>(expProp, expParam);

                toReturn.Add(new Sorting<T>()
                {
                    Direction = direction,
                    Expression = expression
                });
            }
            return toReturn;
        }
    }
}
