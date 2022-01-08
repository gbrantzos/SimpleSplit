using System;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public Type EntityType { get; }
        public EntityID ID { get; private set; }

        public EntityNotFoundException(Type entityType, EntityID id)
            : base($"Could not find entity {entityType.Name} with {id}")
        {
            EntityType = entityType;
            // TODO ID = id.ThrowIfNull(nameof(ID));
        }
    }
}