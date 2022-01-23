using System.Security.Cryptography;
using System.Text;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Security
{
    public class UserID : EntityID
    {
        public UserID(long id) : base(id) { }
        public UserID() : base(0) { }
    }

    public class User : Entity<UserID>
    {
        private static string FixedSalt = "K)Mp72@#12&_pQo{";
        public override UserID ID { get; protected set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; protected set; }
        public string PasswordSalt { get; protected set; }
        public bool PasswordIsReset => String.IsNullOrEmpty(PasswordSalt) || String.IsNullOrEmpty(PasswordHash); 

        public User(UserID id) => ID = id;
        protected User() { }

        /// <summary>
        /// Prepare internal fields for storing given password.
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            PasswordSalt = GeneratePassword();
            PasswordHash = ComputeHash(PasswordSalt + password + FixedSalt);
        }

        /// <summary>
        /// Check if given <paramref name="password"/> matches user information.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>True if <paramref name="password"/> matches user information.</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool CheckPassword(string password)
        {
            if (PasswordIsReset)
                throw new ArgumentException($"Password is reset or SetPassword was not called for {ID}");

            var hashedPassword = ComputeHash(PasswordSalt + password + FixedSalt);
            return hashedPassword == PasswordHash;
        }

        /// <summary>
        /// Generate strong cryptographic password.
        /// </summary>
        /// <param name="length">
        /// Length of internal buffer. Length should be multiple of 4, i.e. 4, 8, 32.
        /// For lengths that are not multiples of 4 result will be padded with =.
        /// </param>
        /// <returns></returns>
        public static string GeneratePassword(int length = 64)
        {
            // Buffer length should be multiple of 3, so let's do maths
            // For lenght 64, buffer should be 54 -> (54 / 3) * 4 = 64
            // For lenght 32, buffer should be 24 -> (24 / 3) * 4 = 32
            // For lenght  8, buffer should be  6 -> ( 6 / 3) * 4 =  8
            //
            // 

            var bufferLength = 3 * (int)Math.Floor((double)length / 4);
            var tokenBuffer = RandomNumberGenerator.GetBytes(bufferLength);
            var pass = Convert.ToBase64String(tokenBuffer);
            return pass.Length < length ? pass.PadRight(length, '=') : pass;
        }

        /// <summary>
        /// Reset password. User must set new password using SetPassword method.
        /// </summary>
        public void ResetPassword()
        {
            PasswordSalt = String.Empty;
            PasswordHash = String.Empty;
        }
        
        private static string ComputeHash(string input)
        {
            var buffer = Encoding.UTF8.GetBytes(input);
            using var hashAlgorithm = HashAlgorithm.Create("SHA256");
            if (hashAlgorithm == null)
                throw new Exception("Could not create hash algorithm implementation instance!");
            
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(buffer));
        }
    }
}
