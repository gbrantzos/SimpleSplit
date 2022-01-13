using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    public interface IConditionParser
    {
        Specification<T> ParseConditions<T>(ConditionGroup conditionGroup);
    }
}