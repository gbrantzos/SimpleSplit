using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Security
{
    public interface IUserRepository : IRepository<User, UserID>
    {
        /// <summary>
        /// Get a user by username.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User> FindByUserName(string userName, CancellationToken cancellationToken = default);
    }
}
