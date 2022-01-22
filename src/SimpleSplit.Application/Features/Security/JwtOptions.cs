namespace SimpleSplit.Application.Features.Security
{
    public class JwtOptions
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
    }
}
