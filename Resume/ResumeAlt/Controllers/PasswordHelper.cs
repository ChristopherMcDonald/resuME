using System;
using System.Security.Cryptography;
using Resume.Models;

namespace Resume.Controllers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash the specified password.
        /// </summary>
        /// <returns>The hash</returns>
        /// <param name="password">Password</param>
        public static string Hash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Check the specified user and password.
        /// </summary>
        /// <returns>The check.</returns>
        /// <param name="user">User.</param>
        /// <param name="password">Password.</param>
        public static bool Check(User user, string password)
        {
            if (string.IsNullOrEmpty(password) || user == null)
            {
                return false;
            }

            return Check(password, user.PasswordHash);
        }

        /// <summary>
        /// Check the specified password against the hashed.
        /// </summary>
        /// <returns>Wether it hashes to the same value</returns>
        /// <param name="password">Password.</param>
        /// <param name="hashed">Hashed.</param>
        public static bool Check(string password, string hashed)
        {
            // Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(hashed);

            // Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the results
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
