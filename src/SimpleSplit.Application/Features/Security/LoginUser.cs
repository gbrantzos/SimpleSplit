using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Security
{
    public class LoginUser : Request<LoginUserResponse>
    {
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }

    public class LoginUserResponse
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
    }
}
