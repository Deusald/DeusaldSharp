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

using System;

namespace DeusaldSharp
{
    public class MathF
    {
        public static float Round(float value)
        {
            return (float) Math.Round(value);
        }

        public static float Floor(float value)
        {
            return (float) Math.Floor(value);
        }

        public static float Ceiling(float value)
        {
            return (float) Math.Ceiling(value);
        }

        public static float Pow(float x, float y)
        {
            return (float) Math.Pow(x, y);
        }

        public static float Sqrt(float x)
        {
            return (float) Math.Sqrt(x);
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Abs(float a)
        {
            return a >= 0 ? a : -a;
        }
        
        public static float Sin(float a)
        {
            return (float) Math.Sin(a);
        }

        public static float Cos(float a)
        {
            return (float) Math.Cos(a);
        }
    }
}
#endif