using System.Security.Cryptography;
using System.Text;

namespace MrLMS.Helper
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password, Guid salt)
        {
            using var sha256 = SHA256.Create();
            var combined = password + salt.ToString();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string password, Guid salt, string storedHash)
        {
            var hash = HashPassword(password, salt);
            return hash == storedHash;
        }
    }
}