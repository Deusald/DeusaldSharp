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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeusaldSharp
{
    /// <summary> Extensions for enums. </summary>
    public static class EnumExtensions
    {
        #region Types

        /// <summary> Delegate for getting random number from range. </summary>
        public delegate int Next(int min, int max);

        #endregion Types

        #region Public Methods

        /// <summary> Checks if the [Flag] enum value has only one bit set. </summary>
        public static bool IsSingleFlagOn(this Enum value)
        {
            return value.GetHashCode().IsSingleBitOn();
        }
        
        /// <summary> Return value that have set single flag on from those given. </summary>
        public static T GetRandomFlag<T>(this T value, Next next) where T : Enum
        {
            if (value.IsSingleFlagOn()) return value;

            T[] matching = Enum.GetValues(typeof(T))
                               .Cast<T>()
                               .Where(c => value.HasFlag(c) && c.IsSingleFlagOn())
                               .ToArray();

            return matching[next(0, matching.Length)];
        }
        
        /// <summary> Checks if the [Flag] enum value has set any flag from mask argument. </summary>
        public static bool HasAnyFlag(this Enum value, Enum mask)
        {
            return value.GetHashCode().HasAnyBitOn(mask.GetHashCode());
        }

        /// <summary> Checks if the [Flag] enum value has set all flags from mask argument. </summary>
        public static bool HasAllFlags(this Enum value, Enum mask)
        {
            return value.GetHashCode().HasAllBitsOn(mask.GetHashCode());
        }
        
        /// <summary> Iterate all Enum values </summary>
        public static IEnumerable<T> IterateAllValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        
        /// <summary> Get custom extension that is attached to enum value. </summary>
        public static T GetCustomExtension<T>(this Enum enumValue) where T : Attribute
        {
            return (T)enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttributes(typeof(T)).First();
        }
        
        /// <summary> Get custom extension that is attached to enum value. </summary>
        public static T GetCustomExtension<T>(this Enum enumValue, T defaultValue) where T : Attribute
        {
            MemberInfo? member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (member == null) return defaultValue;
            T? attribute = (T?)member.GetCustomAttributes(typeof(T)).FirstOrDefault();
            if (attribute == null) return defaultValue;
            return attribute;
        }
        
        /// <summary> Get custom extensions that is attached to enum value. </summary>
        public static T[] GetCustomExtensions<T>(this Enum enumValue) where T : Attribute
        {
            return (T[])enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttributes(typeof(T));
        }

        #endregion Public Methods
    }
}