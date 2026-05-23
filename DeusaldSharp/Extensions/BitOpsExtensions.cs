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

using System.Runtime.CompilerServices;
using JetBrains.Annotations;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace GameLogic
{
    [PublicAPI]
    public static class BitOps
    {
        #if NET8_0_OR_GREATER
        /// <summary>
        /// Returns the number of trailing zero bits in <paramref name="v"/>.
        /// Equivalently, returns the zero-based index of the lowest set bit.
        /// Returns 64 if <paramref name="v"/> is zero.
        /// Delegates to <see cref="BitOperations.TrailingZeroCount(ulong)"/> on .NET 8+.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(ulong v) => BitOperations.TrailingZeroCount(v);

        /// <summary>
        /// Returns the number of trailing zero bits in <paramref name="v"/>.
        /// Equivalently, returns the zero-based index of the lowest set bit.
        /// Returns 32 if <paramref name="v"/> is zero.
        /// Delegates to <see cref="BitOperations.TrailingZeroCount(uint)"/> on .NET 8+.
        /// </summary>
        public static int TrailingZeroCount(uint v) => BitOperations.TrailingZeroCount(v);

        /// <summary>
        /// Returns the number of set bits (population count / Hamming weight) in <paramref name="v"/>.
        /// Delegates to <see cref="BitOperations.PopCount(ulong)"/> on .NET 8+.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(ulong v) => BitOperations.PopCount(v);

        /// <summary>
        /// Returns the number of set bits (population count / Hamming weight) in <paramref name="v"/>.
        /// Delegates to <see cref="BitOperations.PopCount(uint)"/> on .NET 8+.
        /// </summary>
        public static int PopCount(uint v) => BitOperations.PopCount(v);
        #endif

        #if NETSTANDARD2_1
        /// <summary>
        /// Returns the number of trailing zero bits in <paramref name="v"/>.
        /// Equivalently, returns the zero-based index of the lowest set bit.
        /// Returns 64 if <paramref name="v"/> is zero.
        /// Implemented as a software fallback for .NET Standard 2.1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(ulong v)
        {
            if (v == 0) return 64;
            int c = 0;
            while ((v & 1) == 0)
            {
                v >>= 1;
                c++;
            }
            return c;
        }

        /// <summary>
        /// Returns the number of trailing zero bits in <paramref name="v"/>.
        /// Equivalently, returns the zero-based index of the lowest set bit.
        /// Returns 32 if <paramref name="v"/> is zero.
        /// Implemented as a software fallback for .NET Standard 2.1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(uint v)
        {
            if (v == 0) return 32;
            int c = 0;
            while ((v & 1) == 0)
            {
                v >>= 1;
                c++;
            }
            return c;
        }

        /// <summary>
        /// Returns the number of set bits (population count / Hamming weight) in <paramref name="x"/>.
        /// Implemented as a software fallback for .NET Standard 2.1 using Kernighan's bit-clearing trick.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(ulong x)
        {
            int count = 0;
            while (x != 0)
            {
                x &= x - 1UL;
                count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the number of set bits (population count / Hamming weight) in <paramref name="x"/>.
        /// Implemented as a software fallback for .NET Standard 2.1 using Kernighan's bit-clearing trick.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(uint x)
        {
            int count = 0;
            while (x != 0)
            {
                x &= x - 1U;
                count++;
            }
            return count;
        }
        #endif

        /// <summary>
        /// Removes the lowest set bit from <paramref name="value"/> and returns the result.
        /// Also outputs the isolated bit as <paramref name="bit"/> and its zero-based position as <paramref name="index"/>.
        /// Designed for alloc-free bitmask iteration:
        /// <code>
        /// while (mask != 0)
        ///     mask = mask.PopLowestSetBit(out int index, out ulong bit);
        /// </code>
        /// Behavior is undefined when <paramref name="value"/> is zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PopLowestSetBit(this ulong value, out int index, out ulong bit)
        {
            bit   = value & (0UL - value);
            index = TrailingZeroCount(bit);
            return value & (value - 1);
        }

        /// <summary>
        /// Removes the lowest set bit from <paramref name="value"/> and returns the result.
        /// Also outputs the isolated bit as <paramref name="bit"/> and its zero-based position as <paramref name="index"/>.
        /// Designed for alloc-free bitmask iteration:
        /// <code>
        /// while (mask != 0)
        ///     mask = mask.PopLowestSetBit(out int index, out uint bit);
        /// </code>
        /// Behavior is undefined when <paramref name="value"/> is zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PopLowestSetBit(this uint value, out int index, out uint bit)
        {
            bit   = value & (0U - value);
            index = TrailingZeroCount(bit);
            return value & (value - 1);
        }
    }
}