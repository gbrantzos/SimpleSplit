using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class LoginUserHandler : Handler<LoginUser, LoginUserResponse>
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;

        public LoginUserHandler(ITokenGenerator tokenGenerator, IUserRepository userRepository)
        {
            _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
        }

        protected override async Task<LoginUserResponse> HandleCore(LoginUser request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByUserName(request.UserName);
            if (user == null || !user.CheckPassword(request.Password))
                return await Failure("Invalid user name or password!");

            var token = _tokenGenerator.CreateToken(user, DateTime.Now);
            return new LoginUserResponse
            {
                UserName = user.Username,
                DisplayName = user.DisplayName,
                Token = token,
            };
        }
    }
}
