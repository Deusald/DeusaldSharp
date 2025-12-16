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
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Local

using System;
using DeusaldSharp;
using NUnit.Framework;


namespace DeusaldSharpTests
{
    public class ExtensionTests
    {
        [Flags]
        private enum TestFlags
        {
            None = 0,
            A    = 1 << 0,
            B    = 1 << 1,
            C    = 1 << 2
        }

        /// <summary> Testing IsSingleFlagOn method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.IsSingleFlagOn))]
        public void IsSingleFlagOn_Base()
        {
            // Arrange
            TestFlags flags  = TestFlags.A;
            TestFlags flags2 = TestFlags.A | TestFlags.C;

            // Act & Assert
            Assert.AreEqual(true,  flags.IsSingleFlagOn());
            Assert.AreEqual(false, flags2.IsSingleFlagOn());
        }

        /// <summary> Testing GetRandomFlag method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.GetRandomFlag))]
        public void GetRandomFlag_Base()
        {
            // Arrange
            TestFlags flags = TestFlags.A | TestFlags.B;

            // Act
            TestFlags result = flags.GetRandomFlag((min, max) => new Random().Next(min, max));

            // Assert
            Assert.AreEqual(true, result.IsSingleFlagOn());
            Assert.AreEqual(true, result == TestFlags.A || result == TestFlags.B);
        }

        /// <summary> Testing HasAnyFlag method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.HasAnyFlag))]
        public void HasAnyFlag_Base()
        {
            // Arrange
            TestFlags flags = TestFlags.A | TestFlags.B;

            // Act & Assert
            Assert.AreEqual(true, flags.HasAnyFlag(TestFlags.A | TestFlags.C));
        }
        
        /// <summary> Testing HasAllFlags method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.HasAllFlags))]
        public void HasAllFlags_Base()
        {
            // Arrange
            TestFlags flags = TestFlags.A | TestFlags.B;

            // Act & Assert
            Assert.AreEqual(true, flags.HasAllFlags(TestFlags.A | TestFlags.B));
            Assert.AreEqual(false, flags.HasAllFlags(TestFlags.A | TestFlags.C));
        }
    }
}