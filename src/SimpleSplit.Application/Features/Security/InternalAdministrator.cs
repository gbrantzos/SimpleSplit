using SimpleSplit.Domain.Features.Security;

namespace SimpleSplit.Application.Features.Security
{
    public class InternalAdministrator
    {
        private static readonly string InternalPassword;
        static InternalAdministrator() => InternalPassword = User.GeneratePassword(12);

        public string UserName = "SimpleSplit";
        public string DisplayName = "Internal Administrator";
        public string Password => InternalPassword;
    }
}
