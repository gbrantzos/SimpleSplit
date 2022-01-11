﻿using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Infrastructure.Persistence
{
    public class SimpleSplitDbContext : DbContext
    {
        // Common columns
        public const string ID = "id";
        public const string CreatedAt = "created_at";
        public const string ModifiedAt = "modified_at";
        public const string RowVersion = "row_version";

        // ConnectionString name
        public const string ConnectionString = "SimpleSplit";

        public DbSet<Expense> Expenses { get; set; }

        public SimpleSplitDbContext(DbContextOptions<SimpleSplitDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SimpleSplitDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var newOrAdded = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Entity &&
                            e.State is EntityState.Added or EntityState.Modified);
            foreach (var entry in newOrAdded)
            {
                // Change created and updated timestamps
                if (entry.State == EntityState.Added)
                    entry.Property(SimpleSplitDbContext.CreatedAt).CurrentValue = DateTime.Now;
                entry.Property(SimpleSplitDbContext.ModifiedAt).CurrentValue = DateTime.Now;
            }

            // Proper ROW_VERSION number for modified entries
            var allEntries = ChangeTracker.Entries<Entity>();
            foreach (var entry in allEntries)
            {
                if (entry.State == EntityState.Modified || entry.Navigations.Any(n => n.IsModified))
                    entry.Entity.RowVersion++;
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}