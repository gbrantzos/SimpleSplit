using System.Collections.Concurrent;
using System.Linq.Expressions;
using IdGen;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Infrastructure.Persistence
{
    public class TwitterSnowflakeEntityIDFactory : IEntityIDFactory
    {
        private static readonly DateTime Epoch = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly IdStructure Structure = new IdStructure(45, 2, 16);
        private static readonly IdGeneratorOptions Options
            = new IdGeneratorOptions(Structure, new DefaultTimeSource(Epoch));
        private static readonly IdGenerator IDGenerator = new IdGenerator(0, Options);

        private readonly ConcurrentDictionary<Type, Func<long, EntityID>> _localCache = new();

        public TEntityID GetID<TEntityID>(long value) where TEntityID : EntityID
            => PrepareID<TEntityID>(value);
        public TEntityID NextID<TEntityID>() where TEntityID : EntityID
            => PrepareID<TEntityID>(IDGenerator.CreateId());

        private TEntityID PrepareID<TEntityID>(long value) where TEntityID : EntityID
        {
            if (_localCache.TryGetValue(typeof(TEntityID), out var existing))
                return (TEntityID)existing.Invoke(value);

            var creator = CreateCreator<long, TEntityID>();
            var entityID = creator.Invoke(value);

            _localCache.AddOrUpdate(typeof(TEntityID), creator, (_, _) => creator);

            return entityID;
        }

        // https://stackoverflow.com/a/55138101/3410871
        // https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
        private static Func<TArg, T> CreateCreator<TArg, T>()
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(TArg) });
            if (constructor == null)
                throw new Exception($"Could not get constructor for type {typeof(T)}");
            var parameter = Expression.Parameter(typeof(TArg), "p");
            var creatorExpression = Expression.Lambda<Func<TArg, T>>(
                Expression.New(constructor, new Expression[] { parameter }), parameter);
            return creatorExpression.Compile();
        }
    }
}
