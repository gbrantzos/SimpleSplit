using SimpleSplit.Common;

namespace SimpleSplit.Domain.Base.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public Type EntityType { get; }
        public EntityID ID { get; private set; }

        public EntityNotFoundException(Type entityType, EntityID id)
            : base($"Could not find entity {entityType.Name} with {id}")
        {
            EntityType = entityType;
            ID = id.ThrowIfNull(nameof(ID));
        }

        private EntityNotFoundException() : base() { }
        public EntityNotFoundException(string message) : base(message) { }
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}