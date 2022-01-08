using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Exceptions;
using SimpleSplit.Infrastructure.Persistence;

namespace SimpleSplit.Infrastructure.Persistence.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SimpleSplitDbContext _dbContext;

        public UnitOfWork(SimpleSplitDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // TODO Revisit, looks like Vendor specific!
                //var mainEntity = ex.Entries[0].Entity as Entity;
                //if (ex.InnerException is SqlException innerException && innerException.Number == 2601)
                //{
                //    var duplicateKeyEx = new DuplicateKeyException(mainEntity?.GetType(),
                //        "Main entity as JSON", // TODO mainEntity.ToJson(),
                //        innerException.Message,
                //        $"Duplicate key on update, entity {mainEntity?.GetType().Name}!{Environment.NewLine}{innerException.Message}",
                //        ex);
                //    throw duplicateKeyEx;
                //}

                // If not handled just re-throw
                throw;
            }
        }
    }
}