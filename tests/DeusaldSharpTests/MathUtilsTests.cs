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
        public void MathUtils_SEC_TO_MILLISECONDS()
        {
            // Arrange

            // Act
            int secondsToMilliseconds = 15 * MathUtils.SEC_TO_MILLISECONDS;

            // Assert
            Assert.That(secondsToMilliseconds, Is.EqualTo(15000));
        }

        /// <summary> Testing conversion from minutes to seconds. </summary>
        [Test]
        [TestOf(nameof(MathUtils.MIN_TO_SEC))]
        public void MathUtils_MIN_TO_SEC()
        {
            // Arrange

            // Act
            int minutesToSeconds = 15 * MathUtils.MIN_TO_SEC;

            // Assert
            Assert.That(minutesToSeconds, Is.EqualTo(900));
        }

        /// <summary> Testing conversion from hours to minutes. </summary>
        [Test]
        [TestOf(nameof(MathUtils.HOURS_TO_MIN))]
        public void MathUtils_HOURS_TO_MIN()
        {
            // Arrange

            // Act
            int hoursToMinutes = 15 * MathUtils.HOURS_TO_MIN;

            // Assert
            Assert.That(hoursToMinutes, Is.EqualTo(900));
        }

        /// <summary> Testing conversion from days to hours. </summary>
        [Test]
        [TestOf(nameof(MathUtils.DAYS_TO_HOURS))]
        public void MathUtils_DAYS_TO_HOURS()
        {
            // Arrange

            // Act
            int daysToHours = 15 * MathUtils.DAYS_TO_HOURS;

            // Assert
            Assert.That(daysToHours, Is.EqualTo(360));
        }

        /// <summary> Testing conversion from days to seconds. </summary>
        [Test]
        [TestOf(nameof(MathUtils))]
        public void MathUtils_DaysToSeconds()
        {
            // Arrange

            // Act
            int daysToSeconds = 3 * MathUtils.DAYS_TO_HOURS * MathUtils.HOURS_TO_MIN * MathUtils.MIN_TO_SEC;

            // Assert
            Assert.That(daysToSeconds, Is.EqualTo(259200));
        }

        /// <summary> Testing conversion from degrees to radians. </summary>
        [Test]
        [TestOf(nameof(MathUtils.DEG_TO_RAD))]
        public void MathUtils_DEG_TO_RAD()
        {
            // Arrange

            // Act
            float radians = 90 * MathUtils.DEG_TO_RAD;

            // Assert
            Assert.That(radians, Is.EqualTo(1.57079637f));
        }

        /// <summary> Testing conversion from radians to degrees. </summary>
        [Test]
        [TestOf(nameof(MathUtils.RAD_TO_DEG))]
        public void MathUtils_RAD_TO_DEG()
        {
            // Arrange

            // Act
            float degrees = 3.1415926536f * MathUtils.RAD_TO_DEG;

            // Assert
            Assert.That(degrees, Is.EqualTo(180f));
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.Clamp))]
        public void MathUtils_Clamp()
        {
            // Arrange

            // Act
            float clamp      = 10.5f.Clamp(15.1f, 20f);
            int   clampTwo   = 4.Clamp(2, 3);
            int   clampThree = 5.Clamp(2, 10);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(clamp, Is.EqualTo(15.1f));
                Assert.That(clampTwo, Is.EqualTo(3));
                Assert.That(clampThree, Is.EqualTo(5));
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.Lerp))]
        public void MathUtils_Lerp()
        {
            // Arrange

            // Act
            float lerp      = MathUtils.Lerp(1, 10, 0.75f);
            float lerpTwo   = MathUtils.Lerp(1, 10, 0.25f);
            float lerpThree = MathUtils.Lerp(1, 10, 0.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(lerp, Is.EqualTo(7.75f));
                Assert.That(lerpTwo, Is.EqualTo(3.25f));
                Assert.That(lerpThree, Is.EqualTo(5.5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.InverseLerp))]
        public void MathUtils_InverseLerp()
        {
            // Arrange

            // Act
            float inverseLerp      = MathUtils.InverseLerp(1, 10, 7.75f);
            float inverseLerpTwo   = MathUtils.InverseLerp(1, 10, 3.25f);
            float inverseLerpThree = MathUtils.InverseLerp(1, 10, 5.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(inverseLerp, Is.EqualTo(0.75f));
                Assert.That(inverseLerpTwo, Is.EqualTo(0.25f));
                Assert.That(inverseLerpThree, Is.EqualTo(0.5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.RoundToDecimal))]
        public void MathUtils_RoundToDecimal()
        {
            // Arrange

            // Act
            float roundToDecimal      = 7.451587f.RoundToDecimal(6);
            float roundToDecimalTwo   = 7.451587f.RoundToDecimal(3);
            float roundToDecimalThree = 7.451587f.RoundToDecimal(0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(roundToDecimal, Is.EqualTo(7.451587f));
                Assert.That(roundToDecimalTwo, Is.EqualTo(7.452f));
                Assert.That(roundToDecimalThree, Is.EqualTo(7f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.IsFloatZero))]
        public void MathUtils_IsFloatZero()
        {
            // Arrange

            // Act
            bool isFloatZero      = 0f.IsFloatZero();
            bool isFloatZeroTwo   = 1f.IsFloatZero();
            bool isFloatZeroThree = 0.1f.IsFloatZero();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(isFloatZero, Is.True);
                Assert.That(isFloatZeroTwo, Is.False);
                Assert.That(isFloatZeroThree, Is.False);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.AreFloatsEquals))]
        public void MathUtils_AreFloatsEquals()
        {
            // Arrange

            // Act
            bool areFloatsEquals      = 1.55f.AreFloatsEquals(1.55f);
            bool areFloatsEqualsTwo   = 5.123f.AreFloatsEquals(10.234f);
            bool areFloatsEqualsThree = 1.55f.AreFloatsEquals(1.56f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(areFloatsEquals, Is.True);
                Assert.That(areFloatsEqualsTwo, Is.False);
                Assert.That(areFloatsEqualsThree, Is.False);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.MarkBit))]
        public void MathUtils_MarkBit()
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
                Assert.That(markedBitOne, Is.EqualTo(5));
                Assert.That(markedBitTwo, Is.EqualTo(14));
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.NumberOfSetBits))]
        public void MathUtils_NumberOfSetBits()
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
                Assert.That(numberOfSetBits, Is.EqualTo(5));
                Assert.That(numberOfSetBitsTwo, Is.EqualTo(7));
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.IsSingleBitOn))]
        public void MathUtils_IsSingleBitOn()
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
                Assert.That(isSingleBitOn, Is.False);
                Assert.That(isSingleBitOnTwo, Is.False);
                Assert.That(isSingleBitOnThree, Is.True);
                Assert.That(isSingleBitOnFour, Is.True);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.HasAnyBitOn))]
        public void MathUtils_HasAnyBitOn()
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
                Assert.That(hasAnyBitOn, Is.True);
                Assert.That(hasAnyBitOnTwo, Is.True);
                Assert.That(hasAnyBitOnThree, Is.False);
                Assert.That(hasAnyBitOnFour, Is.False);
            });
        }

        /// <summary> Providing data and expecting mathematically correct result. </summary>
        [Test]
        [TestOf(nameof(MathUtils.HasAllBitsOn))]
        public void MathUtils_HasAllBitsOn()
        {
            // Arrange

            // Act
            uint one   = 467456;
            int  two   = 598465;
            uint three = uint.MaxValue;
            int  four  = int.MaxValue;

            bool hasAllBitsOn      = one.HasAllBitsOn(uint.MaxValue);
            bool hasAllBitsOnTwo   = two.HasAllBitsOn(int.MaxValue);
            bool hasAllBitsOnThree = three.HasAllBitsOn(uint.MaxValue);
            bool hasAllBitsOnFour  = four.HasAllBitsOn(int.MaxValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(hasAllBitsOn, Is.False);
                Assert.That(hasAllBitsOnTwo, Is.False);
                Assert.That(hasAllBitsOnThree, Is.True);
                Assert.That(hasAllBitsOnFour, Is.True);
            });
        }

        /// <summary> MathMod must not overflow for big operands and must always return value in [0, |b|). </summary>
        [Test]
        [TestOf(nameof(MathUtils.MathMod))]
        public void MathUtils_MathMod_NoOverflow()
        {
            Assert.Multiple(() =>
            {
                Assert.That(50000.MathMod(50000),        Is.EqualTo(0));
                Assert.That(50001.MathMod(50000),        Is.EqualTo(1));
                Assert.That((-1).MathMod(5),             Is.EqualTo(4));
                Assert.That((-5).MathMod(5),             Is.EqualTo(0));
                Assert.That(7.MathMod(3),                Is.EqualTo(1));
                Assert.That(5.MathMod(-3),               Is.EqualTo(2));
                Assert.That((-5).MathMod(-3),            Is.EqualTo(1));
                Assert.That(int.MinValue.MathMod(3),     Is.EqualTo(1));
                Assert.That(int.MinValue.MathMod(-1),    Is.EqualTo(0));
                Assert.That((-1).MathMod(int.MinValue),  Is.EqualTo(int.MaxValue));
                Assert.That(int.MaxValue.MathMod(1000),  Is.EqualTo(647));
            });
        }

        /// <summary> InverseLerp with equal range ends returns 0 instead of a. </summary>
        [Test]
        [TestOf(nameof(MathUtils.InverseLerp))]
        public void MathUtils_InverseLerp_EqualRange_ReturnsZero()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MathUtils.InverseLerp(5f, 5f, 7f),     Is.EqualTo(0f));
                Assert.That(MathUtils.InverseLerp(5d, 5d, 7d),     Is.EqualTo(0d));
                Assert.That(MathUtils.InverseLerp(5m, 5m, 7m),     Is.EqualTo(0m));
            });
        }

        /// <summary> IsOdd works for negative numbers. </summary>
        [Test]
        [TestOf(nameof(MathUtils.IsOdd))]
        public void MathUtils_IsOdd_Negative()
        {
            Assert.Multiple(() =>
            {
                Assert.That((-3).IsOdd(),         Is.True);
                Assert.That((-4).IsOdd(),         Is.False);
                Assert.That(int.MinValue.IsOdd(), Is.False);
                Assert.That(3.IsOdd(),            Is.True);
            });
        }

        /// <summary> NumberOfSetBits counts the sign bit for negative signed values. </summary>
        [Test]
        [TestOf(nameof(MathUtils.NumberOfSetBits))]
        public void MathUtils_NumberOfSetBits_Negative()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((sbyte)-1).NumberOfSetBits(),    Is.EqualTo(8));
                Assert.That(((short)-1).NumberOfSetBits(),    Is.EqualTo(16));
                Assert.That((-1).NumberOfSetBits(),           Is.EqualTo(32));
                Assert.That((-1L).NumberOfSetBits(),          Is.EqualTo(64));
                Assert.That(int.MinValue.NumberOfSetBits(),   Is.EqualTo(1));
                Assert.That(sbyte.MinValue.NumberOfSetBits(), Is.EqualTo(1));
            });
        }

        /// <summary> IsSingleBitOn handles min values of signed types. </summary>
        [Test]
        [TestOf(nameof(MathUtils.IsSingleBitOn))]
        public void MathUtils_IsSingleBitOn_MinValues()
        {
            Assert.Multiple(() =>
            {
                Assert.That(sbyte.MinValue.IsSingleBitOn(), Is.True);
                Assert.That(short.MinValue.IsSingleBitOn(), Is.True);
                Assert.That(int.MinValue.IsSingleBitOn(),   Is.True);
                Assert.That(long.MinValue.IsSingleBitOn(),  Is.True);
                Assert.That(((sbyte)-1).IsSingleBitOn(),    Is.False);
                Assert.That(((short)-1).IsSingleBitOn(),    Is.False);
            });
        }

        /// <summary> AreFloatsEquals uses a real tolerance and handles infinities and NaN. </summary>
        [Test]
        [TestOf(nameof(MathUtils.AreFloatsEquals))]
        public void MathUtils_AreFloatsEquals_Tolerance()
        {
            Assert.Multiple(() =>
            {
                Assert.That((0.1f + 0.2f).AreFloatsEquals(0.3f),                           Is.True);
                Assert.That(1f.AreFloatsEquals(1f + 1e-7f),                                Is.True);
                Assert.That(1f.AreFloatsEquals(1.001f),                                    Is.False);
                Assert.That(1000000f.AreFloatsEquals(1000000.06f),                         Is.True);
                Assert.That(float.PositiveInfinity.AreFloatsEquals(float.PositiveInfinity), Is.True);
                Assert.That(float.PositiveInfinity.AreFloatsEquals(float.NegativeInfinity), Is.False);
                Assert.That(float.PositiveInfinity.AreFloatsEquals(float.MaxValue),        Is.False);
                Assert.That(float.NaN.AreFloatsEquals(float.NaN),                          Is.False);
                Assert.That(1e-7f.IsFloatZero(),                                           Is.True);
                Assert.That(1e-3f.IsFloatZero(),                                           Is.False);
                Assert.That(MathUtils.EPSILON_SQUARE,                                      Is.GreaterThan(0f));
            });
        }

        /// <summary> Clamp with min greater than max throws like Math.Clamp. </summary>
        [Test]
        [TestOf(nameof(MathUtils.Clamp))]
        public void MathUtils_Clamp_MinGreaterThanMax_Throws()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<System.ArgumentException>(() => 5.Clamp(10, 1));
                Assert.Throws<System.ArgumentException>(() => 5f.Clamp(10f, 1f));
                Assert.Throws<System.ArgumentException>(() => 5d.Clamp(10d, 1d));
                Assert.Throws<System.ArgumentException>(() => ((byte)5).Clamp(10, 1));
                Assert.Throws<System.ArgumentException>(() => 5m.Clamp(10m, 1m));
                Assert.That(5.Clamp(5, 5), Is.EqualTo(5));
            });
        }
    }
}