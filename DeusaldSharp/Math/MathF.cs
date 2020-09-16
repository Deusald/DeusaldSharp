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

#if NETFRAMEWORK

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

using System;

namespace DeusaldSharp
{
    public static class MathF
    {
        #region Variables

        public const float PI = (float) Math.PI;

        #endregion Variables

        #region Public Methods

        /// <summary> Rounds a value to the nearest integer </summary>
        public static float Round(float value)
        {
            return (float) Math.Round(value);
        }

        /// <summary> Returns the largest integral value less than or equal to the specified number. </summary>
        public static float Floor(float value)
        {
            return (float) Math.Floor(value);
        }

        /// <summary> Returns the smallest integral value greater than or equal to the specified number. </summary>
        public static float Ceiling(float value)
        {
            return (float) Math.Ceiling(value);
        }

        /// <summary> Returns a specified number raised to the specified power. </summary>
        public static float Pow(float x, float y)
        {
            return (float) Math.Pow(x, y);
        }

        /// <summary> Returns the square root of a specified number. </summary>
        public static float Sqrt(float x)
        {
            return (float) Math.Sqrt(x);
        }

        /// <summary> Returns the smaller of two numbers. </summary>
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        /// <summary> Returns the larger of two specified numbers. </summary>
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        /// <summary> Returns the absolute value of a specified number. </summary>
        public static float Abs(float a)
        {
            return a >= 0 ? a : -a;
        }
        
        /// <summary> Returns the sine of the specified angle. </summary>
        public static float Sin(float a)
        {
            return (float) Math.Sin(a);
        }

        /// <summary> Returns the cosine of the specified angle. </summary>
        public static float Cos(float a)
        {
            return (float) Math.Cos(a);
        }

        #endregion Public Methods
    }
}
#endif