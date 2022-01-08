namespace SimpleSplit.Domain.Base
{
    public abstract class EntityID : ValueObject
    {
        public long IDValue { get; private init; }

        protected EntityID(long id) => IDValue = id;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return IDValue;
        }

        public static TEntityID FromValue<TEntityID>(long value) where TEntityID : EntityID, new()
            => new TEntityID() { IDValue = value };

        public override string ToString() => $"{GetType().Name}: {IDValue}";
    }
}