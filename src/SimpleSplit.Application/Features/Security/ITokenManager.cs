using System.Security.Claims;
using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public interface ITokenManager
    {
        /// <summary>
        /// Create token for <see cref="User"/> at specified time.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="issueAt"></param>
        /// <returns></returns>
        string CreateToken(User user, DateTime issueAt);

        /// <summary>
        /// Reads and validates an 'JSON Web Token' (JWT). If token is valid it returns the user ID,
        /// based on <see cref="ClaimsPrincipal"/> from provided <paramref name="token"/>.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        long? ValidateToken(string token);
    }
}