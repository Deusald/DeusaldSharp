// MIT License

// DeusaldSharp:
// Copyright (c) 2020 Adam "Deusald" Orliński

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DeusaldSharp
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text.Length < 2) return text.ToLower();

            StringBuilder sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));

            for (int x = 1; x < text.Length; x++)
            {
                char c = text[x];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string HashPassword(this string password)
        {
            byte[] salt;
            RandomNumberGenerator.Create().GetBytes(salt = new byte[16]);
            Rfc2898DeriveBytes pbkdf2    = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA512);
            byte[]             hash      = pbkdf2.GetBytes(20);
            byte[]             hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0,  16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool CompareHashedPasswords(this string enteredPass, string passHash)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(passHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(enteredPass, salt, 100000, HashAlgorithmName.SHA512);
            byte[]             hash   = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }

        public static string Base64Encode(this string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }

        public static string Base64Decode(this string base64Text)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string FirstToUpper(this string text)
        {
            if (text.Length == 0) return text;
            if (text.Length == 1) return char.ToUpper(text[0]).ToString();
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        public static string SplitPascalCase(this string text)
        {
            return Regex.Replace(text, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
        }
    }
}