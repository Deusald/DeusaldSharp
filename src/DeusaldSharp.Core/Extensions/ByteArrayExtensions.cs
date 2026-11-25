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
using System.Text;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public static class ByteArrayExtensions
    {
        public static string ByteArrayToHexString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba) hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] HexStringToByteArray(this string hex)
        {
            if (hex == null) throw new ArgumentNullException(nameof(hex));
            if (hex.Length % 2 != 0) throw new FormatException("Hex string must have an even number of characters.");

            byte[] bytes = new byte[hex.Length / 2];
            for (int x = 0; x < bytes.Length; ++x)
                bytes[x] = (byte)((HexCharToNibble(hex[x * 2]) << 4) | HexCharToNibble(hex[x * 2 + 1]));
            return bytes;
        }

        private static int HexCharToNibble(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            throw new FormatException($"Invalid hex character '{c}'.");
        }

        public static string ByteArrayToBase64String(this byte[] ba)
        {
            return Convert.ToBase64String(ba);
        }

        public static byte[] Base64StringToByteArray(this string base64)
        {
            return Convert.FromBase64String(base64);
        }
    }
}