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

// ReSharper disable InconsistentNaming

using System;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class FlagsMemoryTests
    {
        /// <summary> Valid indexes set, check and clear the right bit. </summary>
        [Test]
        public void FlagsMemory_ValidIndexes_Work()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ByteFlagsMemory.Empty.Set(7).Raw,                      Is.EqualTo(0x80));
                Assert.That(UshortFlagsMemory.Empty.Set(15).Raw,                   Is.EqualTo(0x8000));
                Assert.That(UintFlagsMemory.Empty.Set(31).Raw,                     Is.EqualTo(0x80000000u));
                Assert.That(UlongFlagsMemory.Empty.Set(63).Raw,                    Is.EqualTo(0x8000000000000000ul));
                Assert.That(UintFlagsMemory.Empty.Set(3).IsSet(3),                 Is.True);
                Assert.That(UintFlagsMemory.Empty.Set(3).Clear(3).IsEmpty,         Is.True);
                Assert.That(ByteFlagsMemory.OneUpToIndex(8).IsFull,                Is.True);
                Assert.That(UlongFlagsMemory.OneUpToIndex(64).IsFull,              Is.True);
                Assert.That(UintFlagsMemory.OneUpToIndex(0).IsEmpty,               Is.True);
            });
        }

        /// <summary> Out of range indexes throw instead of silently wrapping or doing nothing. </summary>
        [Test]
        public void FlagsMemory_OutOfRangeIndex_Throws()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => ByteFlagsMemory.Empty.Set(8));
                Assert.Throws<ArgumentOutOfRangeException>(() => ByteFlagsMemory.Empty.Set(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => ByteFlagsMemory.Empty.IsSet(8));
                Assert.Throws<ArgumentOutOfRangeException>(() => ByteFlagsMemory.Empty.Clear(8));
                Assert.Throws<ArgumentOutOfRangeException>(() => ByteFlagsMemory.Empty.SetTo(8, true));
                Assert.Throws<ArgumentOutOfRangeException>(() => ByteFlagsMemory.OneUpToIndex(9));
                Assert.Throws<ArgumentOutOfRangeException>(() => UshortFlagsMemory.Empty.Set(16));
                Assert.Throws<ArgumentOutOfRangeException>(() => UshortFlagsMemory.Empty.IsSet(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => UshortFlagsMemory.OneUpToIndex(17));
                Assert.Throws<ArgumentOutOfRangeException>(() => UintFlagsMemory.Empty.Set(32));
                Assert.Throws<ArgumentOutOfRangeException>(() => UintFlagsMemory.Empty.Set(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => UintFlagsMemory.Empty.Clear(32));
                Assert.Throws<ArgumentOutOfRangeException>(() => UintFlagsMemory.OneUpToIndex(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => UlongFlagsMemory.Empty.Set(64));
                Assert.Throws<ArgumentOutOfRangeException>(() => UlongFlagsMemory.Empty.IsSet(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => UlongFlagsMemory.OneUpToIndex(65));
            });
        }
    }
}
