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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace DeusaldSharp
{
    /// <summary> Extensions for enums. </summary>
    [PublicAPI]
    public static class EnumExtensions
    {
        #region Types

        /// <summary> Delegate for getting random number from range. Min is inclusive, max is exclusive. </summary>
        public delegate int Next(int min, int max);

        #endregion Types

        #region Public Methods

        /// <summary>
        /// Reads the raw underlying value of an enum as an ulong, without boxing.
        /// Works for every underlying type from sbyte to ulong.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ulong ToBits<T>(this T value) where T : unmanaged, Enum
        {
            switch (sizeof(T))
            {
                case 1:  return *(byte*)&value;
                case 2:  return *(ushort*)&value;
                case 4:  return *(uint*)&value;
                case 8:  return *(ulong*)&value;
                default: throw new NotSupportedException($"Enum {typeof(T).Name} has an unsupported underlying size of {sizeof(T)} bytes.");
            }
        }

        /// <summary> Builds an enum value from a raw bit pattern, without boxing. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T FromBits<T>(ulong bits) where T : unmanaged, Enum
        {
            T result = default;

            switch (sizeof(T))
            {
                case 1:  *(byte*)&result   = (byte)bits;   break;
                case 2:  *(ushort*)&result = (ushort)bits; break;
                case 4:  *(uint*)&result   = (uint)bits;   break;
                case 8:  *(ulong*)&result  = bits;         break;
                default: throw new NotSupportedException($"Enum {typeof(T).Name} has an unsupported underlying size of {sizeof(T)} bytes.");
            }

            return result;
        }

        /// <summary> Checks if the [Flags] enum value has exactly one bit set. Zero returns false. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleFlagOn<T>(this T value) where T : unmanaged, Enum
        {
            ulong bits = value.ToBits();
            return bits != 0 && (bits & (bits - 1)) == 0;
        }

        /// <summary> Checks if the [Flags] enum value has no bits set. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNoFlagOn<T>(this T value) where T : unmanaged, Enum
        {
            return value.ToBits() == 0;
        }

        /// <summary> Counts how many bits are set in the [Flags] enum value. </summary>
        public static int CountFlags<T>(this T value) where T : unmanaged, Enum
        {
            ulong bits  = value.ToBits();
            int   count = 0;

            while (bits != 0)
            {
                bits &= bits - 1;
                ++count;
            }

            return count;
        }

        /// <summary> Checks if the [Flags] enum value has set any flag from mask argument. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag<T>(this T value, T mask) where T : unmanaged, Enum
        {
            return (value.ToBits() & mask.ToBits()) != 0;
        }

        /// <summary> Checks if the [Flags] enum value has set all flags from mask argument. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags<T>(this T value, T mask) where T : unmanaged, Enum
        {
            ulong maskBits = mask.ToBits();
            return (value.ToBits() & maskBits) == maskBits;
        }

        /// <summary> Returns the value with the given flags cleared. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithoutFlags<T>(this T value, T mask) where T : unmanaged, Enum
        {
            return FromBits<T>(value.ToBits() & ~mask.ToBits());
        }

        /// <summary> Returns the value with the given flags set. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T WithFlags<T>(this T value, T mask) where T : unmanaged, Enum
        {
            return FromBits<T>(value.ToBits() | mask.ToBits());
        }

        /// <summary>
        /// Return value that have set single flag on from those given.
        /// A value with no flags set is returned unchanged.
        /// </summary>
        public static T GetRandomFlag<T>(this T value, Next next) where T : unmanaged, Enum
        {
            if (value.IsSingleFlagOn()) return value;

            int count = value.CountFlags();

            if (count == 0) return value;

            int index = next(0, count);

            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(next), $"Random delegate returned {index}, which is outside the range [0, {count}).");

            ulong bits = value.ToBits();

            for (int i = 0; i < index; ++i)
                bits &= bits - 1;

            return FromBits<T>(bits & (0UL - bits));
        }

        /// <summary>
        /// Iterates the single flags that are set in the given value, lowest bit first.
        /// Allocation free - the returned enumerator is a struct.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FlagEnumerator<T> IterateSetFlags<T>(this T value) where T : unmanaged, Enum
        {
            return new FlagEnumerator<T>(value);
        }

        /// <summary> Iterate all Enum values. The result is computed once per type and cached. </summary>
        public static IReadOnlyList<T> IterateAllValues<T>() where T : unmanaged, Enum
        {
            return EnumCache<T>.AllValues;
        }

        /// <summary>
        /// Iterate every declared value of the enum that has exactly one bit set,
        /// skipping zero, composite values and duplicated aliases. Computed once per type and cached.
        /// </summary>
        public static IReadOnlyList<T> IterateAllSingleFlags<T>() where T : unmanaged, Enum
        {
            return EnumCache<T>.SingleFlags;
        }

        /// <summary> Get custom extension that is attached to enum value. </summary>
        public static TAttribute GetCustomExtension<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            MemberInfo? member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (member == null)
                throw new ArgumentException($"{enumValue.GetType().Name} has no declared member for value {enumValue}.", nameof(enumValue));

            TAttribute? attribute = member.GetCustomAttribute<TAttribute>();

            if (attribute == null)
                throw new ArgumentException($"{enumValue.GetType().Name}.{enumValue} has no {typeof(TAttribute).Name} attached.", nameof(enumValue));

            return attribute;
        }

        /// <summary> Get custom extension that is attached to enum value. </summary>
        public static TAttribute GetCustomExtension<TAttribute>(this Enum enumValue, TAttribute defaultValue) where TAttribute : Attribute
        {
            MemberInfo? member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (member == null) return defaultValue;

            return member.GetCustomAttribute<TAttribute>() ?? defaultValue;
        }

        /// <summary> Get custom extensions that is attached to enum value. </summary>
        public static TAttribute[] GetCustomExtensions<TAttribute>(this Enum enumValue) where TAttribute : Attribute
        {
            MemberInfo? member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (member == null) return Array.Empty<TAttribute>();

            return member.GetCustomAttributes<TAttribute>().ToArray();
        }

        #endregion Public Methods

        #region Private Types

        private static class EnumCache<T> where T : unmanaged, Enum
        {
            public static readonly T[] AllValues;
            public static readonly T[] SingleFlags;

            static EnumCache()
            {
                AllValues = (T[])Enum.GetValues(typeof(T));

                List<T>        singles = new List<T>();
                HashSet<ulong> seen    = new HashSet<ulong>();

                foreach (T value in AllValues)
                {
                    if (!value.IsSingleFlagOn()) continue;
                    if (!seen.Add(value.ToBits())) continue;

                    singles.Add(value);
                }

                SingleFlags = singles.ToArray();
            }
        }

        #endregion Private Types
    }

    /// <summary> Allocation free enumerator over the single flags set in a [Flags] enum value. </summary>
    [PublicAPI]
    public struct FlagEnumerator<T> where T : unmanaged, Enum
    {
        private ulong _Remaining;

        public FlagEnumerator(T value)
        {
            _Remaining = value.ToBits();
            Current    = default;
        }

        public T Current { get; private set; }

        public FlagEnumerator<T> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_Remaining == 0) return false;

            ulong lowest = _Remaining & (0UL - _Remaining);

            Current    =  EnumExtensions.FromBits<T>(lowest);
            _Remaining &= _Remaining - 1;

            return true;
        }
    }
}