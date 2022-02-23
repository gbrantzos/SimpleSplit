using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Common;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class UpdateUserProfileHandler : Handler<UpdateUserProfile>
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserProfileHandler(IUserRepository userRepository,
            IImageRepository imageRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository  = userRepository;
            _imageRepository = imageRepository;
            _unitOfWork      = unitOfWork;
        }

        protected override async Task<bool> HandleCore(UpdateUserProfile request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByUserName(request.UserName, cancellationToken);
            if (user == null)
                return await Failure("Invalid user name or password!");

            user.DisplayName      = request.DisplayName;
            user.Email            = request.Email;
            user.ProfileImagePath = request.FileName;
            user.UseGravatar      = request.UseGravatar;

            if (request.PasswordInfo != null)
            {
                if (!user.PasswordIsReset && !user.CheckPassword(request.PasswordInfo.OldPassword))
                    return await Failure("Invalid user name or password!");

                user.SetPassword(request.PasswordInfo.NewPassword);
            }

            if (request.Image?.Length > 0)
            {
                await _imageRepository
                    .SaveImage<User>(user.ID.Value, request.FileName, request.Image, cancellationToken);
            }

            if (String.IsNullOrEmpty(user.ProfileImagePath))
            {
                await _imageRepository.DeleteImage<User>(user.ID.Value, cancellationToken);
            }

            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}