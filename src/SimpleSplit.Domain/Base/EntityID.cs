namespace SimpleSplit.Domain.Base
{
    public abstract class EntityID : ValueObject
    {
        public long Value { get; private init; }

        protected EntityID(long id) => Value = id;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => $"{GetType().Name}: {Value}";
    }
}