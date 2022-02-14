using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class JwtTokenManager : ITokenManager
    {
        private readonly JwtOptions _options;

        public JwtTokenManager(JwtOptions options)
        {
            _options = options;
            if (String.IsNullOrEmpty(_options.SecurityKey))
                throw new ArgumentException($"Security key cannot be null!");
        }

        public string CreateToken(User user, DateTime issueAt)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.ID.Value.ToString())
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(_options.Issuer,
                _options.Audience,
                claims,
                expires: issueAt.Add(_options.TokenLifeTime),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public long? ValidateToken(string token)
        {
            // Based on
            // https://jasonwatmore.com/post/2021/06/02/net-5-create-and-validate-jwt-tokens-use-custom-jwt-middleware
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = securityKey,
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidAudience            = _options.Audience,
                    ValidIssuer              = _options.Issuer,
                    ClockSkew                = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Int64.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}