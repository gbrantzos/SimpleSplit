using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class ChangeUserPasswordHandler : Handler<ChangeUserPassword>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeUserPasswordHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<bool> HandleCore(ChangeUserPassword request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByUserName(request.UserName, cancellationToken);
            if (user == null)
                return await Failure("Invalid user name or password!");

            if (!user.PasswordIsReset && !user.CheckPassword(request.OldPassword))
                return await Failure("Invalid user name or password!");

            user.SetPassword(request.NewPassword);
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}