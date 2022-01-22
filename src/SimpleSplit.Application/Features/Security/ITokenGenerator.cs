using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public interface ITokenGenerator
    {
        string CreateToken(User user, DateTime issueAt);
    }
}