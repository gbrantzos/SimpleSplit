using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    public class ConditionParser : IConditionParser
    {
        private static readonly HashSet<string> KnownOperators = new HashSet<string>
        {
            "eq", // Equals
            "neq", // Not equals
            "lt", // Less than
            "lte", // Less than or equal
            "gt", // Greater than
            "gte", // Greater than or equal
            "like", // Like
            "starts", // Starts with
            "ends", // Ends with
            "in", // In
            "nin" // Not In
        };

        private static readonly MethodInfo StartsWithMethod
            = typeof(string).GetMethod("StartsWith", new[] { typeof(string) }) ??
              throw new Exception("Could not get StartsWith method for String");

        private static readonly MethodInfo EndsWithMethod =
            typeof(string).GetMethod("EndsWith", new[] { typeof(string) })
            ?? throw new Exception("Could not get EndWith method for String");

        private static readonly MethodInfo ContainsMethod =
            typeof(string).GetMethod("Contains", new[] { typeof(string) })
            ?? throw new Exception("Could not get Contains method for String");

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

                var result = condition.Operator == "in" || condition.Operator == "nin"
                    ? MultiExpressionForCondition<T>(condition, prop)
                    : SingleExpressionForCondition<T>(condition, prop);
                specifications.Add(new Specification<T>(result));
            }

            return conditionGroup.Grouping == ConditionGroup.GroupingOperator.And
                ? SpecificationHelpers.CombineAnd(specifications.ToArray())
                : SpecificationHelpers.CombineOr(specifications.ToArray());
        }

        private Expression<Func<T, bool>> MultiExpressionForCondition<T>(Condition condition, PropertyInfo prop)
        {
            var propertyName = condition.Property.Split('.').First();
            var expParam = Expression.Parameter(typeof(T), "p");
            var expMember = propertyName == condition.Property
                ? Expression.Property(expParam, prop.Name)
                : GetNestedProperty(expParam, condition.Property);
            var conditionValues = Regex.Split(condition.Value, @"(?<!\?),");
            if (conditionValues.Length == 0)
                throw new ArgumentException("Condition value is empty!");

            if (typeof(EntityID).IsAssignableFrom(expMember.Type))
            {
                // Due to EF Core shortcomings on using ValueObject as keys we cannot use 'Contains', thus
                // we shall rewrite in as OR clause!
                var expValue = Expression.Constant(
                    InstanceFactory.CreateInstance(expMember.Type, Int64.Parse(conditionValues[0])), expMember.Type);
                var expBodyOr = Expression.Equal(expMember, expValue);
                for (var i = 1; i < conditionValues.Length; i++)
                {
                    expValue = Expression.Constant(InstanceFactory
                        .CreateInstance(expMember.Type, Int64.Parse(conditionValues[i])), expMember.Type);
                    expBodyOr = Expression.OrElse(expBodyOr, Expression.Equal(expMember, expValue));
                }

                return condition.Operator == "nin"
                    ? Expression.Lambda<Func<T, bool>>(Expression.Not(expBodyOr), expParam)
                    : Expression.Lambda<Func<T, bool>>(expBodyOr, expParam);
            }

            var containsMethod = typeof(Enumerable)
                .GetMethods()
                .Where(m => m.Name == "Contains")
                .Single(m => m.GetParameters().Length == 2)
                .MakeGenericMethod(expMember.Type);
            var values = conditionValues.Select(v => Expression.Constant(SafeConvert(v, expMember.Type)));
            var expValueArray = Expression.NewArrayInit(expMember.Type, values);
            var expBody = Expression.Call(null, containsMethod, expValueArray, expMember);

            return condition.Operator == "nin"
                ? Expression.Lambda<Func<T, bool>>(Expression.Not(expBody), expParam)
                : Expression.Lambda<Func<T, bool>>(expBody, expParam);
        }

        private static Expression<Func<T, bool>> SingleExpressionForCondition<T>(Condition condition, PropertyInfo prop)
        {
            var propertyName = condition.Property.Split('.').First();
            var expParam = Expression.Parameter(typeof(T), "p");
            var expMember = propertyName == condition.Property
                ? Expression.Property(expParam, prop.Name)
                : GetNestedProperty(expParam, condition.Property);
            var expValue = typeof(EntityID).IsAssignableFrom(expMember.Type)
                ? Expression.Constant(
                    InstanceFactory.CreateInstance(expMember.Type, Int64.Parse(condition.Value)), expMember.Type)
                : Expression.Constant(SafeConvert(condition.Value, expMember.Type));

            Expression expBody = condition.Operator switch
            {
                "eq" => Expression.Equal(expMember, expValue),
                "neq" => Expression.NotEqual(expMember, expValue),
                "lt" => Expression.LessThan(expMember, expValue),
                "lte" => Expression.LessThanOrEqual(expMember, expValue),
                "gt" => Expression.GreaterThan(expMember, expValue),
                "gte" => Expression.GreaterThanOrEqual(expMember, expValue),
                "like" => Expression.Call(expMember, ContainsMethod, expValue),
                "starts" => Expression.Call(expMember, StartsWithMethod, expValue),
                "ends" => Expression.Call(expMember, EndsWithMethod, expValue),
                _ => throw new ArgumentException($"Unknown operator: {condition.Operator}")
            };
            var result = Expression.Lambda<Func<T, bool>>(expBody, expParam);
            return result;
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
                return Enum.Parse(type, value.ToString() ?? String.Empty);
            return Convert.ChangeType(value, type);
        }
    }
}