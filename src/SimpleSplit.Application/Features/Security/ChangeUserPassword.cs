using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Security
{
    public class ChangeUserPassword : Request
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}