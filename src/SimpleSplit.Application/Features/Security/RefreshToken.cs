using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Security
{
    public class RefreshToken : Request<RefreshTokenResponse>
    {
        public string JwtToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public UserViewModel User { get; set; }
        public string NewToken { get; set; }
    }
}