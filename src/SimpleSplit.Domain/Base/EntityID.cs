using IdGen;

namespace SimpleSplit.Domain.Base
{
    public abstract class EntityID : ValueObject
    {
        private static readonly DateTime _epoch = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly IdStructure _structure = new IdStructure(45, 2, 16);
        private static readonly IdGeneratorOptions _options
            = new IdGeneratorOptions(_structure, new DefaultTimeSource(_epoch));
        private static readonly IdGenerator _idGenerator = new IdGenerator(0, _options);

        public long Value { get; private init; }

        protected EntityID(long id) => Value = id == 0 ? _idGenerator.CreateId() : id;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static TEntityID FromValue<TEntityID>(long value) where TEntityID : EntityID, new()
            => new TEntityID() { Value = value };

        public override string ToString() => $"{GetType().Name}: {Value}";
    }
}