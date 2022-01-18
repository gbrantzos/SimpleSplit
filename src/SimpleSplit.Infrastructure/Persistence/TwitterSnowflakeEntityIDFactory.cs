using System.Collections.Concurrent;
using System.Linq.Expressions;
using IdGen;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    public class TwitterSnowflakeEntityIDFactory : IEntityIDFactory
    {
        private static readonly DateTime _epoch = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly IdStructure _structure = new IdStructure(45, 2, 16);
        private static readonly IdGeneratorOptions _options
            = new IdGeneratorOptions(_structure, new DefaultTimeSource(_epoch));
        private static readonly IdGenerator _idGenerator = new IdGenerator(0, _options);

        private readonly ConcurrentDictionary<Type, Func<long, EntityID>> _localCache = new();

        public TEntityID GetID<TEntityID>(long value) where TEntityID : EntityID
            => PrepareID<TEntityID>(value);
        public TEntityID NextID<TEntityID>() where TEntityID : EntityID
            => PrepareID<TEntityID>(_idGenerator.CreateId());

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
        private static Func<TArg, T> CreateCreator<TArg, T>()
        {
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(TArg) });
            var parameter = Expression.Parameter(typeof(TArg), "p");
            var creatorExpression = Expression.Lambda<Func<TArg, T>>(
                Expression.New(constructor, new Expression[] { parameter }), parameter);
            return creatorExpression.Compile();
        }
    }
}
