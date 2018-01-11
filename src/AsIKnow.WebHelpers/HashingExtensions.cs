using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AsIKnow.WebHelpers
{
    public static class HashingExtensions
    {
        public static byte[] Sha512(this byte[] ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            using (SHA512Managed hash = new SHA512Managed())
            {
                return hash.ComputeHash(ext);
            }
        }
        public static Guid GuidFromSha512(this byte[] ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            return Guid.Parse(ext.Sha512().ToHex().Substring(0, 32));
        }
        public static string ToHex(this byte[] ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            return BitConverter.ToString(ext).Replace("-", "");
        }

        public static byte[] FromHexString(this string ext)
        {
            if (ext.Length % 2 != 0)
                throw new ArgumentException($"Not a valid hexadecimal string. Wrong length.", nameof(ext));
            if (Regex.IsMatch(ext, "[^0-9a-fA-F]"))
                throw new ArgumentException($"Not a valid hexadecimal string. Unexpeted characters.", nameof(ext));

            int NumberChars = ext.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(ext.Substring(i, 2), 16);
            return bytes;
        }
    }
}
