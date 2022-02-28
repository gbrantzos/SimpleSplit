using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class SearchUserByEmailHandler : Handler<SearchUserByEmail, UserViewModel>
    {
        private readonly IUserRepository _userRepository;

        public SearchUserByEmailHandler(IUserRepository userRepository) 
            => _userRepository = userRepository;

        protected override async Task<UserViewModel> HandleCore(SearchUserByEmail request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByEmail(request.Email, cancellationToken);
            return user?.ToViewModel();
        }
    }
}