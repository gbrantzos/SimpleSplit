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
        
        /// <summary>
        /// Get a user by email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> FindByEmail(string email, CancellationToken cancellationToken = default);
    }
}
