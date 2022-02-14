using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class RefreshTokenHandler : Handler<RefreshToken, RefreshTokenResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityIDFactory _idFactory;
        private readonly ITokenManager _tokenManager;

        public RefreshTokenHandler(IUserRepository userRepository,
            IEntityIDFactory idFactory,
            ITokenManager tokenManager,
            ILogger<RefreshTokenHandler> logger) : base(logger)
        {
            _userRepository = userRepository;
            _idFactory      = idFactory;
            _tokenManager   = tokenManager;
        }

        protected override async Task<RefreshTokenResponse> HandleCore(RefreshToken request,
            CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.JwtToken))
                return await Failure("Token cannot be null!");
            var userID = _tokenManager.ValidateToken(request.JwtToken);
            if (userID == null)
                return await Failure("Token is invalid");

            var user = await _userRepository.GetByID(_idFactory.GetID<UserID>(userID ?? -1), cancellationToken);
            if (user == null)
                return await Failure("Invalid user from token!");
            if (user.PasswordIsReset)
                return await Failure($"Password is reset or SetPassword was not called for {user.Username}");
            
            var token = _tokenManager.CreateToken(user, DateTime.UtcNow);
            return new RefreshTokenResponse
            {
                User  = user.ToViewModel(),
                NewToken = token
            };
        }
    }
}