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
                var propertyName = sortingProperty.Split('.').First();
                var prop = Array.Find(typeProperties, p => p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
                if (prop == null) continue;

                var expParam = Expression.Parameter(type, "p");
                var expProp = propertyName == sortingProperty
                    ? Expression.Property(expParam, prop.Name)
                    : GetNestedProperty(expParam, sortingProperty);
                
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
        
        private static Expression GetNestedProperty(ParameterExpression param, string propertyName)
        {
            Expression toReturn = param;
            foreach (var member in propertyName.Split('.'))
            {
                toReturn = Expression.PropertyOrField(toReturn, member);
            }
            return toReturn;
        }
    }
}
