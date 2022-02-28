using Microsoft.EntityFrameworkCore;
using SimpleSplit.Application.Features.Security;
using SimpleSplit.Domain.Features.Security;
using SimpleSplit.Infrastructure.Persistence.Base;

namespace SimpleSplit.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User, UserID>, IUserRepository
    {
        private readonly InternalAdministrator _internalAdministrator;

        public UserRepository(SimpleSplitDbContext dbContext, InternalAdministrator internalAdministrator)
            : base(dbContext) => _internalAdministrator = internalAdministrator;

        public async Task<User> FindByUserName(string userName, CancellationToken cancellationToken = default)
        {
            if (userName.Equals(_internalAdministrator.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var internalAdmin = new User(new UserID(-1))
                {
                    Username = _internalAdministrator.UserName,
                    DisplayName = _internalAdministrator.DisplayName,
                };
                internalAdmin.SetPassword(_internalAdministrator.Password);

                return internalAdmin;
            }

            return await Set.FirstOrDefaultAsync(u => u.Username == userName, cancellationToken);
        }

        public async Task<User> FindByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await Set.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
