namespace SimpleSplit.Domain.Base
{
    public abstract class Entity
    {
        public int RowVersion { get; set; } = 1;
    }

    public abstract class Entity<TKey> : Entity where TKey : EntityID
    {
        public abstract TKey ID { get; set; }

        public bool IsNew => ID.Value == default && RowVersion == 1;
    }
}