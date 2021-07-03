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

namespace DeusaldSharp
{
    public static class MathUtils
    {
        #region Variables

        /// <summary> Multiply seconds by this value to calculate value in milliseconds. </summary>
        public const int SecToMilliseconds = 1000;
        
        /// <summary> Multiply minutes by this value to calculate value in seconds. </summary>
        public const int MinToSec      = 60;
        
        /// <summary> Multiply hours by this value to calculate value in minutes. </summary>
        public const int HoursToMin = 60;
        
        /// <summary> Multiply days by this value to calculate value in hours. </summary>
        public const int DaysToHours = 24;
        
        /// <summary> Multiply degrees by this value to calculate value in radians. </summary>
        public const float DegToRad      = MathF.PI / 180f;
        
        /// <summary> Multiply radians by this value to calculate value in degrees. </summary>
        public const float RadToDeg      = 180f / MathF.PI;
        
        /// <summary> Squared float epsilon value. </summary>
        public const float EpsilonSquare = float.Epsilon * float.Epsilon;

        #endregion Variables

        #region Public Methods

        #region Float Utils

        /// <summary> Return clamped value between min and max. </summary>
        public static float Clamp(float value, float min, float max)
        {
            return MathF.Min(MathF.Max(value, min), max);
        }

        /// <summary> Return clamped value between min and max. </summary>
        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary> Linearly interpolates between two values. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary> Calculates the linear parameter t that produces the interpolated value within the range [a, b]. </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="value"> Value between start and end. </param>
        public static float InverseLerp(float a, float b, float value)
        {
            if (Math.Abs(b - a) < float.Epsilon)
                return a;

            return (value - a) / (b - a);
        }

        /// <summary> Rounds the value component to given decimal point. </summary>
        public static float RoundToDecimal(float value, int decimalPoint)
        {
            return MathF.Round(value, decimalPoint, MidpointRounding.ToEven);
        }

        /// <summary> Check that the given value is zero by comparing it to the epsilon float value. </summary>
        public static bool IsFloatZero(float value)
        {
            return AreFloatsEquals(value, 0f);
        }

        /// <summary> Check that the given values are equal, taking into account the epsilon float tolerance. </summary>
        public static bool AreFloatsEquals(float one, float two)
        {
            return Math.Abs(one - two) < float.Epsilon;
        }

        #endregion Float Utils
        
        #region Bits

        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static uint NumberOfSetBits(uint value)
        {
            uint count = 0;

            while (value > 0)
            {
                count +=  value & 1;
                value  >>= 1;
            }

            return count;
        }
        
        /// <summary> Check how many individual bits are set in the given value. </summary>
        public static uint NumberOfSetBits(int value)
        {
            int count = 0;

            while (value > 0)
            {
                count +=  value & 1;
                value  >>= 1;
            }

            return (uint) count;
        }

        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(uint value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }
        
        /// <summary> Checks if the value has only one bit set. </summary>
        public static bool IsSingleBitOn(int value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }

        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(uint value, uint mask)
        {
            return (value & mask) != 0;
        }
        
        /// <summary> Checks if the value has set any flag from mask argument. </summary>
        public static bool HasAnyBitOn(int value, int mask)
        {
            return (value & mask) != 0;
        }

        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(uint value, uint mask)
        {
            return (value & mask) == mask;
        }
        
        /// <summary> Checks if the value has set all flags from mask argument. </summary>
        public static bool HasAllBitsOn(int value, int mask)
        {
            return (value & mask) == mask;
        }

        #endregion Bits

        #endregion Public Methods
    }
}