using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Security
{
    public class SearchUserByEmail : Request<UserViewModel>
    {
        public string Email { get; set; }
    }
}