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

using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class MathUtilsTests
    {
        /// <summary> Testing conversion from minutes to seconds. </summary>
        [Test]
        [TestOf(nameof(MathUtils.SEC_TO_MILLISECONDS))]
        public void XWTMN()
        {
            // Arrange

            // Act
            int secondsToMilliseconds = 15 * MathUtils.SEC_TO_MILLISECONDS;

            // Assert
            Assert.AreEqual(15000, secondsToMilliseconds);
        }

        /// <summary> Testing conversion from minutes to seconds. </summary>
        [Test]
        [TestOf(nameof(MathUtils.MIN_TO_SEC))]
        public void SCMWP()
        {
            // Arrange

            // Act
            int minutesToSeconds = 15 * MathUtils.MIN_TO_SEC;

            // Assert
            Assert.AreEqual(900, minutesToSeconds);
        }

        /// <summary> Testing conversion from hours to minutes. </summary>
        [Test]
        [TestOf(nameof(MathUtils.HOURS_TO_MIN))]
        public void ARRPH()
        {
            // Arrange

            // Act
            int hoursToMinutes = 15 * MathUtils.HOURS_TO_MIN;

            // Assert
            Assert.AreEqual(900, hoursToMinutes);
        }

        /// <summary> Testing conversion from days to hours. </summary>
        [Test]
        [TestOf(nameof(MathUtils.DAYS_TO_HOURS))]
        public void YXMWS()
        {
            // Arrange

            // Act
            int daysToHours = 15 * MathUtils.DAYS_TO_HOURS;

            // Assert
            Assert.AreEqual(360, daysToHours);
        }

        /// <summary> Testing conversion from days to seconds. </summary>
        [Test]
        [TestOf(nameof(MathUtils))]
        public void APWZT()
        {
            // Arrange

            // Act
            int daysToSeconds = 3 * MathUtils.DAYS_TO_HOURS * MathUtils.HOURS_TO_MIN * MathUtils.MIN_TO_SEC;

            // Assert
            Assert.AreEqual(259200, daysToSeconds);
        }

        /// <summary> Testing conversion from degrees to radians. </summary>
        [Test]
        [TestOf(nameof(MathUtils.DEG_TO_RAD))]
        public void SASTE()
        {
            // Arrange

            // Act
            float radians = 90 * MathUtils.DEG_TO_RAD;

            // Assert
            Assert.AreEqual(1.57079637f, radians);
        }

        /// <summary> Testing conversion from radians to degrees. </summary>
        [Test]
        [TestOf(nameof(MathUtils.RAD_TO_DEG))]
        public void EKSCS()
        {
            // Arrange

            // Act
            float degrees = 3.1415926536f * MathUtils.RAD_TO_DEG;

            // Assert
            Assert.AreEqual(180f, degrees);
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.Clamp))]
        public void EAFTF()
        {
            // Arrange

            // Act
            float clamp      = 10.5f.Clamp(15.1f, 20f);
            int   clampTwo   = 4.Clamp(2, 3);
            int   clampThree = 5.Clamp(2, 10);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(15.1f, clamp);
                Assert.AreEqual(3,     clampTwo);
                Assert.AreEqual(5,     clampThree);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.Lerp))]
        public void MWPMX()
        {
            // Arrange

            // Act
            float lerp      = MathUtils.Lerp(1, 10, 0.75f);
            float lerpTwo   = MathUtils.Lerp(1, 10, 0.25f);
            float lerpThree = MathUtils.Lerp(1, 10, 0.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(7.75f, lerp);
                Assert.AreEqual(3.25f, lerpTwo);
                Assert.AreEqual(5.5f,  lerpThree);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.InverseLerp))]
        public void ZBKKN()
        {
            // Arrange

            // Act
            float inverseLerp      = MathUtils.InverseLerp(1, 10, 7.75f);
            float inverseLerpTwo   = MathUtils.InverseLerp(1, 10, 3.25f);
            float inverseLerpThree = MathUtils.InverseLerp(1, 10, 5.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(0.75f, inverseLerp);
                Assert.AreEqual(0.25f, inverseLerpTwo);
                Assert.AreEqual(0.5f,  inverseLerpThree);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.RoundToDecimal))]
        public void FLYTW()
        {
            // Arrange

            // Act
            float roundToDecimal      = 7.451587f.RoundToDecimal(6);
            float roundToDecimalTwo   = 7.451587f.RoundToDecimal(3);
            float roundToDecimalThree = 7.451587f.RoundToDecimal(0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(7.451587f, roundToDecimal);
                Assert.AreEqual(7.452f,    roundToDecimalTwo);
                Assert.AreEqual(7f,        roundToDecimalThree);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.IsFloatZero))]
        public void MWZXA()
        {
            // Arrange

            // Act
            bool isFloatZero      = 0f.IsFloatZero();
            bool isFloatZeroTwo   = 1f.IsFloatZero();
            bool isFloatZeroThree = 0.1f.IsFloatZero();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(isFloatZero);
                Assert.IsFalse(isFloatZeroTwo);
                Assert.IsFalse(isFloatZeroThree);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.AreFloatsEquals))]
        public void FTGTK()
        {
            // Arrange

            // Act
            bool areFloatsEquals      = 1.55f.AreFloatsEquals(1.55f);
            bool areFloatsEqualsTwo   = 5.123f.AreFloatsEquals(10.234f);
            bool areFloatsEqualsThree = 1.55f.AreFloatsEquals(1.56f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(areFloatsEquals);
                Assert.IsFalse(areFloatsEqualsTwo);
                Assert.IsFalse(areFloatsEqualsThree);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.MarkBit))]
        public void RKXXS()
        {
            // Arrange

            // Act
            uint one = 7;
            int  two = 10;

            uint markedBitOne = one.MarkBit(2, false);
            int  markedBitTwo = two.MarkBit(4, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, markedBitOne);
                Assert.AreEqual(14, markedBitTwo);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.NumberOfSetBits))]
        public void RKXAM()
        {
            // Arrange

            // Act
            uint one = 467456;
            int  two = 598465;

            uint numberOfSetBits    = one.NumberOfSetBits();
            uint numberOfSetBitsTwo = two.NumberOfSetBits();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, numberOfSetBits);
                Assert.AreEqual(7, numberOfSetBitsTwo);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.IsSingleBitOn))]
        public void LLAMP()
        {
            // Arrange

            // Act
            uint one   = 467456;
            int  two   = 598465;
            uint three = 1024;
            int  four  = 1024;

            bool isSingleBitOn      = one.IsSingleBitOn();
            bool isSingleBitOnTwo   = two.IsSingleBitOn();
            bool isSingleBitOnThree = three.IsSingleBitOn();
            bool isSingleBitOnFour  = four.IsSingleBitOn();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(isSingleBitOn);
                Assert.IsFalse(isSingleBitOnTwo);
                Assert.IsTrue(isSingleBitOnThree);
                Assert.IsTrue(isSingleBitOnFour);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.HasAnyBitOn))]
        public void YTDTA()
        {
            // Arrange

            // Act
            uint one   = 467456;
            int  two   = 598465;
            uint three = 0;
            int  four  = 0;

            bool hasAnyBitOn      = one.HasAnyBitOn(uint.MaxValue);
            bool hasAnyBitOnTwo   = two.HasAnyBitOn(int.MaxValue);
            bool hasAnyBitOnThree = three.HasAnyBitOn(uint.MaxValue);
            bool hasAnyBitOnFour  = four.HasAnyBitOn(int.MaxValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(hasAnyBitOn);
                Assert.IsTrue(hasAnyBitOnTwo);
                Assert.IsFalse(hasAnyBitOnThree);
                Assert.IsFalse(hasAnyBitOnFour);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.HasAllBitsOn))]
        public void XGRXH()
        {
            // Arrange

            // Act
            uint one   = 467456;
            int  two   = 598465;
            uint three = uint.MaxValue;
            int  four  = int.MaxValue;

            bool hasAllBitsOn      = MathUtils.HasAllBitsOn(one,   uint.MaxValue);
            bool hasAllBitsOnTwo   = MathUtils.HasAllBitsOn(two,   int.MaxValue);
            bool hasAllBitsOnThree = MathUtils.HasAllBitsOn(three, uint.MaxValue);
            bool hasAllBitsOnFour  = MathUtils.HasAllBitsOn(four,  int.MaxValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(hasAllBitsOn);
                Assert.IsFalse(hasAllBitsOnTwo);
                Assert.IsTrue(hasAllBitsOnThree);
                Assert.IsTrue(hasAllBitsOnFour);
            });
        }
    }
}