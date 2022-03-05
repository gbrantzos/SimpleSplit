using SimpleSplit.Common;

namespace SimpleSplit.Application.Services
{
    public class Condition
    {
        /// <summary>
        /// The property of the condition, case insensitive
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Operator of the condition.<br/>
        /// Supported operators are 'eq', 'neq', 'lt', 'lte', 'gt', 'gte', 'like', 'starts', 'ends'.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// String representation of value for the condition
        /// </summary>
        public string Value { get; set; }

        public virtual string Display() => $"{Property} {Operator} {Value}";

        /// <summary>
        /// Create a condition from parsing a string. Useful for reading API query string parameters.
        /// <para>
        /// Examples:
        /// <code>
        ///   "amount|gte|50"
        ///   "description|like|Month Expenses"
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="conditionString"></param>
        /// <returns>A <see cref="Condition"/> object instance</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Condition FromString(string conditionString)
        {
            conditionString.ThrowIfEmpty(nameof(conditionString));
            var terms = conditionString
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (terms.Length != 3)
                throw new ArgumentException($"Invalid condition string. {conditionString}");

            return new Condition
            {
                Property = terms[0],
                Operator = terms[1],
                Value = terms[2]
            };
        }
    }
}
