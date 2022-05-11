namespace SimpleSplit.Domain.Base.Exceptions
{
    public class ConcurrencyConflictException : Exception
    {
        public Type EntityType { get; }
        public EntityID EntityID { get; }
        public long VersionToUpdate { get; }


        public ConcurrencyConflictException(Entity entityInstance,
            EntityID entityID,
            int versionToUpdate) : this(entityInstance, entityID, versionToUpdate, null)
        {
        }

        public ConcurrencyConflictException(Entity entityInstance,
            EntityID entityID,
            int versionToUpdate,
            Exception innerException) : base(GetMessage(entityID.Value, versionToUpdate), innerException)
        {
            EntityType      = entityInstance.GetType();
            EntityID        = entityID;
            VersionToUpdate = versionToUpdate;
        }

        private static string GetMessage(long id, int version)
            => $"Entity changed by other user/process! [ID: {id} - Request Version: {version}]";
    }
}
