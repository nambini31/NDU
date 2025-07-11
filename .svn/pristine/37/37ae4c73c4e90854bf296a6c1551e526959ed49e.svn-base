using System;
using System.Security.Cryptography;
using System.Text;

namespace SAIM.Core.Utilities
{
    public class Encryptor
    {
        public static string HashPassword(string text)
        {
            using var sha256 = SHA512.Create();
            var encodedPassword = Encoding.UTF8.GetBytes(text);
            var bytesFromEncodedPassword = sha256.ComputeHash(encodedPassword);
            return BitConverter.ToString(bytesFromEncodedPassword).Replace("-", string.Empty);
        }
    }
}