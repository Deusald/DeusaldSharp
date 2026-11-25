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

using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class HelpersTests
    {
        private enum TestEnum
        {
            A = 0,
            B = 1
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(Helpers))]
        public void Helpers_ArgumentTaker()
        {
            // Arrange
            string[] args = { "10", "B" };

            // Act
            int      ten = args.TakeSimpleType(0, 0);
            TestEnum b   = args.TakeEnum(1, TestEnum.A);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(ten, Is.EqualTo(10));
                Assert.That(b, Is.EqualTo(TestEnum.B));
            });
        }

        [System.Flags]
        private enum TestFlags
        {
            None = 0,
            A    = 1,
            B    = 2
        }

        /// <summary> Numbers are parsed with invariant culture regardless of current culture. </summary>
        [Test]
        [TestOf(nameof(Helpers.TakeSimpleType))]
        public void Helpers_TakeSimpleType_UsesInvariantCulture()
        {
            // Arrange
            string[]                         args            = { "1.5" };
            System.Globalization.CultureInfo previousCulture = System.Globalization.CultureInfo.CurrentCulture;

            try
            {
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-DE");

                // Act
                double value    = args.TakeSimpleType(0, 0d);
                double fromList =args.TakeFromList(0, new System.Collections.Generic.List<double> { 1.5d }, 0d);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(value,    Is.EqualTo(1.5d));
                    Assert.That(fromList, Is.EqualTo(1.5d));
                });
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentCulture = previousCulture;
            }
        }

        /// <summary> Negative index returns default value instead of throwing. </summary>
        [Test]
        [TestOf(nameof(Helpers))]
        public void Helpers_ArgumentTaker_NegativeIndex_ReturnsDefault()
        {
            // Arrange
            string[] args = { "10", "B" };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(args.TakeSimpleType(-1, 7),                                               Is.EqualTo(7));
                Assert.That(args.TakeString(-1, "def"),                                               Is.EqualTo("def"));
                Assert.That(args.TakeUsername(-1, "def"),                                             Is.EqualTo("def"));
                Assert.That(args.TakeEnum(-1, TestEnum.A),                                            Is.EqualTo(TestEnum.A));
                Assert.That(args.TakeFromList(-1, new System.Collections.Generic.List<int> { 10 }, 3), Is.EqualTo(3));
            });
        }

        /// <summary> Undefined numeric values are rejected for non-flags enums but allowed for flags enums. </summary>
        [Test]
        [TestOf(nameof(Helpers.TakeEnum))]
        public void Helpers_TakeEnum_UndefinedValue()
        {
            // Arrange
            string[] args = { "999", "1", "3" };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(args.TakeEnum(0, TestEnum.A),     Is.EqualTo(TestEnum.A));
                Assert.That(args.TakeEnum(1, TestEnum.A),     Is.EqualTo(TestEnum.B));
                Assert.That(args.TakeEnum(2, TestFlags.None), Is.EqualTo(TestFlags.A | TestFlags.B));
            });
        }

        /// <summary> GetRandomBool returns both values. </summary>
        [Test]
        [TestOf(nameof(Helpers.GetRandomBool))]
        public void Helpers_GetRandomBool_ReturnsBothValues()
        {
            // Arrange
            bool seenTrue  = false;
            bool seenFalse = false;

            // Act
            for (int x = 0; x < 1000 && !(seenTrue && seenFalse); ++x)
            {
                bool value = Helpers.GetRandomBool();
                seenTrue  |= value;
                seenFalse |= !value;
            }

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(seenTrue,  Is.True);
                Assert.That(seenFalse, Is.True);
            });
        }
    }
}