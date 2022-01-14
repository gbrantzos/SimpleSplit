using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    public interface IConditionParser
    {
        Specification<T> BuildSpecifications<T>(ConditionGroup conditionGroup);
    }
}