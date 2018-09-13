using System;
using System.Security.Cryptography;
using System.Text;

namespace DNT.IDP.Common
{
    public static class SecurityExtensions
    {
        public static string GetSha256Hash(this string text)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(text);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }
    }
}