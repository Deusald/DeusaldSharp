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

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public static class StringExtensions
    {
        public static string? GetEnvironmentVariable(this string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }
        
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

        private const int _SALT_SIZE  = 16;
        private const int _HASH_SIZE  = 32; // 256-bit
        private const int _ITERATIONS = 100_000;

        public static string HashPassword(this string password)
        {
            byte[] salt = new byte[_SALT_SIZE];
            RandomNumberGenerator.Fill(salt);

            byte[] hash = new byte[_HASH_SIZE];
            DeriveKey(password, salt, hash);

            byte[] result = new byte[_SALT_SIZE + _HASH_SIZE];
            Buffer.BlockCopy(salt, 0, result, 0,          _SALT_SIZE);
            Buffer.BlockCopy(hash, 0, result, _SALT_SIZE, _HASH_SIZE);

            return Convert.ToBase64String(result);
        }

        public static bool CompareHashedPasswords(this string enteredPass, string passHash)
        {
            byte[] hashBytes = Convert.FromBase64String(passHash);
            if (hashBytes.Length != _SALT_SIZE + _HASH_SIZE)
                return false;

            ReadOnlySpan<byte> salt         = hashBytes.AsSpan(0,          _SALT_SIZE);
            ReadOnlySpan<byte> expectedHash = hashBytes.AsSpan(_SALT_SIZE, _HASH_SIZE);

            byte[] actualHash = new byte[_HASH_SIZE];
            DeriveKey(enteredPass, salt, actualHash);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        private static void DeriveKey(string password, ReadOnlySpan<byte> salt, Span<byte> destination)
        {
            #if NET6_0_OR_GREATER
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                destination,
                _ITERATIONS,
                HashAlgorithmName.SHA512);
            #else
            // netstandard2.1 / older TFMs: no static Pbkdf2 API, use the ctor.
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt.ToArray(), _ITERATIONS, HashAlgorithmName.SHA512);
            byte[]    derived = pbkdf2.GetBytes(destination.Length);
            derived.CopyTo(destination);
            #endif
        }

        public static string ComputeHmacSha256Hex(this string message, string key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            byte[] textBytes = encoding.GetBytes(message);
            byte[] keyBytes  = encoding.GetBytes(key);

            byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
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

        /// <summary>Checks if a string is Null or white space</summary>
        public static bool IsNullOrWhiteSpace(this string? val) => string.IsNullOrWhiteSpace(val);

        /// <summary>Checks if a string is Null or empty</summary>
        public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

        /// <summary>Checks if a string contains null, empty or white space.</summary>
        public static bool IsBlank(this string? val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();

        /// <summary>Checks if a string is null and returns an empty string if it is.</summary>
        public static string OrEmpty(this string? val) => val ?? string.Empty;
    }
}