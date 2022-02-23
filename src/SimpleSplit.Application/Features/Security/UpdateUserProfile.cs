using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Security
{
    public class UpdateUserProfile : Request
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool UseGravatar { get; set; }
        public string FileName { get; set; }
        public Byte[] Image { get; set; }
        public ChangePasswordInfo PasswordInfo  { get; set; }
    }
    
    public class ChangePasswordInfo
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}