using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class LoginUserHandler : Handler<LoginUser, LoginUserResponse>
    {
        private readonly ITokenManager _tokenManager;
        private readonly IUserRepository _userRepository;

        public LoginUserHandler(ITokenManager tokenManager, IUserRepository userRepository)
        {
            _tokenManager = tokenManager;
            _userRepository = userRepository;
        }

        protected override async Task<LoginUserResponse> HandleCore(LoginUser request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByUserName(request.UserName, cancellationToken);
            if (user == null)
                return await Failure("Invalid user name or password!");
            if (user.PasswordIsReset)
                return await Failure($"Password is reset or SetPassword was not called for {request.UserName}");
            if (!user.CheckPassword(request.Password))
                return await Failure("Invalid user name or password!");

            var token = _tokenManager.CreateToken(user, DateTime.UtcNow);
            return new LoginUserResponse
            {
                User  = user.ToViewModel(),
                Token = token
            };
        }
    }
}