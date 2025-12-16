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
    public class Vector2Tests
    {
        /// <summary> Testing if all standard Vector2 static values are correct. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Base()
        {
            // Arrange

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0f,  0f), Vector2.Zero);
                Assert.AreEqual(new Vector2(1f,  1f), Vector2.One);
                Assert.AreEqual(new Vector2(0f,  1f), Vector2.Up);
                Assert.AreEqual(new Vector2(0f,  -1f), Vector2.Down);
                Assert.AreEqual(new Vector2(1f,  0f), Vector2.Right);
                Assert.AreEqual(new Vector2(-1f, 0f), Vector2.Left);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Magnitude))]
        public void Vector2_Magnitude()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneMagnitude   = one.Magnitude.RoundToDecimal(4);
            float twoMagnitude   = two.Magnitude.RoundToDecimal(4);
            float threeMagnitude = three.Magnitude.RoundToDecimal(4);
            float fourMagnitude  = four.Magnitude.RoundToDecimal(4);
            float fiveMagnitude  = five.Magnitude.RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1.41419995f, oneMagnitude);
                Assert.AreEqual(5.38520002f, twoMagnitude);
                Assert.AreEqual(1.0f,        threeMagnitude);
                Assert.AreEqual(1.11800003f, fourMagnitude);
                Assert.AreEqual(0f,          fiveMagnitude);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.SqrMagnitude))]
        public void Vector2_SqrMagnitude()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneSqrMagnitude   = one.SqrMagnitude.RoundToDecimal(4);
            float twoSqrMagnitude   = two.SqrMagnitude.RoundToDecimal(4);
            float threeSqrMagnitude = three.SqrMagnitude.RoundToDecimal(4);
            float fourSqrMagnitude  = four.SqrMagnitude.RoundToDecimal(4);
            float fiveSqrMagnitude  = five.SqrMagnitude.RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2f,      oneSqrMagnitude);
                Assert.AreEqual(29f,     twoSqrMagnitude);
                Assert.AreEqual(1f,      threeSqrMagnitude);
                Assert.AreEqual(1.25f, fourSqrMagnitude);
                Assert.AreEqual(0f,      fiveSqrMagnitude);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Normalized vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector2.Normalized))]
        public void Vector2_Normalized()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneNormalized   = one.Normalized;
            Vector2 twoNormalized   = two.Normalized;
            Vector2 threeNormalized = three.Normalized;
            Vector2 fourNormalized  = four.Normalized;
            Vector2 fiveNormalized  = five.Normalized;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.70710677f, 0.70710677f), oneNormalized);
                Assert.AreEqual(new Vector2(0.37139067f, -0.9284767f), twoNormalized);
                Assert.AreEqual(new Vector2(0f,          1f      ),    threeNormalized);
                Assert.AreEqual(new Vector2(-0.8944272f, 0.4472136f),  fourNormalized);
                Assert.AreEqual(new Vector2(0f,          0f        ),  fiveNormalized);

                Assert.AreEqual(new Vector2(1f,  1f),  one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),  three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),  five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Negated vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector2.Negated))]
        public void Vector2_Negated()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneNegated   = one.Negated;
            Vector2 twoNegated   = two.Negated;
            Vector2 threeNegated = three.Negated;
            Vector2 fourNegated  = four.Negated;
            Vector2 fiveNegated  = five.Negated;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, -1f), oneNegated);
                Assert.AreEqual(new Vector2(-2f, 5f), twoNegated);
                Assert.AreEqual(new Vector2(0f,  -1f),  threeNegated);
                Assert.AreEqual(new Vector2(1f,  -0.5f), fourNegated);
                Assert.AreEqual(new Vector2(0f,  0f),  fiveNegated);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.IsValid))]
        public void Vector2_IsValid()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,                     1f                    );
            Vector2 two   = new Vector2(2f,                     -5f                    );
            Vector2 three = new Vector2(float.PositiveInfinity, 1f                     );
            Vector2 four  = new Vector2(-1f,                    float.NegativeInfinity);
            Vector2 five  = new Vector2(0f,                     0f                    );

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
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Skew))]
        public void Vector2_Skew()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneSkew   = one.Skew;
            Vector2 twoSkew   = two.Skew;
            Vector2 threeSkew = three.Skew;
            Vector2 fourSkew  = four.Skew;
            Vector2 fiveSkew  = five.Skew;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, 1f),   oneSkew);
                Assert.AreEqual(new Vector2(5f, 2f),    twoSkew);
                Assert.AreEqual(new Vector2(-1f,  0f),   threeSkew);
                Assert.AreEqual(new Vector2(-0.5f,  -1f), fourSkew);
                Assert.AreEqual(new Vector2(0f,  0f),    fiveSkew);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Testing if all constructors create correct vector. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Constructors()
        {
            // Arrange
            Vector2 one   = new Vector2(1f, 1f);
            Vector2 two   = new Vector2(5f);
            Vector2 three = new Vector2(new Vector2(1f, 2f));
            Vector2 four  = new Vector2(new Vector3(8f, 2f, 1f));

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1f, one.x);
                Assert.AreEqual(1f, one.y);

                Assert.AreEqual(5f, two.x);
                Assert.AreEqual(5f, two.y);

                Assert.AreEqual(1f, three.x);
                Assert.AreEqual(2f, three.y);

                Assert.AreEqual(8f, four.x);
                Assert.AreEqual(2f, four.y);
            });
        }

        /// <summary> Testing if setting values returns correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Set))]
        public void Vector2_Set()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            one.Set(2f, 3f);
            two.Set(-5f, 4f);
            three.Set(1f, 0f);
            four.Set(3f, 6f);
            five.Set(1f, 1f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(2f,  3f),  one);
                Assert.AreEqual(new Vector2(-5f, 4f), two);
                Assert.AreEqual(new Vector2(1f,  0f), three);
                Assert.AreEqual(new Vector2(3f,  6f),  four);
                Assert.AreEqual(new Vector2(1f,  1f),  five);
            });
        }

        /// <summary> Testing if setting zero on all axis returns correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.SetZero))]
        public void Vector2_SetZero()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);

            // Act
            one.SetZero();

            // Assert
            Assert.AreEqual(new Vector2(0f, 0f), one);
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Add))]
        public void Vector2_Add()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoSum    = Vector2.Add(one,   two);
            Vector2 threeFourSum = Vector2.Add(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(3f, -4f),  oneTwoSum);
                Assert.AreEqual(new Vector2(3f, 1.5f), threeFourSum);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Subtract))]
        public void Vector2_Subtract()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoSub    = Vector2.Subtract(one,   two);
            Vector2 threeFourSub = Vector2.Subtract(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, 6f), oneTwoSub);
                Assert.AreEqual(new Vector2(5f,  0.5f), threeFourSub);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Multiplying two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Multiply))]
        public void Vector2_Multiply_Vectors()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoMul    = Vector2.Multiply(one,   two);
            Vector2 threeFourMul = Vector2.Multiply(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(2f,  -5f), oneTwoMul);
                Assert.AreEqual(new Vector2(-4f, 0.5f), threeFourMul);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Multiplying vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Multiply))]
        public void Vector2_Multiply_VectorScalar()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            float   two = 4f;

            Vector2 three = new Vector2(4f, 1f);
            float   four  = -3f;

            // Act
            Vector2 oneTwoMul    = Vector2.Multiply(one,   two);
            Vector2 threeFourMul = Vector2.Multiply(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(4f,   4f), oneTwoMul);
                Assert.AreEqual(new Vector2(-12f, -3f), threeFourMul);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(4f, 1f), three);
            });
        }

        /// <summary> Dividing two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Divide))]
        public void Vector2_Divide_Vectors()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoDiv    = Vector2.Divide(one,   two);
            Vector2 threeFourDiv = Vector2.Divide(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.5f, -0.2f), oneTwoDiv);
                Assert.AreEqual(new Vector2(-4f,  2f),    threeFourDiv);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Dividing vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Divide))]
        public void Vector2_Divide_VectorScalar()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            float   two = 4f;

            Vector2 three = new Vector2(4f, 1f);
            float   four  = -2f;

            // Act
            Vector2 oneTwoDiv    = Vector2.Divide(one,   two);
            Vector2 threeFourDiv = Vector2.Divide(three, four);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.25f, 0.25f), oneTwoDiv);
                Assert.AreEqual(new Vector2(-2f,   -0.5f),    threeFourDiv);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(4f, 1f), three);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.GetNegated))]
        public void Vector2_GetNegated()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            // Act
            Vector2 oneNegated   = Vector2.GetNegated(one);
            Vector2 threeNegated = Vector2.GetNegated(two);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, -1f), oneNegated);
                Assert.AreEqual(new Vector2(-2f, 5f), threeNegated);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Negate))]
        public void Vector2_Negate()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            one.Negate();
            two.Negate();
            three.Negate();
            four.Negate();
            five.Negate();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, -1f), one);
                Assert.AreEqual(new Vector2(-2f, 5f), two);
                Assert.AreEqual(new Vector2(0f,  -1f),  three);
                Assert.AreEqual(new Vector2(1f,  -0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),  five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Length))]
        public void Vector2_Length()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneLength   = one.Length().RoundToDecimal(4);
            float twoLength   = two.Length().RoundToDecimal(4);
            float threeLength = three.Length().RoundToDecimal(4);
            float fourLength  = four.Length().RoundToDecimal(4);
            float fiveLength  = five.Length().RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1.41419995f, oneLength);
                Assert.AreEqual(5.38520002f, twoLength);
                Assert.AreEqual(1f,          threeLength);
                Assert.AreEqual(1.11800003f, fourLength);
                Assert.AreEqual(0f,          fiveLength);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.LengthSquared))]
        public void Vector2_LengthSquared()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneLengthSquared   = one.LengthSquared().RoundToDecimal(4);
            float twoLengthSquared   = two.LengthSquared().RoundToDecimal(4);
            float threeLengthSquared = three.LengthSquared().RoundToDecimal(4);
            float fourLengthSquared  = four.LengthSquared().RoundToDecimal(4);
            float fiveLengthSquared  = five.LengthSquared().RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2f,      oneLengthSquared);
                Assert.AreEqual(29f,     twoLengthSquared);
                Assert.AreEqual(1f,      threeLengthSquared);
                Assert.AreEqual(1.25f, fourLengthSquared);
                Assert.AreEqual(0f,      fiveLengthSquared);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Normalize))]
        public void Vector2_Normalize()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            one.Normalize();
            two.Normalize();
            three.Normalize();
            four.Normalize();
            five.Normalize();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.70710677f, 0.70710677f), one);
                Assert.AreEqual(new Vector2(0.37139067f, -0.9284767f), two);
                Assert.AreEqual(new Vector2(0f,          1f),          three);
                Assert.AreEqual(new Vector2(-0.8944272f, 0.4472136f),  four);
                Assert.AreEqual(new Vector2(0f,          0f),          five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Normalized vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector2.GetNormalized))]
        public void Vector2_GetNormalized()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneNormalized   = Vector2.GetNormalized(one);
            Vector2 twoNormalized   = Vector2.GetNormalized(two);
            Vector2 threeNormalized = Vector2.GetNormalized(three);
            Vector2 fourNormalized  = Vector2.GetNormalized(four);
            Vector2 fiveNormalized  = Vector2.GetNormalized(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.70710677f, 0.70710677f), oneNormalized);
                Assert.AreEqual(new Vector2(0.37139067f, -0.9284767f), twoNormalized);
                Assert.AreEqual(new Vector2(0f,          1f),          threeNormalized);
                Assert.AreEqual(new Vector2(-0.8944272f, 0.4472136f),  fourNormalized);
                Assert.AreEqual(new Vector2(0f,          0f),          fiveNormalized);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Cross))]
        public void Vector2_Cross()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneCrossTwo    = one.Cross(two);
            float twoCrossThree  = two.Cross(three);
            float threeCrossFour = three.Cross(four);
            float fourCrossFive  = four.Cross(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual( -7f, oneCrossTwo);
                Assert.AreEqual( 2f,  twoCrossThree);
                Assert.AreEqual( 1f,  threeCrossFour);
                Assert.AreEqual( 0f,  fourCrossFive);

                Assert.AreEqual(new Vector2(1f,  1f),         one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Cross))]
        public void Vector2_Cross2()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            
            float four = 8f;
            float five = 2f;
            float six  = 7f;

            // Act
            Vector2 oneCrossFour  = Vector2.Cross(one, four);
            Vector2 twoCrossFive  = Vector2.Cross(two, five);
            Vector2 threeCrossSix = Vector2.Cross(three, six);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(8f, -8f), oneCrossFour);
                Assert.AreEqual(new Vector2(-10f, -4f), twoCrossFive);
                Assert.AreEqual(new Vector2(7f, 0f), threeCrossSix);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Cross))]
        public void Vector2_Cross3()
        {
            // Arrange
            Vector2 one   = new Vector2(1f, 1f);
            Vector2 two   = new Vector2(2f, -5f);
            Vector2 three = new Vector2(0f, 1f);
            
            float four = 8f;
            float five = 2f;
            float six  = 7f;

            // Act
            Vector2 oneCrossFour  = Vector2.Cross(four,  one);
            Vector2 twoCrossFive  = Vector2.Cross(five,  two);
            Vector2 threeCrossSix = Vector2.Cross(six, three);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-8f,   8f), oneCrossFour);
                Assert.AreEqual(new Vector2(10f, 4f), twoCrossFive);
                Assert.AreEqual(new Vector2(-7f,   0f),  threeCrossSix);

                Assert.AreEqual(new Vector2(1f, 1f),  one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
                Assert.AreEqual(new Vector2(0f, 1f),  three);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Distance))]
        public void Vector2_Distance()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneDistanceTwo    = one.Distance(two);
            float twoDistanceThree  = two.Distance(three);
            float threeDistanceFour = three.Distance(four);
            float fourDistanceFive  = four.Distance(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(6.08276272f, oneDistanceTwo);
                Assert.AreEqual(6.3245554f,  twoDistanceThree);
                Assert.AreEqual(1.11803401f, threeDistanceFour);
                Assert.AreEqual(1.11803401f, fourDistanceFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.DistanceSquared))]
        public void Vector2_DistanceSquared()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneDistanceSquaredTwo    = one.DistanceSquared(two);
            float twoDistanceSquaredThree  = two.DistanceSquared(three);
            float threeDistanceSquaredFour = three.DistanceSquared(four);
            float fourDistanceSquaredFive  = four.DistanceSquared(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(37f,     oneDistanceSquaredTwo);
                Assert.AreEqual(40f,     twoDistanceSquaredThree);
                Assert.AreEqual(1.25f, threeDistanceSquaredFour);
                Assert.AreEqual(1.25f, fourDistanceSquaredFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Dot))]
        public void Vector2_Dot()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            float oneDotTwo    = one.Dot(two);
            float twoDotThree  = two.Dot(three);
            float threeDotFour = three.Dot(four);
            float fourDotFive  = four.Dot(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(-3f,   oneDotTwo);
                Assert.AreEqual(-5f,  twoDotThree);
                Assert.AreEqual(0.5f, threeDotFour);
                Assert.AreEqual(0f,   fourDotFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Reflect))]
        public void Vector2_Reflect()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneReflectTwo    = one.Reflect(two);
            Vector2 twoReflectThree  = two.Reflect(three);
            Vector2 threeReflectFour = three.Reflect(four);
            Vector2 fourReflectFive  = four.Reflect(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(13f, -29f),  oneReflectTwo);
                Assert.AreEqual(new Vector2(2f,  5f),   twoReflectThree);
                Assert.AreEqual(new Vector2(1f,  0.5f),  threeReflectFour);
                Assert.AreEqual(new Vector2(-1f, 0.5f), fourReflectFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Clamp))]
        public void Vector2_Clamp()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 oneMin = new Vector2(0.5f, 2f);
            Vector2 oneMax = new Vector2(3f,   4f);
            Vector2 twoMin = new Vector2(2.5f, -2f);
            Vector2 twoMax = new Vector2(5f,   7f);

            // Act
            Vector2 oneClamp = Vector2.Clamp(one, oneMin, oneMax);
            Vector2 twoClamp = Vector2.Clamp(two, twoMin, twoMax);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1f,   2f), oneClamp);
                Assert.AreEqual(new Vector2(2.5f, -2f),   twoClamp);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Lerp))]
        public void Vector2_Lerp()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneLerpTwo    = Vector2.Lerp(one,   two,   0.25f);
            Vector2 twoLerpThree  = Vector2.Lerp(two,   three, 0.25f);
            Vector2 threeLerpFour = Vector2.Lerp(three, four,  0.25f);
            Vector2 fourLerpFive  = Vector2.Lerp(four,  five,  0.25f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1.25f,  -0.5f), oneLerpTwo);
                Assert.AreEqual(new Vector2(1.5f,   -3.5f),    twoLerpThree);
                Assert.AreEqual(new Vector2(-0.25f, 0.875f), threeLerpFour);
                Assert.AreEqual(new Vector2(-0.75f, 0.375f), fourLerpFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Lerp))]
        public void Vector2_Lerp2()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneLerpTwo    = Vector2.Lerp(one,   two,   0.5f);
            Vector2 twoLerpThree  = Vector2.Lerp(two,   three, 0.5f);
            Vector2 threeLerpFour = Vector2.Lerp(three, four,  0.5f);
            Vector2 fourLerpFive  = Vector2.Lerp(four,  five,  0.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1.5f,  -2f),   oneLerpTwo);
                Assert.AreEqual(new Vector2(1f,    -2f),   twoLerpThree);
                Assert.AreEqual(new Vector2(-0.5f, 0.75f), threeLerpFour);
                Assert.AreEqual(new Vector2(-0.5f, 0.25f), fourLerpFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Lerp))]
        public void Vector2_Lerp3()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneLerpTwo    = Vector2.Lerp(one,   two,   0.75f);
            Vector2 twoLerpThree  = Vector2.Lerp(two,   three, 0.75f);
            Vector2 threeLerpFour = Vector2.Lerp(three, four,  0.75f);
            Vector2 fourLerpFive  = Vector2.Lerp(four,  five,  0.75f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1.75f,  -3.5f),  oneLerpTwo);
                Assert.AreEqual(new Vector2(0.5f,   -0.5f),  twoLerpThree);
                Assert.AreEqual(new Vector2(-0.75f, 0.625f), threeLerpFour);
                Assert.AreEqual(new Vector2(-0.25f, 0.125f), fourLerpFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Max))]
        public void Vector2_Max()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneMaxTwo    = Vector2.Max(one,   two);
            Vector2 twoMaxThree  = Vector2.Max(two,   three);
            Vector2 threeMaxFour = Vector2.Max(three, four);
            Vector2 fourMaxFive  = Vector2.Max(four,  five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(2f, 1f), oneMaxTwo);
                Assert.AreEqual(new Vector2(2f, 1f), twoMaxThree);
                Assert.AreEqual(new Vector2(0f, 1f), threeMaxFour);
                Assert.AreEqual(new Vector2(0f, 0.5f), fourMaxFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Min))]
        public void Vector2_Min()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneMinTwo    = Vector2.Min(one,   two);
            Vector2 twoMinThree  = Vector2.Min(two,   three);
            Vector2 threeMinFour = Vector2.Min(three, four);
            Vector2 fourMinFive  = Vector2.Min(four,  five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1f,  -5f),   oneMinTwo);
                Assert.AreEqual(new Vector2(0f,  -5f),   twoMinThree);
                Assert.AreEqual(new Vector2(-1f, 0.5f), threeMinFour);
                Assert.AreEqual(new Vector2(-1f, 0f), fourMinFive);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Abs))]
        public void Vector2_Abs()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneAbs   = Vector2.Abs(one);
            Vector2 twoAbs   = Vector2.Abs(two);
            Vector2 threeAbs = Vector2.Abs(three);
            Vector2 fourAbs  = Vector2.Abs(four);
            Vector2 fiveAbs  = Vector2.Abs(five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1f,  1f),   oneAbs);
                Assert.AreEqual(new Vector2(2f,  5f),   twoAbs);
                Assert.AreEqual(new Vector2(0f,  1f),   threeAbs);
                Assert.AreEqual(new Vector2(1f, 0.5f), fourAbs);
                Assert.AreEqual(new Vector2(0f,  0f),   fiveAbs);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.RoundToDecimal))]
        public void Vector2_RoundToDecimal()
        {
            // Arrange
            Vector2 one   = new Vector2(1.32532f,  1.456765f);
            Vector2 two   = new Vector2(2.45645f,  -5.24567f);
            Vector2 three = new Vector2(0.4567f,  1.234556f);
            Vector2 four  = new Vector2(-1.45623f, 0.54577f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            one.RoundToDecimal(1);
            two.RoundToDecimal(1);
            three.RoundToDecimal(1);
            four.RoundToDecimal(1);
            five.RoundToDecimal(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1.3000001f, 1.5f),        one);
                Assert.AreEqual(new Vector2(2.5f,       -5.2000003f), two);
                Assert.AreEqual(new Vector2(0.5f,       1.2f),        three);
                Assert.AreEqual(new Vector2(-1.5f,      0.5f),        four);
                Assert.AreEqual(new Vector2(0f,         0f),          five);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Rotate))]
        public void Vector2_Rotate()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneAbs   = Vector2.Rotate(50f, one);
            Vector2 twoAbs   = Vector2.Rotate(50f, two);
            Vector2 threeAbs = Vector2.Rotate(50f, three);
            Vector2 fourAbs  = Vector2.Rotate(50f, four);
            Vector2 fiveAbs  = Vector2.Rotate(50f, five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1.2273408f,   0.7025912f),  oneAbs);
                Assert.AreEqual(new Vector2(0.6180577f,   -5.34958f),   twoAbs);
                Assert.AreEqual(new Vector2(0.26237485f,  0.964966f),   threeAbs);
                Assert.AreEqual(new Vector2(-0.83377856f, 0.74485785f), fourAbs);
                Assert.AreEqual(new Vector2(0f,           0f),          fiveAbs);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.RotateInverse))]
        public void Vector2_RotateInverse()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneAbs   = Vector2.RotateInverse(50f, one);
            Vector2 twoAbs   = Vector2.RotateInverse(50f, two);
            Vector2 threeAbs = Vector2.RotateInverse(50f, three);
            Vector2 fourAbs  = Vector2.RotateInverse(50f, four);
            Vector2 fiveAbs  = Vector2.RotateInverse(50f, five);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.7025912f,   1.2273408f),  oneAbs);
                Assert.AreEqual(new Vector2(3.2418063f,   -4.3000803f), twoAbs);
                Assert.AreEqual(new Vector2(-0.26237485f, 0.964966f),   threeAbs);
                Assert.AreEqual(new Vector2(-1.0961534f,  0.22010815f), fourAbs);
                Assert.AreEqual(new Vector2(0f,           0f),          fiveAbs);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),  two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.ShortenLength))]
        public void Vector2_ShortenLength()
        {
            // Arrange
            Vector2 one   = new Vector2(1.32532f,  1.456765f);
            Vector2 two   = new Vector2(2.45645f,  -5.24567f);
            Vector2 three = new Vector2(0.4567f,   1.234556f);
            Vector2 four  = new Vector2(-0.5f, 0.5f);
            Vector2 five  = new Vector2(0f,        0f);

            // Act
            one.ShortenLength(1);
            two.ShortenLength(1);
            three.ShortenLength(1);
            four.ShortenLength(1);
            five.ShortenLength(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.65237254f,  0.71707475f), one);
                Assert.AreEqual(new Vector2(2.032364f,    -4.340048f),  two);
                Assert.AreEqual(new Vector2(0.10974839f,  0.29667294f), three);
                Assert.AreEqual(new Vector2(0f, 0f), four);
                Assert.AreEqual(new Vector2(0f,           0f),          five);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.MoveTowards))]
        public void Vector2_MoveTowards()
        {
            // Arrange
            Vector2 one   = new Vector2(1.32532f, 1.456765f);
            Vector2 two   = new Vector2(2.45645f, -5.24567f);
            Vector2 three = new Vector2(0.4567f,  1.234556f);
            Vector2 four  = new Vector2(-0.5f,    0.5f);
            Vector2 five  = new Vector2(0f,       0f);

            float maxDistance = 0.25f;
            
            // Act
            Vector2 oneMoveTowardsTwo    = Vector2.MoveTowards(one,   two,   maxDistance);
            Vector2 twoMoveTowardsThree  = Vector2.MoveTowards(two,   three, maxDistance);
            Vector2 threeMoveTowardsFour = Vector2.MoveTowards(three, four,  maxDistance);
            Vector2 fourMoveTowardsFive  = Vector2.MoveTowards(four,  five,   maxDistance);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1.3669227f,  1.2102509f),  oneMoveTowardsTwo);
                Assert.AreEqual(new Vector2(2.3827322f,  -5.0067854f), twoMoveTowardsThree);
                Assert.AreEqual(new Vector2(0.25840715f, 1.0823064f),  threeMoveTowardsFour);
                Assert.AreEqual(new Vector2(-0.3232233f, 0.3232233f),  fourMoveTowardsFive);
            });
        }
        
        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Equals))]
        public void Vector2_Equals()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(one.Equals(one));
                Assert.IsFalse(one.Equals(two));
                Assert.IsFalse(one.Equals(new object()));

                Assert.AreEqual(new Vector2(1f, 1f),  one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.GetHashCode))]
        public void Vector2_GetHashCode()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(268435456,   one.GetHashCode());
                Assert.AreEqual(-2136997888, two.GetHashCode());

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
            });
        }

        /// <summary> Testing operator ==. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Eq()
        {
            // Arrange
            Vector2 one   = new Vector2(1f, 1f);
            Vector2 two   = new Vector2(2f, -5f);
            Vector2 three = new Vector2(1f, 1f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(one == three);
                Assert.IsFalse(one == two);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
                Assert.AreEqual(new Vector2(1f, 1f), three);
            });
        }

        /// <summary> Testing operator !=. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Neq()
        {
            // Arrange
            Vector2 one   = new Vector2(1f, 1f);
            Vector2 two   = new Vector2(2f, -5f);
            Vector2 three = new Vector2(1f, 1f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(one != three);
                Assert.IsTrue(one != two);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(2f, -5f), two);
                Assert.AreEqual(new Vector2(1f, 1f), three);
            });
        }

        /// <summary> Testing operator single -. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Single()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,  1f);
            Vector2 two   = new Vector2(2f,  -5f);
            Vector2 three = new Vector2(0f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);
            Vector2 five  = new Vector2(0f,  0f);

            // Act
            Vector2 oneNegated   = -one;
            Vector2 twoNegated   = -two;
            Vector2 threeNegated = -three;
            Vector2 fourNegated  = -four;
            Vector2 fiveNegated  = -five;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, -1f), oneNegated);
                Assert.AreEqual(new Vector2(-2f, 5f), twoNegated);
                Assert.AreEqual(new Vector2(0f,  -1f),  threeNegated);
                Assert.AreEqual(new Vector2(1f,  -0.5f), fourNegated);
                Assert.AreEqual(new Vector2(0f,  0f),  fiveNegated);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(0f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
                Assert.AreEqual(new Vector2(0f,  0f),   five);
            });
        }

        /// <summary> Testing operator +. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Plus()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoSum    = one + two;
            Vector2 threeFourSum = three + four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(3f, -4f),   oneTwoSum);
                Assert.AreEqual(new Vector2(3f, 1.5f), threeFourSum);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Testing operator -. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Minus()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoSub    = one - two;
            Vector2 threeFourSub = three - four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(-1f, 6f), oneTwoSub);
                Assert.AreEqual(new Vector2(5f,  0.5f), threeFourSub);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Testing operator *. Multiplying two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Star()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoMul    = one * two;
            Vector2 threeFourMul = three * four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(2f,  -5f), oneTwoMul);
                Assert.AreEqual(new Vector2(-4f, 0.5f), threeFourMul);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Testing operator *. Multiplying vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_StarScalar()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            float   two = 4f;

            Vector2 three = new Vector2(4f, 1f);
            float   four  = -3f;

            // Act
            Vector2 oneTwoMul    = one * two;
            Vector2 threeFourMul = three * four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(4f,   4f), oneTwoMul);
                Assert.AreEqual(new Vector2(-12f, -3f), threeFourMul);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(4f, 1f), three);
            });
        }

        /// <summary> Testing operator /. Dividing two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_Slash()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            Vector2 two = new Vector2(2f, -5f);

            Vector2 three = new Vector2(4f,  1f);
            Vector2 four  = new Vector2(-1f, 0.5f);

            // Act
            Vector2 oneTwoDiv    = one / two;
            Vector2 threeFourDiv = three / four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.5f, -0.2f), oneTwoDiv);
                Assert.AreEqual(new Vector2(-4f,  2f),    threeFourDiv);

                Assert.AreEqual(new Vector2(1f,  1f),   one);
                Assert.AreEqual(new Vector2(2f,  -5f),   two);
                Assert.AreEqual(new Vector2(4f,  1f),   three);
                Assert.AreEqual(new Vector2(-1f, 0.5f), four);
            });
        }

        /// <summary> Testing operator /. Dividing vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Operator_SlashScalar()
        {
            // Arrange
            Vector2 one = new Vector2(1f, 1f);
            float   two = 4f;

            Vector2 three = new Vector2(4f, 1f);
            float   four  = -2f;

            // Act
            Vector2 oneTwoDiv    = one / two;
            Vector2 threeFourDiv = three / four;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0.25f, 0.25f), oneTwoDiv);
                Assert.AreEqual(new Vector2(-2f,   -0.5f),    threeFourDiv);

                Assert.AreEqual(new Vector2(1f, 1f), one);
                Assert.AreEqual(new Vector2(4f, 1f), three);
            });
        }

        /// <summary> Testing implicit operator from Vector2 to Vector3. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Vector3()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);
            Vector3 two = new Vector3(7f, 8f, 9f);

            // Act
            Vector2 newOne = one;
            Vector2 newTwo = two;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(1,  1f), newOne);
                Assert.AreEqual(new Vector2(7f, 8f), newTwo);

                Assert.AreEqual(new Vector3(1f, 1f, 1f), one);
                Assert.AreEqual(new Vector3(7f, 8f, 9f), two);
            });
        }
        
        /// <summary> Testing bracket access operator. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Bracket()
        {
            // Arrange
            Vector2 two = new Vector2(7f, 8f);
            Vector2 one = new Vector2(1f, 1f);

            // Act
            two[0] = 10f;
            two[1] = 11f;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1f,  one[0]);
                Assert.AreEqual(1f,  one[1]);
                Assert.AreEqual(10f, two[0]);
                Assert.AreEqual(11f, two[1]);
            });
        }
    }
}