namespace SimpleSplit.Domain.Base
{
    public abstract class Entity
    {
        public int RowVersion { get; set; }
    }

    public abstract class Entity<TKey> : Entity where TKey : EntityID
    {
        public abstract TKey ID { get; protected set; }

        public bool IsNew => RowVersion == 0;

        protected Entity(TKey id)
        {
            ID = id;
        }
        protected Entity() { }
    }
}