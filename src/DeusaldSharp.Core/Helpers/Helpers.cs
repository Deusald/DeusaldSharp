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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    /// <summary> Collection of helper methods. </summary>
    [PublicAPI]
    public static class Helpers
    {
        public static bool GetRandomBool()
        {
            return RandomNumberGenerator.GetInt32(2) == 1;
        }

        #region Arguments

        /// <summary> Converts argument at index to T using the invariant culture. Returns defaultValue when index is out of range or conversion fails. </summary>
        public static T TakeSimpleType<T>(this string[] args, int index, T defaultValue) where T : IConvertible
        {
            if (!IsValidIndex(args, index)) return defaultValue;

            try
            {
                return (T)Convert.ChangeType(args[index], typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string TakeUsername(this string[] args, int index, string defaultValue)
        {
            if (!IsValidIndex(args, index)) return defaultValue;
            return int.TryParse(args[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) ? $"user{id}" : args[index];
        }

        /// <summary> Converts argument at index to T using the invariant culture and returns it if it is one of possibleValues, otherwise defaultValue. </summary>
        public static T TakeFromList<T>(this string[] args, int index, List<T> possibleValues, T defaultValue) where T : IConvertible
        {
            if (!IsValidIndex(args, index)) return defaultValue;

            try
            {
                T value = (T)Convert.ChangeType(args[index], typeof(T), CultureInfo.InvariantCulture);
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
            if (!IsValidIndex(args, index)) return defaultValue;
            return args[index];
        }

        /// <summary> Parses argument at index as enum T. For non-[Flags] enums numeric values that don't map to a defined member return defaultValue. </summary>
        public static T TakeEnum<T>(this string[] args, int index, T defaultValue) where T : struct, Enum
        {
            if (!IsValidIndex(args, index)) return defaultValue;
            if (!Enum.TryParse(args[index], out T value)) return defaultValue;
            if (!typeof(T).IsDefined(typeof(FlagsAttribute), false) && !Enum.IsDefined(typeof(T), value)) return defaultValue;
            return value;
        }

        private static bool IsValidIndex(string[] args, int index)
        {
            return index >= 0 && index < args.Length;
        }

        #endregion Arguments

        #region Binary Writer/Reader

        public static byte[] SerializeByBinaryWriter(Action<BinaryWriter> writer)
        {
            using MemoryStream stream      = new MemoryStream();
            using BinaryWriter innerWriter = new BinaryWriter(stream, Encoding.UTF8, false);
            writer(innerWriter);
            return stream.ToArray();
        }

        public static void DeserializeFromBinaryReader(byte[] data, Action<BinaryReader> reader)
        {
            using MemoryStream stream      = new MemoryStream(data);
            using BinaryReader innerReader = new BinaryReader(stream, Encoding.UTF8);
            reader(innerReader);
        }

        #endregion Binary Writer/Reader
    }
}