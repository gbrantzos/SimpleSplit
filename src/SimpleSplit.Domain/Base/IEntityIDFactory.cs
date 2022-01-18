namespace SimpleSplit.Domain.Base
{
    public interface IEntityIDFactory
    {
        /// <summary>
        /// Create an instance of <typeparamref name="TEntityID"/> with given ID value.
        /// </summary>
        /// <typeparam name="TEntityID"></typeparam>
        /// <param name="value"></param>
        /// <returns>Instance of <typeparamref name="TEntityID"/></returns>
        TEntityID GetID<TEntityID>(long value) where TEntityID : EntityID;

        /// <summary>
        /// Create an instance of <typeparamref name="TEntityID"/> with the next available ID.
        /// </summary>
        /// <typeparam name="TEntityID">Type of EntityID to return</typeparam>
        /// <returns>Instance of <typeparamref name="TEntityID"/></returns>
        TEntityID NextID<TEntityID>() where TEntityID : EntityID;
    }
}
