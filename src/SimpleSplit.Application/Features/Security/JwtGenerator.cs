using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SimpleSplit.Application.Features.Security
{
    public class JwtGenerator : ITokenGenerator
    {
        private readonly JwtOptions _options;

        public JwtGenerator(JwtOptions options)
        {
            _options = options;
            if (String.IsNullOrEmpty(_options.SecurityKey))
                throw new ArgumentException($"Security key cannot be null!");
        }

        public string CreateToken(Domain.Features.Security.User user, DateTime issueAt)
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
    }
}