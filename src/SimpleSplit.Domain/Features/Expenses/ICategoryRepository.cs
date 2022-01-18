using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Expenses
{
    public interface ICategoryRepository : IRepository<Category, CategoryID>
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of <see cref="Category"/> incatances.</returns>
        Task<IEnumerable<Category>> GetAll();
    }
}
