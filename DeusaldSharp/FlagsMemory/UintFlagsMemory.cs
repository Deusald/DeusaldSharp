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
using GameLogic;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    // ====================================================================
    // Bit layout (_Value, 4 bytes)
    // ====================================================================
    // Bits 0–31 : 32 independent boolean flags (index 0..31)
    // ====================================================================
    [PublicAPI]
    public struct UintFlagsMemory
    {
        public static UintFlagsMemory Empty => new(0u);

        private readonly uint _Value;

        public UintFlagsMemory(uint value) => _Value = value;

        public uint Raw => _Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSet(int index) => (_Value & (1u << index)) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UintFlagsMemory Set(int index) => new(_Value | (1u << index));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UintFlagsMemory Clear(int index) => new(_Value & ~(1u << index));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UintFlagsMemory SetTo(int index, bool value) => value ? Set(index) : Clear(index);

        public int  FlagsSetCount => BitOps.PopCount(_Value);
        public bool IsEmpty       => _Value == 0;
        public bool IsFull        => _Value == uint.MaxValue;

        public static UintFlagsMemory OneUpToIndex(int index)
        {
            uint value = 0;
            for (int x = 0; x < index; ++x)
                value = value | (1u << x);
            return new UintFlagsMemory(value);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("[UintFlagsMemory ");
            for (int x = 31; x >= 0; --x)
                sb.Append(IsSet(x) ? '1' : '0');
            sb.Append($" count={FlagsSetCount}]");
            return sb.ToString();
        }
    }
}