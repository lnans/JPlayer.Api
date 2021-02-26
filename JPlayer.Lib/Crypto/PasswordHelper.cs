using System.Security.Cryptography;
using System.Text;

namespace JPlayer.Lib.Crypto
{
    public static class PasswordHelper
    {
        public static string Crypt(string salt, string password)
        {
            string saltedPassword = salt + "#" + password;
            return Sha512(saltedPassword);
        }

        public static bool Check(string salt, string givenPassword, string cryptedPassword) =>
            Crypt(salt, givenPassword) == cryptedPassword;

        private static string Sha512(string value)
        {
            using SHA512 hash = SHA512.Create();
            StringBuilder res = new();
            byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            foreach (byte b in result)
                res.Append(b.ToString("x2"));

            return res.ToString();
        }
    }
}