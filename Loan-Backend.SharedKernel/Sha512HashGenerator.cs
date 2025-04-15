using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel
{
    public class Sha512HashGenerator
    {
        /// <summary>
        /// Generates a SHA512 hash for the given plain text.
        /// </summary>
        /// <param name="plainText">The input text to hash.</param>
        /// <returns>A SHA512 hash represented as a hexadecimal string.</returns>
        public static string GenerateHash(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Input cannot be null or empty.", nameof(plainText));

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                byte[] hash = sha512.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2")); // convert byte to hex

                return sb.ToString();
            }
        }
    }

}
