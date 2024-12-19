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
using System.Collections.Generic;
using System.Security.Cryptography;

namespace DeusaldSharp
{
    /// <summary> Collection of helper methods. </summary>
    public static class Helpers
    {
        public static bool GetRandomBool()
        {
            RandomNumberGenerator rng   = RandomNumberGenerator.Create();
            byte[]                bytes = new byte[1];
            rng.GetBytes(bytes);
            return bytes[0] % 2 == 1;
        }

        #region Arguments

        public static T TakeSimpleType<T>(this string[] args, int index, T defaultValue) where T : IConvertible
        {
            if (args.Length <= index) return defaultValue;

            try
            {
                return (T)Convert.ChangeType(args[index], typeof(T));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string TakeUsername(this string[] args, int index, string defaultValue)
        {
            if (args.Length <= index) return defaultValue;
            return int.TryParse(args[index], out int id) ? $"user{id}" : args[index];
        }

        public static T TakeFromList<T>(this string[] args, int index, List<T> possibleValues, T defaultValue) where T : IConvertible
        {
            if (args.Length <= index) return defaultValue;

            try
            {
                T value = (T)Convert.ChangeType(args[index], typeof(T));
                if (possibleValues.Contains(value)) return value;
                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string TakeString(this string[] args, int index, string defaultValue)
        {
            if (args.Length <= index) return defaultValue;
            return args[index];
        }

        public static T TakeEnum<T>(this string[] args, int index, T defaultValue) where T : struct, Enum
        {
            if (args.Length <= index) return defaultValue;
            return Enum.TryParse(args[index], out T value) ? value : defaultValue;
        }

        #endregion Arguments
    }
}