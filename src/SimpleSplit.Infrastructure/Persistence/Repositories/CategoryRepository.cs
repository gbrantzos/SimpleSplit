using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Features.Expenses;
using SimpleSplit.Infrastructure.Persistence.Base;

namespace SimpleSplit.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category, CategoryID>, ICategoryRepository
    {
        public CategoryRepository(SimpleSplitDbContext dbContext) : base(dbContext)
        {
        }

        protected override Expression<Func<Category, object>> DefaultSort { get; } = category => category.Description;

        public async Task<IEnumerable<Category>> GetAll()
            => await Set.ToListAsync();
    }
}