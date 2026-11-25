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
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    // ====================================================================
    // Bit layout (_Value, 1 byte)
    // ====================================================================
    // Bits 0–7 : 8 independent boolean flags (index 0..7)
    // ====================================================================
    [PublicAPI]
    public struct ByteFlagsMemory
    {
        private const int _BIT_COUNT = 8;

        public static ByteFlagsMemory Empty => new(0);
 
        private readonly byte _Value;
 
        public ByteFlagsMemory(byte value) => _Value = value;
 
        public byte Raw => _Value;
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSet(int index)
        {
            CheckIndex(index);
            return (_Value & (1 << index)) != 0;
        }
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteFlagsMemory Set(int index)
        {
            CheckIndex(index);
            return new((byte)(_Value | (1 << index)));
        }
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteFlagsMemory Clear(int index)
        {
            CheckIndex(index);
            return new((byte)(_Value & ~(1 << index)));
        }
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByteFlagsMemory SetTo(int index, bool value) => value ? Set(index) : Clear(index);
 
        public int  FlagsSetCount => BitOps.PopCount(_Value);
        public bool IsEmpty       => _Value == 0;
        public bool IsFull        => _Value == byte.MaxValue;
 
        public static ByteFlagsMemory OneUpToIndex(int index)
        {
            if ((uint)index > _BIT_COUNT) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and {_BIT_COUNT}.");
            byte value = 0;
            for (int x = 0; x < index; ++x)
                value = (byte)(value | (1 << x));
            return new ByteFlagsMemory(value);
        }
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckIndex(int index)
        {
            if ((uint)index >= _BIT_COUNT) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and {_BIT_COUNT - 1}.");
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("[ByteFlagsMemory ");
            for (int x = 7; x >= 0; --x)
                sb.Append(IsSet(x) ? '1' : '0');
            sb.Append($" count={FlagsSetCount}]");
            return sb.ToString();
        }
    }
}