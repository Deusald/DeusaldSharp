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

// ReSharper disable UnusedMember.Global

using System;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public static class MathUtils
    {
        #region Variables

        /// <summary> Multiply milliseconds by this value to calculate seconds. </summary>
        public const double MILLISECONDS_TO_SECONDS = 1d / 1000d;
        
        /// <summary> Multiply seconds by this value to calculate value in milliseconds. </summary>
        public const int SEC_TO_MILLISECONDS = 1000;

        /// <summary> Multiply minutes by this value to calculate value in seconds. </summary>
        public const int MIN_TO_SEC = 60;

        /// <summary> Multiply hours by this value to calculate value in minutes. </summary>
        public const int HOURS_TO_MIN = 60;

        /// <summary> Multiply days by this value to calculate value in hours. </summary>
        public const int DAYS_TO_HOURS = 24;

        /// <summary> Multiply degrees by this value to calculate value in radians. </summary>
        public const float DEG_TO_RAD = MathF.PI / 180f;

        /// <summary> Multiply radians by this value to calculate value in degrees. </summary>
        public const float RAD_TO_DEG = 180f / MathF.PI;

        /// <summary> Tolerance used by <see cref="AreFloatsEquals"/> and <see cref="IsFloatZero"/>.
        /// Absolute for values with magnitude up to 1, relative to the larger magnitude above that. </summary>
        public const float EPSILON = 1e-6f;

        /// <summary> Squared <see cref="EPSILON"/> value. </summary>
        public const float EPSILON_SQUARE = EPSILON * EPSILON;

        #endregion Variables

        #region Public Methods

        #region Clamp

        /// <summary> Return clamped value between min and max. </summary>
        public static byte Clamp(this byte value, byte min, byte max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static short Clamp(this short value, short min, short max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static ushort Clamp(this ushort value, ushort min, ushort max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static int Clamp(this int value, int min, int max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static uint Clamp(this uint value, uint min, uint max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static long Clamp(this long value, long min, long max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static double Clamp(this double value, double min, double max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static float Clamp(this float value, float min, float max)
        {
            if (min > max) ThrowMinGreaterThanMax(min, max);
            return MathF.Min(MathF.Max(value, min), max);
        }

        private static void ThrowMinGreaterThanMax<T>(T min, T max)
        {
            throw new ArgumentException($"'{min}' cannot be greater than {max}.", nameof(min));
        }

        #endregion Clamp

        #region Lerp

        /// <summary> Linearly interpolates between two values. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static decimal Lerp(decimal a, decimal b, decimal t)
        {
            return a + (b - a) * t;
        }

        /// <summary> Linearly interpolates between two values. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        /// <summary> Linearly interpolates between two values. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        #endregion Lerp

        #region Inverse Lerp

        /// <summary> Calculates the linear parameter t that produces the interpolated value within the range [a, b]. </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="value"> Value between start and end. </param>
        public static decimal InverseLerp(decimal a, decimal b, decimal value)
        {
            if (b == a)
                return decimal.Zero;

            return (value - a) / (b - a);
        }

        /// <summary> Calculates the linear parameter t that produces the interpolated value within the range [a, b]. </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="value"> Value between start and end. </param>
        public static double InverseLerp(double a, double b, double value)
        {
            if (Math.Abs(b - a) < double.Epsilon)
                return 0d;

            return (value - a) / (b - a);
        }

        /// <summary> Calculates the linear parameter t that produces the interpolated value within the range [a, b]. </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="value"> Value between start and end. </param>
        public static float InverseLerp(float a, float b, float value)
        {
            if (MathF.Abs(b - a) < float.Epsilon)
                return 0f;

            return (value - a) / (b - a);
        }

        #endregion Inverse Lerp

        #region Int Utils

        public static bool IsOdd(this int i)  => i % 2 != 0;
        public static bool IsEven(this int i) => i % 2 == 0;

        /// <summary> Mathematical (Euclidean) modulo - the result is always in range [0, |b|). </summary>
        public static int MathMod(this int a, int b)
        {
            if (b == -1) return 0; // int.MinValue % -1 throws OverflowException
            int r = a % b;
            if (r >= 0) return r;
            // r > -|b| here, so adding |b| can't overflow; computed without Math.Abs to support b == int.MinValue.
            return b > 0 ? r + b : r - b;
        }

        #endregion Int Utils
        
        #region Float Utils

        /// <summary> Rounds the value component to given decimal point. </summary>
        public static float RoundToDecimal(this float value, int decimalPoint)
        {
            return MathF.Round(value, decimalPoint, MidpointRounding.ToEven);
        }

        /// <summary> Check that the given value is zero within the <see cref="EPSILON"/> tolerance. </summary>
        public static bool IsFloatZero(this float value)
        {
            return value.AreFloatsEquals(0f);
        }

        /// <summary> Check that the given values are equal within the <see cref="EPSILON"/> tolerance
        /// (absolute for magnitudes up to 1, relative above). Equal infinities are equal, NaN is never equal. </summary>
        public static bool AreFloatsEquals(this float one, float two)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (one == two) return true;
            if (float.IsInfinity(one) || float.IsInfinity(two)) return false;
            float scale = MathF.Max(1f, MathF.Max(MathF.Abs(one), MathF.Abs(two)));
            return MathF.Abs(one - two) <= EPSILON * scale;
        }

        #endregion Float Utils

        #region Bits

        #region Mark bit

        /// <summary> Set state of bits in value based on mask. </summary>
        public static byte MarkBit(this byte value, byte mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= (byte)~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static sbyte MarkBit(this sbyte value, sbyte mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= (sbyte)~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static short MarkBit(this short value, short mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= (short)~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static ushort MarkBit(this ushort value, ushort mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= (ushort)~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static int MarkBit(this int value, int mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= ~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static uint MarkBit(this uint value, uint mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= ~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static long MarkBit(this long value, long mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= ~mask;
            }

            return value;
        }

        /// <summary> Set state of bits in value based on mask. </summary>
        public static ulong MarkBit(this ulong value, ulong mask, bool state)
        {
            if (state)
            {
                value |= mask;
            }
            else
            {
                value &= ~mask;
            }

            return value;
        }

        #endregion Mark bit

        #region Number of set bits

        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this byte value)
        {
            ushort       count = 0;
            const ushort one   = 1;
            const ushort zero  = 0;

            while (value > 0)
            {
                count +=  (value & 1) == 0 ? zero : one;
                value >>= 1;
            }

            return count;
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this sbyte value)
        {
            return unchecked((byte)value).NumberOfSetBits();
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this short value)
        {
            return unchecked((ushort)value).NumberOfSetBits();
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this ushort value)
        {
            ushort       count = 0;
            const ushort one   = 1;
            const ushort zero  = 0;

            while (value > 0)
            {
                count +=  (value & 1) == 0 ? zero : one;
                value >>= 1;
            }

            return count;
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this int value)
        {
            return unchecked((uint)value).NumberOfSetBits();
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this uint value)
        {
            ushort       count = 0;
            const ushort one   = 1;
            const ushort zero  = 0;

            while (value > 0)
            {
                count +=  (value & 1) == 0 ? zero : one;
                value >>= 1;
            }

            return count;
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this long value)
        {
            return unchecked((ulong)value).NumberOfSetBits();
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static ushort NumberOfSetBits(this ulong value)
        {
            ushort       count = 0;
            const ushort one   = 1;
            const ushort zero  = 0;

            while (value > 0)
            {
                count +=  (value & 1) == 0 ? zero : one;
                value >>= 1;
            }

            return count;
        }

        #endregion Number of set bits

        #region Is single bit on

        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this byte value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this sbyte value)
        {
            return unchecked((byte)value).IsSingleBitOn();
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this short value)
        {
            return unchecked((ushort)value).IsSingleBitOn();
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this ushort value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this int value)
        {
            return unchecked((uint)value).IsSingleBitOn();
        }

        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this uint value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this long value)
        {
            return unchecked((ulong)value).IsSingleBitOn();
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(this ulong value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }

        #endregion Is single bit on

        #region Has any bit on

        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this byte value, byte mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this sbyte value, sbyte mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this short value, short mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this ushort value, ushort mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this int value, int mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this uint value, uint mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this long value, long mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(this ulong value, ulong mask)
        {
            return (value & mask) != 0;
        }

        #endregion Has any bit on

        #region Has all bits on

        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this byte value, byte mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this sbyte value, sbyte mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this short value, short mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this ushort value, ushort mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this int value, int mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this uint value, uint mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this long value, long mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(this ulong value, ulong mask)
        {
            return (value & mask) == mask;
        }

        #endregion Has all bits on

        #endregion Bits

        #endregion Public Methods
    }
}