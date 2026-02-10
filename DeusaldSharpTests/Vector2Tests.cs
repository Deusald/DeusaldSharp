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
                Assert.That(Vector2.Zero.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector2.Zero.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector2.One.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector2.One.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector2.Up.x,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector2.Up.y,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector2.Down.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector2.Down.y,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(Vector2.Right.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector2.Right.y, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector2.Left.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(Vector2.Left.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneMagnitude,   Is.EqualTo(1.41419995f));
                Assert.That(twoMagnitude,   Is.EqualTo(5.38520002f));
                Assert.That(threeMagnitude, Is.EqualTo(1.0f));
                Assert.That(fourMagnitude,  Is.EqualTo(1.11800003f));
                Assert.That(fiveMagnitude,  Is.EqualTo(0f));
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
                Assert.That(oneSqrMagnitude,   Is.EqualTo(2f));
                Assert.That(twoSqrMagnitude,   Is.EqualTo(29f));
                Assert.That(threeSqrMagnitude, Is.EqualTo(1f));
                Assert.That(fourSqrMagnitude,  Is.EqualTo(1.25f));
                Assert.That(fiveSqrMagnitude,  Is.EqualTo(0f));
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
                Assert.That(oneNormalized.x,   Is.EqualTo(0.70710677f).Within(1e-5f));
                Assert.That(oneNormalized.y,   Is.EqualTo(0.70710677f).Within(1e-5f));
                Assert.That(twoNormalized.x,   Is.EqualTo(0.37139067f).Within(1e-5f));
                Assert.That(twoNormalized.y,   Is.EqualTo(-0.9284767f).Within(1e-5f));
                Assert.That(threeNormalized.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNormalized.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourNormalized.x,  Is.EqualTo(-0.8944272f).Within(1e-5f));
                Assert.That(fourNormalized.y,  Is.EqualTo(0.4472136f).Within(1e-5f));
                Assert.That(fiveNormalized.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNormalized.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneNegated.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(twoNegated.x,   Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoNegated.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeNegated.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNegated.y, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fourNegated.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourNegated.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fiveNegated.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNegated.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.IsValid))]
        public void Vector2_IsValid()
        {
            // Arrange
            Vector2 one   = new Vector2(1f,                     1f);
            Vector2 two   = new Vector2(2f,                     -5f);
            Vector2 three = new Vector2(float.PositiveInfinity, 1f);
            Vector2 four  = new Vector2(-1f,                    float.NegativeInfinity);
            Vector2 five  = new Vector2(0f,                     0f);

            // Act
            bool oneIsValid   = one.IsValid;
            bool twoIsValid   = two.IsValid;
            bool threeIsValid = three.IsValid;
            bool fourIsValid  = four.IsValid;
            bool fiveIsValid  = five.IsValid;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneIsValid,   Is.True);
                Assert.That(twoIsValid,   Is.True);
                Assert.That(threeIsValid, Is.False);
                Assert.That(fourIsValid,  Is.False);
                Assert.That(fiveIsValid,  Is.True);
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
                Assert.That(oneSkew.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneSkew.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoSkew.x,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(twoSkew.y,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(threeSkew.x, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeSkew.y, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourSkew.x,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fourSkew.y,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fiveSkew.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveSkew.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(one.x, Is.EqualTo(1f));
                Assert.That(one.y, Is.EqualTo(1f));

                Assert.That(two.x, Is.EqualTo(5f));
                Assert.That(two.y, Is.EqualTo(5f));

                Assert.That(three.x, Is.EqualTo(1f));
                Assert.That(three.y, Is.EqualTo(2f));

                Assert.That(four.x, Is.EqualTo(8f));
                Assert.That(four.y, Is.EqualTo(2f));
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
                Assert.That(one.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(3f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(3f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(6f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(1f).Within(1e-5f));
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
            Assert.That(one.x, Is.EqualTo(0f).Within(1e-5f));
            Assert.That(one.y, Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneTwoSum.x,    Is.EqualTo(3f).Within(1e-5f));
                Assert.That(oneTwoSum.y,    Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourSum.x, Is.EqualTo(3f).Within(1e-5f));
                Assert.That(threeFourSum.y, Is.EqualTo(1.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoSub.x,    Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneTwoSub.y,    Is.EqualTo(6f).Within(1e-5f));
                Assert.That(threeFourSub.x, Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeFourSub.y, Is.EqualTo(0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-12f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(-3f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(-0.2f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(2f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(-0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(oneNegated.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeNegated.x, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeNegated.y, Is.EqualTo(5f).Within(1e-5f));

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
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
                Assert.That(one.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneLength,   Is.EqualTo(1.41419995f));
                Assert.That(twoLength,   Is.EqualTo(5.38520002f));
                Assert.That(threeLength, Is.EqualTo(1f));
                Assert.That(fourLength,  Is.EqualTo(1.11800003f));
                Assert.That(fiveLength,  Is.EqualTo(0f));
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
                Assert.That(oneLengthSquared,   Is.EqualTo(2f));
                Assert.That(twoLengthSquared,   Is.EqualTo(29f));
                Assert.That(threeLengthSquared, Is.EqualTo(1f));
                Assert.That(fourLengthSquared,  Is.EqualTo(1.25f));
                Assert.That(fiveLengthSquared,  Is.EqualTo(0f));
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
                Assert.That(one.x,   Is.EqualTo(0.70710677f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(0.70710677f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(0.37139067f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-0.9284767f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-0.8944272f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.4472136f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneNormalized.x,   Is.EqualTo(0.70710677f).Within(1e-5f));
                Assert.That(oneNormalized.y,   Is.EqualTo(0.70710677f).Within(1e-5f));
                Assert.That(twoNormalized.x,   Is.EqualTo(0.37139067f).Within(1e-5f));
                Assert.That(twoNormalized.y,   Is.EqualTo(-0.9284767f).Within(1e-5f));
                Assert.That(threeNormalized.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNormalized.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourNormalized.x,  Is.EqualTo(-0.8944272f).Within(1e-5f));
                Assert.That(fourNormalized.y,  Is.EqualTo(0.4472136f).Within(1e-5f));
                Assert.That(fiveNormalized.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNormalized.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneCrossTwo,    Is.EqualTo(-7f));
                Assert.That(twoCrossThree,  Is.EqualTo(2f));
                Assert.That(threeCrossFour, Is.EqualTo(1f));
                Assert.That(fourCrossFive,  Is.EqualTo(0f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.Cross))]
        public void Vector2_Cross2()
        {
            // Arrange
            Vector2 one   = new Vector2(1f, 1f);
            Vector2 two   = new Vector2(2f, -5f);
            Vector2 three = new Vector2(0f, 1f);

            float four = 8f;
            float five = 2f;
            float six  = 7f;

            // Act
            Vector2 oneCrossFour  = Vector2.Cross(one,   four);
            Vector2 twoCrossFive  = Vector2.Cross(two,   five);
            Vector2 threeCrossSix = Vector2.Cross(three, six);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneCrossFour.x,  Is.EqualTo(8f).Within(1e-5f));
                Assert.That(oneCrossFour.y,  Is.EqualTo(-8f).Within(1e-5f));
                Assert.That(twoCrossFive.x,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(twoCrossFive.y,  Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeCrossSix.x, Is.EqualTo(7f).Within(1e-5f));
                Assert.That(threeCrossSix.y, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
            Vector2 oneCrossFour  = Vector2.Cross(four, one);
            Vector2 twoCrossFive  = Vector2.Cross(five, two);
            Vector2 threeCrossSix = Vector2.Cross(six,  three);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneCrossFour.x,  Is.EqualTo(-8f).Within(1e-5f));
                Assert.That(oneCrossFour.y,  Is.EqualTo(8f).Within(1e-5f));
                Assert.That(twoCrossFive.x,  Is.EqualTo(10f).Within(1e-5f));
                Assert.That(twoCrossFive.y,  Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeCrossSix.x, Is.EqualTo(-7f).Within(1e-5f));
                Assert.That(threeCrossSix.y, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(oneDistanceTwo,    Is.EqualTo(6.08276272f));
                Assert.That(twoDistanceThree,  Is.EqualTo(6.3245554f));
                Assert.That(threeDistanceFour, Is.EqualTo(1.11803401f));
                Assert.That(fourDistanceFive,  Is.EqualTo(1.11803401f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneDistanceSquaredTwo,    Is.EqualTo(37f));
                Assert.That(twoDistanceSquaredThree,  Is.EqualTo(40f));
                Assert.That(threeDistanceSquaredFour, Is.EqualTo(1.25f));
                Assert.That(fourDistanceSquaredFive,  Is.EqualTo(1.25f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneDotTwo,    Is.EqualTo(-3f));
                Assert.That(twoDotThree,  Is.EqualTo(-5f));
                Assert.That(threeDotFour, Is.EqualTo(0.5f));
                Assert.That(fourDotFive,  Is.EqualTo(0f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneReflectTwo.x,    Is.EqualTo(13f).Within(1e-5f));
                Assert.That(oneReflectTwo.y,    Is.EqualTo(-29f).Within(1e-5f));
                Assert.That(twoReflectThree.x,  Is.EqualTo(2f).Within(1e-5f));
                Assert.That(twoReflectThree.y,  Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeReflectFour.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeReflectFour.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(fourReflectFive.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fourReflectFive.y,  Is.EqualTo(0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneClamp.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(oneClamp.y, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(twoClamp.x, Is.EqualTo(2.5f).Within(1e-5f));
                Assert.That(twoClamp.y, Is.EqualTo(-2f).Within(1e-5f));

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
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
                Assert.That(oneLerpTwo.x,    Is.EqualTo(1.25f).Within(1e-5f));
                Assert.That(oneLerpTwo.y,    Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(twoLerpThree.x,  Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(twoLerpThree.y,  Is.EqualTo(-3.5f).Within(1e-5f));
                Assert.That(threeLerpFour.x, Is.EqualTo(-0.25f).Within(1e-5f));
                Assert.That(threeLerpFour.y, Is.EqualTo(0.875f).Within(1e-5f));
                Assert.That(fourLerpFive.x,  Is.EqualTo(-0.75f).Within(1e-5f));
                Assert.That(fourLerpFive.y,  Is.EqualTo(0.375f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneLerpTwo.x,    Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(oneLerpTwo.y,    Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoLerpThree.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoLerpThree.y,  Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeLerpFour.x, Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(threeLerpFour.y, Is.EqualTo(0.75f).Within(1e-5f));
                Assert.That(fourLerpFive.x,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fourLerpFive.y,  Is.EqualTo(0.25f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneLerpTwo.x,    Is.EqualTo(1.75f).Within(1e-5f));
                Assert.That(oneLerpTwo.y,    Is.EqualTo(-3.5f).Within(1e-5f));
                Assert.That(twoLerpThree.x,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(twoLerpThree.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(threeLerpFour.x, Is.EqualTo(-0.75f).Within(1e-5f));
                Assert.That(threeLerpFour.y, Is.EqualTo(0.625f).Within(1e-5f));
                Assert.That(fourLerpFive.x,  Is.EqualTo(-0.25f).Within(1e-5f));
                Assert.That(fourLerpFive.y,  Is.EqualTo(0.125f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneMaxTwo.x,    Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneMaxTwo.y,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoMaxThree.x,  Is.EqualTo(2f).Within(1e-5f));
                Assert.That(twoMaxThree.y,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeMaxFour.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeMaxFour.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourMaxFive.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourMaxFive.y,  Is.EqualTo(0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneMinTwo.x,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(oneMinTwo.y,    Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(twoMinThree.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(twoMinThree.y,  Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(threeMinFour.x, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeMinFour.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(fourMinFive.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fourMinFive.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneAbs.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(oneAbs.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoAbs.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(twoAbs.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeAbs.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeAbs.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourAbs.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourAbs.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(fiveAbs.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveAbs.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
            Vector2 three = new Vector2(0.4567f,   1.234556f);
            Vector2 four  = new Vector2(-1.45623f, 0.54577f);
            Vector2 five  = new Vector2(0f,        0f);

            // Act
            one.RoundToDecimal(1);
            two.RoundToDecimal(1);
            three.RoundToDecimal(1);
            four.RoundToDecimal(1);
            five.RoundToDecimal(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one.x,   Is.EqualTo(1.3000001f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2.5f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5.2000003f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1.2f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1.5f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneAbs.x,   Is.EqualTo(1.2273408f).Within(1e-5f));
                Assert.That(oneAbs.y,   Is.EqualTo(0.7025912f).Within(1e-5f));
                Assert.That(twoAbs.x,   Is.EqualTo(0.6180577f).Within(1e-5f));
                Assert.That(twoAbs.y,   Is.EqualTo(-5.34958f).Within(1e-5f));
                Assert.That(threeAbs.x, Is.EqualTo(0.26237485f).Within(1e-5f));
                Assert.That(threeAbs.y, Is.EqualTo(0.964966f).Within(1e-5f));
                Assert.That(fourAbs.x,  Is.EqualTo(-0.83377856f).Within(1e-5f));
                Assert.That(fourAbs.y,  Is.EqualTo(0.74485785f).Within(1e-5f));
                Assert.That(fiveAbs.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveAbs.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneAbs.x,   Is.EqualTo(0.7025912f).Within(1e-5f));
                Assert.That(oneAbs.y,   Is.EqualTo(1.2273408f).Within(1e-5f));
                Assert.That(twoAbs.x,   Is.EqualTo(3.2418063f).Within(1e-5f));
                Assert.That(twoAbs.y,   Is.EqualTo(-4.3000803f).Within(1e-5f));
                Assert.That(threeAbs.x, Is.EqualTo(-0.26237485f).Within(1e-5f));
                Assert.That(threeAbs.y, Is.EqualTo(0.964966f).Within(1e-5f));
                Assert.That(fourAbs.x,  Is.EqualTo(-1.0961534f).Within(1e-5f));
                Assert.That(fourAbs.y,  Is.EqualTo(0.22010815f).Within(1e-5f));
                Assert.That(fiveAbs.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveAbs.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector2.ShortenLength))]
        public void Vector2_ShortenLength()
        {
            // Arrange
            Vector2 one   = new Vector2(1.32532f, 1.456765f);
            Vector2 two   = new Vector2(2.45645f, -5.24567f);
            Vector2 three = new Vector2(0.4567f,  1.234556f);
            Vector2 four  = new Vector2(-0.5f,    0.5f);
            Vector2 five  = new Vector2(0f,       0f);

            // Act
            one.ShortenLength(1);
            two.ShortenLength(1);
            three.ShortenLength(1);
            four.ShortenLength(1);
            five.ShortenLength(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one.x,   Is.EqualTo(0.65237254f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(0.71707475f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2.032364f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-4.340048f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0.10974839f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(0.29667294f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
            Vector2 fourMoveTowardsFive  = Vector2.MoveTowards(four,  five,  maxDistance);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneMoveTowardsTwo.x,    Is.EqualTo(1.3669227f).Within(1e-5f));
                Assert.That(oneMoveTowardsTwo.y,    Is.EqualTo(1.2102509f).Within(1e-5f));
                Assert.That(twoMoveTowardsThree.x,  Is.EqualTo(2.3827322f).Within(1e-5f));
                Assert.That(twoMoveTowardsThree.y,  Is.EqualTo(-5.0067854f).Within(1e-5f));
                Assert.That(threeMoveTowardsFour.x, Is.EqualTo(0.25840715f).Within(1e-5f));
                Assert.That(threeMoveTowardsFour.y, Is.EqualTo(1.0823064f).Within(1e-5f));
                Assert.That(fourMoveTowardsFive.x,  Is.EqualTo(-0.3232233f).Within(1e-5f));
                Assert.That(fourMoveTowardsFive.y,  Is.EqualTo(0.3232233f).Within(1e-5f));
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
                Assert.That(one.Equals(one),          Is.True);
                Assert.That(one.Equals(two),          Is.False);
                Assert.That(one.Equals(new object()), Is.False);

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
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
                Assert.That(one.GetHashCode(), Is.EqualTo(268435456));
                Assert.That(two.GetHashCode(), Is.EqualTo(-2136997888));

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
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
                Assert.That(one == three, Is.True);
                Assert.That(one == two,   Is.False);

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(one != three, Is.False);
                Assert.That(one != two,   Is.True);

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(oneNegated.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(twoNegated.x,   Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoNegated.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeNegated.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNegated.y, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fourNegated.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourNegated.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fiveNegated.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNegated.y,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
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
                Assert.That(oneTwoSum.x,    Is.EqualTo(3f).Within(1e-5f));
                Assert.That(oneTwoSum.y,    Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourSum.x, Is.EqualTo(3f).Within(1e-5f));
                Assert.That(threeFourSum.y, Is.EqualTo(1.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoSub.x,    Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneTwoSub.y,    Is.EqualTo(6f).Within(1e-5f));
                Assert.That(threeFourSub.x, Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeFourSub.y, Is.EqualTo(0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-12f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(-3f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(-0.2f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(2f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(-0.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
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
                Assert.That(newOne.x, Is.EqualTo(1).Within(1e-5f));
                Assert.That(newOne.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(newTwo.x, Is.EqualTo(7f).Within(1e-5f));
                Assert.That(newTwo.y, Is.EqualTo(8f).Within(1e-5f));

                Assert.That(one, Is.EqualTo(new Vector3(1f, 1f, 1f)));
                Assert.That(two, Is.EqualTo(new Vector3(7f, 8f, 9f)));
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
                Assert.That(one[0], Is.EqualTo(1f));
                Assert.That(one[1], Is.EqualTo(1f));
                Assert.That(two[0], Is.EqualTo(10f));
                Assert.That(two[1], Is.EqualTo(11f));
            });
        }
    }
}