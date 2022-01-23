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
        /// <summary>
        /// User details
        /// </summary>
        public UserViewModel User { get; set; }
        
        /// <summary>
        /// Security token
        /// </summary>
        public string Token { get; set; }
    }
}