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

namespace DeusaldSharp
{
    public class MathUtils
    {
        #region Float Utils

        public static float Clamp(float value, float min, float max)
        {
            return MathF.Min(MathF.Max(value, min), max);
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float InverseLerp(float a, float b, float value)
        {
            if (Math.Abs(b - a) < float.Epsilon)
                return a;

            return (value - a) / (b - a);
        }

        public static float RoundToDecimal(float value, int decimalPoint)
        {
            float decimalPow = MathF.Pow(10f, decimalPoint);
            value =  value * decimalPow;
            value =  MathF.Round(value);
            value /= decimalPow;
            return value;
        }

        public static bool IsFloatZero(float value)
        {
            return AreFloatsEquals(value, 0f);
        }

        public static bool AreFloatsEquals(float one, float two)
        {
            return Math.Abs(one - two) < float.Epsilon;
        }

        #endregion Float Utils
    }
}