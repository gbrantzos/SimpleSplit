using System.Text;

namespace SimpleSplit.Application.Services
{
    public class ConditionGroup : Condition
    {
        public enum GroupingOperator { And, Or }
        public GroupingOperator Grouping { get; set; }

        public List<Condition> Conditions { get; set; }

        public ConditionGroup() => Conditions = new List<Condition>();

        public override string Display()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Conditions.Count; i++)
            {
                var condition = Conditions[i];
                if (i > 0)
                    sb.Append($" {Grouping} ");
                sb.Append($"({condition})");
            }
            return sb.ToString();
        }
    }
}