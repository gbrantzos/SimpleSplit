using System.Linq.Expressions;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    public class ConditionParser : IConditionParser
    {
        private static readonly HashSet<string> KnownOperators = new HashSet<string>
        {
            "eq",      // Equals
            "neq",     // Not Equals
            "lt",      // Less than
            "lte",     // Less than or equal
            "gt",      // Greater than
            "gte",     // Greater than or equal
            "like",    // Like
            "starts",  // Starts with
            "ends",    // Ends with
        };

        public Specification<T> BuildSpecifications<T>(ConditionGroup conditionGroup)
        {
            var type = typeof(T);
            var typeProperties = type.GetProperties();
            var specifications = new List<Specification<T>>();

            if (conditionGroup.Conditions.Count == 0)
                return Specification<T>.True;

            foreach (var condition in conditionGroup.Conditions)
            {
                var propertyName = condition.Property.Split('.').First();
                var prop = Array.Find(typeProperties,
                    p => p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
                if (prop == null) continue;

                if (!KnownOperators.Contains(condition.Operator))
                    throw new ArgumentException($"Unsupported operator: {condition.Operator}");

                var expParam = Expression.Parameter(typeof(T), "p");
                var expMember = propertyName == condition.Property
                    ? Expression.Property(expParam, prop.Name)
                    : GetNestedProperty(expParam, condition.Property);
                var expValue = Expression.Constant(SafeConvert(condition.Value, expMember.Type));

                var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                Expression expBody = condition.Operator switch
                {
                    "eq"     => Expression.Equal(expMember, expValue),
                    "neq"    => Expression.NotEqual(expMember, expValue),
                    "lt"     => Expression.LessThan(expMember, expValue),
                    "lte"    => Expression.LessThanOrEqual(expMember, expValue),
                    "gt"     => Expression.GreaterThan(expMember, expValue),
                    "gte"    => Expression.GreaterThanOrEqual(expMember, expValue),
                    "like"   => Expression.Call(expMember, containsMethod, expValue),
                    "starts" => Expression.Call(expMember, startsWithMethod, expValue),
                    "ends"   => Expression.Call(expMember, endsWithMethod, expValue),
                    _        => throw new ArgumentException($"Unknown operator: {condition.Operator}")
                };
                var result = Expression.Lambda<Func<T, bool>>(expBody, expParam);
                specifications.Add(new Specification<T>(result));
            }

            return conditionGroup.Grouping == ConditionGroup.GroupingOperator.And
                ? SpecificationHelpers.CombineAnd(specifications.ToArray())
                : SpecificationHelpers.CombineOr(specifications.ToArray());
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

        private static object SafeConvert(object value, Type type)
        {
            if (type.IsEnum)
                return Enum.Parse(type, value.ToString());
            return Convert.ChangeType(value, type);
        }
    }
}