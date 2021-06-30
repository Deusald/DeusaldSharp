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
    public class Vector3Tests
    {
        /// <summary> Testing if all standard Vector3 static values are correct. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void SCMWP()
        {
            // Arrange

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0f,  0f,  0f),  Vector3.Zero);
                Assert.AreEqual(new Vector3(1f,  1f,  1f),  Vector3.One);
                Assert.AreEqual(new Vector3(1f,  0f,  0f),  Vector3.UnitX);
                Assert.AreEqual(new Vector3(0f,  1f,  0f),  Vector3.UnitY);
                Assert.AreEqual(new Vector3(0f,  0f,  1f),  Vector3.UnitZ);
                Assert.AreEqual(new Vector3(0f,  1f,  0f),  Vector3.Up);
                Assert.AreEqual(new Vector3(0f,  -1f, 0f),  Vector3.Down);
                Assert.AreEqual(new Vector3(1f,  0f,  0f),  Vector3.Right);
                Assert.AreEqual(new Vector3(-1f, 0f,  0f),  Vector3.Left);
                Assert.AreEqual(new Vector3(0f,  0f,  -1f), Vector3.Forward);
                Assert.AreEqual(new Vector3(0f,  0f,  1f),  Vector3.Backward);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Magnitude))]
        public void KLHDC()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneMagnitude   = MathUtils.RoundToDecimal(one.Magnitude,   4);
            float twoMagnitude   = MathUtils.RoundToDecimal(two.Magnitude,   4);
            float threeMagnitude = MathUtils.RoundToDecimal(three.Magnitude, 4);
            float fourMagnitude  = MathUtils.RoundToDecimal(four.Magnitude,  4);
            float fiveMagnitude  = MathUtils.RoundToDecimal(five.Magnitude,  4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1.7321f,  oneMagnitude);
                Assert.AreEqual(6.7082f,  twoMagnitude);
                Assert.AreEqual(1f,       threeMagnitude);
                Assert.AreEqual(10.0623f, fourMagnitude);
                Assert.AreEqual(0f,       fiveMagnitude);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.SqrMagnitude))]
        public void GFYGF()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneSqrMagnitude   = MathUtils.RoundToDecimal(one.SqrMagnitude,   4);
            float twoSqrMagnitude   = MathUtils.RoundToDecimal(two.SqrMagnitude,   4);
            float threeSqrMagnitude = MathUtils.RoundToDecimal(three.SqrMagnitude, 4);
            float fourSqrMagnitude  = MathUtils.RoundToDecimal(four.SqrMagnitude,  4);
            float fiveSqrMagnitude  = MathUtils.RoundToDecimal(five.SqrMagnitude,  4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(3f,      oneSqrMagnitude);
                Assert.AreEqual(45f,     twoSqrMagnitude);
                Assert.AreEqual(1f,      threeSqrMagnitude);
                Assert.AreEqual(101.25f, fourSqrMagnitude);
                Assert.AreEqual(0f,      fiveSqrMagnitude);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Normalized vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector3.Normalized))]
        public void YFCBF()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneNormalized   = one.Normalized;
            Vector3 twoNormalized   = two.Normalized;
            Vector3 threeNormalized = three.Normalized;
            Vector3 fourNormalized  = four.Normalized;
            Vector3 fiveNormalized  = five.Normalized;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.57735026f,   0.57735026f,  0.57735026f),  oneNormalized);
                Assert.AreEqual(new Vector3(0.2981424f,    -0.745356f,   0.5962848f),   twoNormalized);
                Assert.AreEqual(new Vector3(0f,            1f,           0f),           threeNormalized);
                Assert.AreEqual(new Vector3(-0.099380806f, 0.049690403f, -0.99380803f), fourNormalized);
                Assert.AreEqual(new Vector3(0f,            0f,           0f),           fiveNormalized);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Negated vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector3.Negated))]
        public void FAAMC()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneNegated   = one.Negated;
            Vector3 twoNegated   = two.Negated;
            Vector3 threeNegated = three.Negated;
            Vector3 fourNegated  = four.Negated;
            Vector3 fiveNegated  = five.Negated;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-1f, -1f,   -1f), oneNegated);
                Assert.AreEqual(new Vector3(-2f, 5f,    -4f), twoNegated);
                Assert.AreEqual(new Vector3(0f,  -1f,   0f),  threeNegated);
                Assert.AreEqual(new Vector3(1f,  -0.5f, 10f), fourNegated);
                Assert.AreEqual(new Vector3(0f,  0f,    0f),  fiveNegated);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.IsValid))]
        public void TDDBE()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,                     1f,                     1f);
            Vector3 two   = new Vector3(2f,                     -5f,                    4f);
            Vector3 three = new Vector3(float.PositiveInfinity, 1f,                     0f);
            Vector3 four  = new Vector3(-1f,                    float.NegativeInfinity, -10f);
            Vector3 five  = new Vector3(0f,                     0f,                     0f);

            // Act
            bool oneIsValid   = one.IsValid;
            bool twoIsValid   = two.IsValid;
            bool threeIsValid = three.IsValid;
            bool fourIsValid  = four.IsValid;
            bool fiveIsValid  = five.IsValid;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(oneIsValid);
                Assert.IsTrue(twoIsValid);
                Assert.IsFalse(threeIsValid);
                Assert.IsFalse(fourIsValid);
                Assert.IsTrue(fiveIsValid);
            });
        }

        /// <summary> Testing if all constructors create correct vector. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void HHZWH()
        {
            // Arrange
            Vector3 one   = new Vector3(1f, 1f, 1f);
            Vector3 two   = new Vector3(5f);
            Vector3 three = new Vector3(new Vector2(1f, 2f), 3f);
            Vector3 four  = new Vector3(new Vector2(6f, 7f));
            Vector3 five  = new Vector3(4f, 2f);
            Vector3 six   = new Vector3(new Vector3(8f, 2f, 1f));

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1f, one.x);
                Assert.AreEqual(1f, one.y);
                Assert.AreEqual(1f, one.z);

                Assert.AreEqual(5f, two.x);
                Assert.AreEqual(5f, two.y);
                Assert.AreEqual(5f, two.z);

                Assert.AreEqual(1f, three.x);
                Assert.AreEqual(2f, three.y);
                Assert.AreEqual(3f, three.z);

                Assert.AreEqual(6f, four.x);
                Assert.AreEqual(7f, four.y);
                Assert.AreEqual(0f, four.z);

                Assert.AreEqual(4f, five.x);
                Assert.AreEqual(2f, five.y);
                Assert.AreEqual(0f, five.z);

                Assert.AreEqual(8f, six.x);
                Assert.AreEqual(2f, six.y);
                Assert.AreEqual(1f, six.z);
            });
        }

        /// <summary> Testing if setting values returns correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Set))]
        public void DKYTL()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            one.Set(2f, 3f, 4f);
            two.Set(-5f, 4f, 88f);
            three.Set(1f, 0f, -1f);
            four.Set(3f, 6f, 3f);
            five.Set(1f, 1f, 1f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(2f,  3f, 4f),  one);
                Assert.AreEqual(new Vector3(-5f, 4f, 88f), two);
                Assert.AreEqual(new Vector3(1f,  0f, -1f), three);
                Assert.AreEqual(new Vector3(3f,  6f, 3f),  four);
                Assert.AreEqual(new Vector3(1f,  1f, 1f),  five);
            });
        }

        /// <summary> Testing if setting zero on all axis returns correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.SetZero))]
        public void WLNHS()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);

            // Act
            one.SetZero();

            // Assert
            Assert.AreEqual(new Vector3(0f, 0f, 0f), one);
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Add))]
        public void NEKKT()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoSum    = Vector3.Add(one,   two);
            Vector3 threeFourSum = Vector3.Add(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(3f, -4f,  5f),   oneTwoSum);
                Assert.AreEqual(new Vector3(3f, 1.5f, -10f), threeFourSum);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Subtract))]
        public void BKAXF()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoSub    = Vector3.Subtract(one,   two);
            Vector3 threeFourSub = Vector3.Subtract(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-1f, 6f,   -3f), oneTwoSub);
                Assert.AreEqual(new Vector3(5f,  0.5f, 10f), threeFourSub);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Multiplying two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Multiply))]
        public void CYHKD()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoMul    = Vector3.Multiply(one,   two);
            Vector3 threeFourMul = Vector3.Multiply(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(2f,  -5f,  4f), oneTwoMul);
                Assert.AreEqual(new Vector3(-4f, 0.5f, 0f), threeFourMul);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Multiplying vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Multiply))]
        public void XRCXS()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);
            float   two = 4f;

            Vector3 three = new Vector3(4f, 1f, 0f);
            float   four  = -3f;

            // Act
            Vector3 oneTwoMul    = Vector3.Multiply(one,   two);
            Vector3 threeFourMul = Vector3.Multiply(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(4f,   4f,  4f), oneTwoMul);
                Assert.AreEqual(new Vector3(-12f, -3f, 0f), threeFourMul);

                Assert.AreEqual(new Vector3(1f, 1f, 1f), one);
                Assert.AreEqual(new Vector3(4f, 1f, 0f), three);
            });
        }

        /// <summary> Dividing two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Divide))]
        public void PAAZZ()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoDiv    = Vector3.Divide(one,   two);
            Vector3 threeFourDiv = Vector3.Divide(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.5f, -0.2f, 0.25f), oneTwoDiv);
                Assert.AreEqual(new Vector3(-4f,  2f,    0f),    threeFourDiv);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Dividing vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Divide))]
        public void WAHXS()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);
            float   two = 4f;

            Vector3 three = new Vector3(4f, 1f, 0f);
            float   four  = -2f;

            // Act
            Vector3 oneTwoDiv    = Vector3.Divide(one,   two);
            Vector3 threeFourDiv = Vector3.Divide(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.25f, 0.25f, 0.25f), oneTwoDiv);
                Assert.AreEqual(new Vector3(-2f,   -0.5f, 0f),    threeFourDiv);

                Assert.AreEqual(new Vector3(1f, 1f, 1f), one);
                Assert.AreEqual(new Vector3(4f, 1f, 0f), three);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.GetNegated))]
        public void SZLKG()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            // Act
            Vector3 oneNegated   = Vector3.GetNegated(one);
            Vector3 threeNegated = Vector3.GetNegated(two);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-1f, -1f, -1f), oneNegated);
                Assert.AreEqual(new Vector3(-2f, 5f,  -4f), threeNegated);

                Assert.AreEqual(new Vector3(1f, 1f,  1f), one);
                Assert.AreEqual(new Vector3(2f, -5f, 4f), two);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Negate))]
        public void DYKYA()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            one.Negate();
            two.Negate();
            three.Negate();
            four.Negate();
            five.Negate();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-1f, -1f,   -1f), one);
                Assert.AreEqual(new Vector3(-2f, 5f,    -4f), two);
                Assert.AreEqual(new Vector3(0f,  -1f,   0f),  three);
                Assert.AreEqual(new Vector3(1f,  -0.5f, 10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,    0f),  five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Length))]
        public void XAPXX()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneLength   = MathUtils.RoundToDecimal(one.Length(),   4);
            float twoLength   = MathUtils.RoundToDecimal(two.Length(),   4);
            float threeLength = MathUtils.RoundToDecimal(three.Length(), 4);
            float fourLength  = MathUtils.RoundToDecimal(four.Length(),  4);
            float fiveLength  = MathUtils.RoundToDecimal(five.Length(),  4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1.7321f,  oneLength);
                Assert.AreEqual(6.7082f,  twoLength);
                Assert.AreEqual(1f,       threeLength);
                Assert.AreEqual(10.0623f, fourLength);
                Assert.AreEqual(0f,       fiveLength);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.LengthSquared))]
        public void WXGXM()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneLengthSquared   = MathUtils.RoundToDecimal(one.LengthSquared(),   4);
            float twoLengthSquared   = MathUtils.RoundToDecimal(two.LengthSquared(),   4);
            float threeLengthSquared = MathUtils.RoundToDecimal(three.LengthSquared(), 4);
            float fourLengthSquared  = MathUtils.RoundToDecimal(four.LengthSquared(),  4);
            float fiveLengthSquared  = MathUtils.RoundToDecimal(five.LengthSquared(),  4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(3f,      oneLengthSquared);
                Assert.AreEqual(45f,     twoLengthSquared);
                Assert.AreEqual(1f,      threeLengthSquared);
                Assert.AreEqual(101.25f, fourLengthSquared);
                Assert.AreEqual(0f,      fiveLengthSquared);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Normalize))]
        public void ZFCPN()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            one.Normalize();
            two.Normalize();
            three.Normalize();
            four.Normalize();
            five.Normalize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.57735026f,   0.57735026f,  0.57735026f),  one);
                Assert.AreEqual(new Vector3(0.2981424f,    -0.745356f,   0.5962848f),   two);
                Assert.AreEqual(new Vector3(0f,            1f,           0f),           three);
                Assert.AreEqual(new Vector3(-0.099380806f, 0.049690403f, -0.99380803f), four);
                Assert.AreEqual(new Vector3(0f,            0f,           0f),           five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Normalized vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector3.GetNormalized))]
        public void MBFYF()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneNormalized   = Vector3.GetNormalized(one);
            Vector3 twoNormalized   = Vector3.GetNormalized(two);
            Vector3 threeNormalized = Vector3.GetNormalized(three);
            Vector3 fourNormalized  = Vector3.GetNormalized(four);
            Vector3 fiveNormalized  = Vector3.GetNormalized(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.57735026f,   0.57735026f,  0.57735026f),  oneNormalized);
                Assert.AreEqual(new Vector3(0.2981424f,    -0.745356f,   0.5962848f),   twoNormalized);
                Assert.AreEqual(new Vector3(0f,            1f,           0f),           threeNormalized);
                Assert.AreEqual(new Vector3(-0.099380806f, 0.049690403f, -0.99380803f), fourNormalized);
                Assert.AreEqual(new Vector3(0f,            0f,           0f),           fiveNormalized);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Cross))]
        public void DPBNC()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneCrossTwo    = one.Cross(two);
            Vector3 twoCrossThree  = two.Cross(three);
            Vector3 threeCrossFour = three.Cross(four);
            Vector3 fourCrossFive  = four.Cross(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(9f,   -2f, -7f), oneCrossTwo);
                Assert.AreEqual(new Vector3(-4f,  0f,  2f),  twoCrossThree);
                Assert.AreEqual(new Vector3(-10f, 0f,  1f),  threeCrossFour);
                Assert.AreEqual(new Vector3(0f,   0f,  0f),  fourCrossFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Distance))]
        public void RBFGP()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneDistanceTwo    = one.Distance(two);
            float twoDistanceThree  = two.Distance(three);
            float threeDistanceFour = three.Distance(four);
            float fourDistanceFive  = four.Distance(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(6.78233f,    oneDistanceTwo);
                Assert.AreEqual(7.483315f,   twoDistanceThree);
                Assert.AreEqual(10.0623055f, threeDistanceFour);
                Assert.AreEqual(10.0623055f, fourDistanceFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.DistanceSquared))]
        public void FCSGK()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneDistanceSquaredTwo    = one.DistanceSquared(two);
            float twoDistanceSquaredThree  = two.DistanceSquared(three);
            float threeDistanceSquaredFour = three.DistanceSquared(four);
            float fourDistanceSquaredFive  = four.DistanceSquared(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(46f,     oneDistanceSquaredTwo);
                Assert.AreEqual(56f,     twoDistanceSquaredThree);
                Assert.AreEqual(101.25f, threeDistanceSquaredFour);
                Assert.AreEqual(101.25f, fourDistanceSquaredFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Dot))]
        public void ZRBSY()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneDotTwo    = one.Dot(two);
            float twoDotThree  = two.Dot(three);
            float threeDotFour = three.Dot(four);
            float fourDotFive  = four.Dot(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1f,   oneDotTwo);
                Assert.AreEqual(-5f,  twoDotThree);
                Assert.AreEqual(0.5f, threeDotFour);
                Assert.AreEqual(0f,   fourDotFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Reflect))]
        public void FPTMC()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneReflectTwo    = one.Reflect(two);
            Vector3 twoReflectThree  = two.Reflect(three);
            Vector3 threeReflectFour = three.Reflect(four);
            Vector3 fourReflectFive  = four.Reflect(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-3f, 11f,  -7f),  oneReflectTwo);
                Assert.AreEqual(new Vector3(2f,  5f,   4f),   twoReflectThree);
                Assert.AreEqual(new Vector3(1f,  0.5f, 10f),  threeReflectFour);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), fourReflectFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Clamp))]
        public void XDPWS()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 oneMin = new Vector3(0.5f, 2f,  0f);
            Vector3 oneMax = new Vector3(3f,   4f,  0.7f);
            Vector3 twoMin = new Vector3(2.5f, -2f, 2f);
            Vector3 twoMax = new Vector3(5f,   7f,  3f);

            // Act
            Vector3 oneClamp = Vector3.Clamp(one, oneMin, oneMax);
            Vector3 twoClamp = Vector3.Clamp(two, twoMin, twoMax);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(1f,   2f,  0.7f), oneClamp);
                Assert.AreEqual(new Vector3(2.5f, -2f, 3f),   twoClamp);

                Assert.AreEqual(new Vector3(1f, 1f,  1f), one);
                Assert.AreEqual(new Vector3(2f, -5f, 4f), two);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Lerp))]
        public void ANPBE()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneLerpTwo    = Vector3.Lerp(one,   two,   0.25f);
            Vector3 twoLerpThree  = Vector3.Lerp(two,   three, 0.25f);
            Vector3 threeLerpFour = Vector3.Lerp(three, four,  0.25f);
            Vector3 fourLerpFive  = Vector3.Lerp(four,  five,  0.25f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(1.25f,  -0.5f,  1.75f), oneLerpTwo);
                Assert.AreEqual(new Vector3(1.5f,   -3.5f,  3f),    twoLerpThree);
                Assert.AreEqual(new Vector3(-0.25f, 0.875f, -2.5f), threeLerpFour);
                Assert.AreEqual(new Vector3(-0.75f, 0.375f, -7.5f), fourLerpFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Lerp))]
        public void RTZGA()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneLerpTwo    = Vector3.Lerp(one,   two,   0.5f);
            Vector3 twoLerpThree  = Vector3.Lerp(two,   three, 0.5f);
            Vector3 threeLerpFour = Vector3.Lerp(three, four,  0.5f);
            Vector3 fourLerpFive  = Vector3.Lerp(four,  five,  0.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(1.5f,  -2f,   2.5f), oneLerpTwo);
                Assert.AreEqual(new Vector3(1f,    -2f,   2f),   twoLerpThree);
                Assert.AreEqual(new Vector3(-0.5f, 0.75f, -5f),  threeLerpFour);
                Assert.AreEqual(new Vector3(-0.5f, 0.25f, -5f),  fourLerpFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Lerp))]
        public void ZXPPK()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneLerpTwo    = Vector3.Lerp(one,   two,   0.75f);
            Vector3 twoLerpThree  = Vector3.Lerp(two,   three, 0.75f);
            Vector3 threeLerpFour = Vector3.Lerp(three, four,  0.75f);
            Vector3 fourLerpFive  = Vector3.Lerp(four,  five,  0.75f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(1.75f,  -3.5f,  3.25f), oneLerpTwo);
                Assert.AreEqual(new Vector3(0.5f,   -0.5f,  1f),    twoLerpThree);
                Assert.AreEqual(new Vector3(-0.75f, 0.625f, -7.5f), threeLerpFour);
                Assert.AreEqual(new Vector3(-0.25f, 0.125f, -2.5f), fourLerpFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Max))]
        public void YLADW()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneMaxTwo    = Vector3.Max(one,   two);
            Vector3 twoMaxThree  = Vector3.Max(two,   three);
            Vector3 threeMaxFour = Vector3.Max(three, four);
            Vector3 fourMaxFive  = Vector3.Max(four,  five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(2f, 1f,   4f), oneMaxTwo);
                Assert.AreEqual(new Vector3(2f, 1f,   4f), twoMaxThree);
                Assert.AreEqual(new Vector3(0f, 1f,   0f), threeMaxFour);
                Assert.AreEqual(new Vector3(0f, 0.5f, 0f), fourMaxFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Min))]
        public void XXXTC()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneMinTwo    = Vector3.Min(one,   two);
            Vector3 twoMinThree  = Vector3.Min(two,   three);
            Vector3 threeMinFour = Vector3.Min(three, four);
            Vector3 fourMinFive  = Vector3.Min(four,  five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(1f,  -5f,  1f),   oneMinTwo);
                Assert.AreEqual(new Vector3(0f,  -5f,  0f),   twoMinThree);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), threeMinFour);
                Assert.AreEqual(new Vector3(-1f, 0f,   -10f), fourMinFive);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Equals))]
        public void CSPNN()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(one.Equals(one));
                Assert.IsFalse(one.Equals(two));
                Assert.IsFalse(one.Equals(new object()));

                Assert.AreEqual(new Vector3(1f, 1f,  1f), one);
                Assert.AreEqual(new Vector3(2f, -5f, 4f), two);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.GetHashCode))]
        public void ZAMCB()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(796917760,  one.GetHashCode());
                Assert.AreEqual(-736100352, two.GetHashCode());

                Assert.AreEqual(new Vector3(1f, 1f,  1f), one);
                Assert.AreEqual(new Vector3(2f, -5f, 4f), two);
            });
        }

        /// <summary> Testing operator ==. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void PMFHA()
        {
            // Arrange
            Vector3 one   = new Vector3(1f, 1f,  1f);
            Vector3 two   = new Vector3(2f, -5f, 4f);
            Vector3 three = new Vector3(1f, 1f,  1f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(one == three);
                Assert.IsFalse(one == two);

                Assert.AreEqual(new Vector3(1f, 1f,  1f), one);
                Assert.AreEqual(new Vector3(2f, -5f, 4f), two);
                Assert.AreEqual(new Vector3(1f, 1f,  1f), three);
            });
        }

        /// <summary> Testing operator !=. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void NCHSG()
        {
            // Arrange
            Vector3 one   = new Vector3(1f, 1f,  1f);
            Vector3 two   = new Vector3(2f, -5f, 4f);
            Vector3 three = new Vector3(1f, 1f,  1f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(one != three);
                Assert.IsTrue(one != two);

                Assert.AreEqual(new Vector3(1f, 1f,  1f), one);
                Assert.AreEqual(new Vector3(2f, -5f, 4f), two);
                Assert.AreEqual(new Vector3(1f, 1f,  1f), three);
            });
        }

        /// <summary> Testing operator single -. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void BTFTB()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            Vector3 oneNegated   = -one;
            Vector3 twoNegated   = -two;
            Vector3 threeNegated = -three;
            Vector3 fourNegated  = -four;
            Vector3 fiveNegated  = -five;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-1f, -1f,   -1f), oneNegated);
                Assert.AreEqual(new Vector3(-2f, 5f,    -4f), twoNegated);
                Assert.AreEqual(new Vector3(0f,  -1f,   0f),  threeNegated);
                Assert.AreEqual(new Vector3(1f,  -0.5f, 10f), fourNegated);
                Assert.AreEqual(new Vector3(0f,  0f,    0f),  fiveNegated);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(0f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
                Assert.AreEqual(new Vector3(0f,  0f,   0f),   five);
            });
        }

        /// <summary> Testing operator +. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void AMHKM()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoSum    = one + two;
            Vector3 threeFourSum = three + four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(3f, -4f,  5f),   oneTwoSum);
                Assert.AreEqual(new Vector3(3f, 1.5f, -10f), threeFourSum);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Testing operator -. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void EEGTY()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoSub    = one - two;
            Vector3 threeFourSub = three - four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(-1f, 6f,   -3f), oneTwoSub);
                Assert.AreEqual(new Vector3(5f,  0.5f, 10f), threeFourSub);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Testing operator *. Multiplying two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void TBCME()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoMul    = one * two;
            Vector3 threeFourMul = three * four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(2f,  -5f,  4f), oneTwoMul);
                Assert.AreEqual(new Vector3(-4f, 0.5f, 0f), threeFourMul);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Testing operator *. Multiplying vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void PPNHA()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);
            float   two = 4f;

            Vector3 three = new Vector3(4f, 1f, 0f);
            float   four  = -3f;

            // Act
            Vector3 oneTwoMul    = one * two;
            Vector3 threeFourMul = three * four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(4f,   4f,  4f), oneTwoMul);
                Assert.AreEqual(new Vector3(-12f, -3f, 0f), threeFourMul);

                Assert.AreEqual(new Vector3(1f, 1f, 1f), one);
                Assert.AreEqual(new Vector3(4f, 1f, 0f), three);
            });
        }

        /// <summary> Testing operator /. Dividing two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void FFHCA()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            Vector3 three = new Vector3(4f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);

            // Act
            Vector3 oneTwoDiv    = one / two;
            Vector3 threeFourDiv = three / four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.5f, -0.2f, 0.25f), oneTwoDiv);
                Assert.AreEqual(new Vector3(-4f,  2f,    0f),    threeFourDiv);

                Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
                Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
                Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
                Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
            });
        }

        /// <summary> Testing operator /. Dividing vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void FRGXA()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);
            float   two = 4f;

            Vector3 three = new Vector3(4f, 1f, 0f);
            float   four  = -2f;

            // Act
            Vector3 oneTwoDiv    = one / two;
            Vector3 threeFourDiv = three / four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(0.25f, 0.25f, 0.25f), oneTwoDiv);
                Assert.AreEqual(new Vector3(-2f,   -0.5f, 0f),    threeFourDiv);

                Assert.AreEqual(new Vector3(1f, 1f, 1f), one);
                Assert.AreEqual(new Vector3(4f, 1f, 0f), three);
            });
        }

        /// <summary> Testing implicit operator from Vector3 to Vector2. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void PEATS()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(7f, 8f);

            // Act
            Vector3 newOne = one;
            Vector3 newTwo = two;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector3(1,  1f, 0f), newOne);
                Assert.AreEqual(new Vector3(7f, 8f, 0f), newTwo);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(7f, 8f), two);
            });
        }
        
        /// <summary> Testing bracket access operator. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void HPRSH()
        {
            // Arrange
            Vector3 two = new Vector3(7f, 8f, 9f);
            Vector3 one = new Vector3(1f, 1f, 1f);

            // Act
            two[0] = 10f;
            two[1] = 11f;
            two[2] = 12f;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1f, one[0]);
                Assert.AreEqual(1f, one[1]);
                Assert.AreEqual(1f, one[2]);
                Assert.AreEqual(10f, two[0]);
                Assert.AreEqual(11f, two[1]);
                Assert.AreEqual(12f, two[2]);
            });
        }
    }
}