using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class UserViewModel : ViewModel
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ProfileImagePath { get; set; }
        public bool HasProfileImage => !String.IsNullOrEmpty(ProfileImagePath);
        public bool UseGravatar { get; set; }
    }

    public static class ExpenseViewModelExtensions
    {
        public static UserViewModel ToViewModel(this User domainObject)
        {
            return new UserViewModel
            {
                ID               = domainObject.ID.Value,
                RowVersion       = domainObject.RowVersion,
                UserName         = domainObject.Username,
                DisplayName      = domainObject.DisplayName,
                Email            = domainObject.Email,
                ProfileImagePath = domainObject.ProfileImagePath,
                UseGravatar      = domainObject.UseGravatar
            };
        }
    }
}