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

        public static TEntityID FromValue<TEntityID>(long value) where TEntityID : EntityID, new()
            => new TEntityID() { Value = value };

        public override string ToString() => $"{GetType().Name}: {Value}";
    }
}