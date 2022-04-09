using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Base;
using SimpleSplit.Infrastructure.Persistence;
using SimpleSplit.Infrastructure.Persistence.Configuration;

namespace SimpleSplit.IntegrationTests.Infrastructure.Persistence
{
    public class InfrastructureFixture
    {
        public SimpleSplitDbContext DbContext { get; }
        public IEntityIDFactory IDFactory { get; }

        public InfrastructureFixture()
        {
            var idFactory = new TwitterSnowflakeEntityIDFactory();
            var types = typeof(SimpleSplitDbContext).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && typeof(EntityTypeConfiguration).IsAssignableFrom(t))
                .Select(t =>
                {
                    var constructors = t.GetConstructors();
                    var instance = constructors.First().GetParameters().Length == 0
                        ? Activator.CreateInstance(t)
                        : Activator.CreateInstance(t, idFactory);
                    return instance as EntityTypeConfiguration 
                        ?? throw new Exception("Failed to create type configuration");
                })
                .ToArray();
            
            var optionsBuilder = new DbContextOptionsBuilder<SimpleSplitDbContext>();
            var connectionString =
                "server=mysql.gbworks.local; port=3309; user=root; password=devel1; database=split; connect timeout=300";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();

            DbContext = new SimpleSplitDbContext(optionsBuilder.Options, types);
            IDFactory = idFactory;
        }
    }
}