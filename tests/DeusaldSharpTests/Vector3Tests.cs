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
        public void Vector3_Base()
        {
            // Arrange

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(Vector3.Zero.x,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Zero.y,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Zero.z,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.One.x,      Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.One.y,      Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.One.z,      Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.UnitX.x,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.UnitX.y,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.UnitX.z,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.UnitY.x,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.UnitY.y,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.UnitY.z,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.UnitZ.x,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.UnitZ.y,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.UnitZ.z,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.Up.x,       Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Up.y,       Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.Up.z,       Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Down.x,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Down.y,     Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(Vector3.Down.z,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Right.x,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(Vector3.Right.y,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Right.z,    Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Left.x,     Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(Vector3.Left.y,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Left.z,     Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Forward.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Forward.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Forward.z,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(Vector3.Backward.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Backward.y, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(Vector3.Backward.z, Is.EqualTo(1f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Magnitude))]
        public void Vector3_Magnitude()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneMagnitude   = one.Magnitude.RoundToDecimal(4);
            float twoMagnitude   = two.Magnitude.RoundToDecimal(4);
            float threeMagnitude = three.Magnitude.RoundToDecimal(4);
            float fourMagnitude  = four.Magnitude.RoundToDecimal(4);
            float fiveMagnitude  = five.Magnitude.RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneMagnitude,   Is.EqualTo(1.7321f));
                Assert.That(twoMagnitude,   Is.EqualTo(6.7082f));
                Assert.That(threeMagnitude, Is.EqualTo(1f));
                Assert.That(fourMagnitude,  Is.EqualTo(10.0623f));
                Assert.That(fiveMagnitude,  Is.EqualTo(0f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.SqrMagnitude))]
        public void Vector3_SqrMagnitude()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneSqrMagnitude   = one.SqrMagnitude.RoundToDecimal(4);
            float twoSqrMagnitude   = two.SqrMagnitude.RoundToDecimal(4);
            float threeSqrMagnitude = three.SqrMagnitude.RoundToDecimal(4);
            float fourSqrMagnitude  = four.SqrMagnitude.RoundToDecimal(4);
            float fiveSqrMagnitude  = five.SqrMagnitude.RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneSqrMagnitude,   Is.EqualTo(3f));
                Assert.That(twoSqrMagnitude,   Is.EqualTo(45f));
                Assert.That(threeSqrMagnitude, Is.EqualTo(1f));
                Assert.That(fourSqrMagnitude,  Is.EqualTo(101.25f));
                Assert.That(fiveSqrMagnitude,  Is.EqualTo(0f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Normalized vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector3.Normalized))]
        public void Vector3_Normalized()
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
                Assert.That(oneNormalized.x,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(oneNormalized.y,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(oneNormalized.z,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(twoNormalized.x,   Is.EqualTo(0.2981424f).Within(1e-5f));
                Assert.That(twoNormalized.y,   Is.EqualTo(-0.745356f).Within(1e-5f));
                Assert.That(twoNormalized.z,   Is.EqualTo(0.5962848f).Within(1e-5f));
                Assert.That(threeNormalized.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNormalized.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeNormalized.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourNormalized.x,  Is.EqualTo(-0.099380806f).Within(1e-5f));
                Assert.That(fourNormalized.y,  Is.EqualTo(0.049690403f).Within(1e-5f));
                Assert.That(fourNormalized.z,  Is.EqualTo(-0.99380803f).Within(1e-5f));
                Assert.That(fiveNormalized.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNormalized.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNormalized.z,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Negated vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector3.Negated))]
        public void Vector3_Negated()
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
                Assert.That(oneNegated.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.z,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(twoNegated.x,   Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoNegated.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(twoNegated.z,   Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeNegated.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNegated.y, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeNegated.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourNegated.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourNegated.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fourNegated.z,  Is.EqualTo(10f).Within(1e-5f));
                Assert.That(fiveNegated.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNegated.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNegated.z,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.IsValid))]
        public void Vector3_IsValid()
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
                Assert.That(oneIsValid,   Is.True);
                Assert.That(twoIsValid,   Is.True);
                Assert.That(threeIsValid, Is.False);
                Assert.That(fourIsValid,  Is.False);
                Assert.That(fiveIsValid,  Is.True);
            });
        }

        /// <summary> Testing if all constructors create correct vector. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Constructors()
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
                Assert.That(one.x, Is.EqualTo(1f));
                Assert.That(one.y, Is.EqualTo(1f));
                Assert.That(one.z, Is.EqualTo(1f));

                Assert.That(two.x, Is.EqualTo(5f));
                Assert.That(two.y, Is.EqualTo(5f));
                Assert.That(two.z, Is.EqualTo(5f));

                Assert.That(three.x, Is.EqualTo(1f));
                Assert.That(three.y, Is.EqualTo(2f));
                Assert.That(three.z, Is.EqualTo(3f));

                Assert.That(four.x, Is.EqualTo(6f));
                Assert.That(four.y, Is.EqualTo(7f));
                Assert.That(four.z, Is.EqualTo(0f));

                Assert.That(five.x, Is.EqualTo(4f));
                Assert.That(five.y, Is.EqualTo(2f));
                Assert.That(five.z, Is.EqualTo(0f));

                Assert.That(six.x, Is.EqualTo(8f));
                Assert.That(six.y, Is.EqualTo(2f));
                Assert.That(six.z, Is.EqualTo(1f));
            });
        }

        /// <summary> Testing if setting values returns correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Set))]
        public void Vector3_Set()
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
                Assert.That(one.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(3f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(88f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(3f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(6f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(3f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(1f).Within(1e-5f));
            });
        }

        /// <summary> Testing if setting zero on all axis returns correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.SetZero))]
        public void Vector3_SetZero()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);

            // Act
            one.SetZero();

            // Assert
            Assert.That(one.x, Is.EqualTo(0f).Within(1e-5f));
            Assert.That(one.y, Is.EqualTo(0f).Within(1e-5f));
            Assert.That(one.z, Is.EqualTo(0f).Within(1e-5f));
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Add))]
        public void Vector3_Add()
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
                Assert.That(oneTwoSum.x,    Is.EqualTo(3f).Within(1e-5f));
                Assert.That(oneTwoSum.y,    Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(oneTwoSum.z,    Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeFourSum.x, Is.EqualTo(3f).Within(1e-5f));
                Assert.That(threeFourSum.y, Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(threeFourSum.z, Is.EqualTo(-10f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Subtract))]
        public void Vector3_Subtract()
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
                Assert.That(oneTwoSub.x,    Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneTwoSub.y,    Is.EqualTo(6f).Within(1e-5f));
                Assert.That(oneTwoSub.z,    Is.EqualTo(-3f).Within(1e-5f));
                Assert.That(threeFourSub.x, Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeFourSub.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(threeFourSub.z, Is.EqualTo(10f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Multiplying two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Multiply))]
        public void Vector3_Multiply()
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(oneTwoMul.z,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(threeFourMul.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Multiplying vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Multiply))]
        public void Vector3_MultiplyScalar()
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(oneTwoMul.z,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-12f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(-3f).Within(1e-5f));
                Assert.That(threeFourMul.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Dividing two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Divide))]
        public void Vector3_Divide()
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(-0.2f).Within(1e-5f));
                Assert.That(oneTwoDiv.z,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(threeFourDiv.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Dividing vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Divide))]
        public void Vector3_DivideScalar()
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(oneTwoDiv.z,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(threeFourDiv.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.GetNegated))]
        public void Vector3_GetNegated()
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
                Assert.That(oneNegated.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.z,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeNegated.x, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeNegated.y, Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeNegated.z, Is.EqualTo(-4f).Within(1e-5f));

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z, Is.EqualTo(4f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Negate))]
        public void Vector3_Negate()
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
                Assert.That(one.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Length))]
        public void Vector3_Length()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneLength   = one.Length().RoundToDecimal(4);
            float twoLength   = two.Length().RoundToDecimal(4);
            float threeLength = three.Length().RoundToDecimal(4);
            float fourLength  = four.Length().RoundToDecimal(4);
            float fiveLength  = five.Length().RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneLength,   Is.EqualTo(1.7321f));
                Assert.That(twoLength,   Is.EqualTo(6.7082f));
                Assert.That(threeLength, Is.EqualTo(1f));
                Assert.That(fourLength,  Is.EqualTo(10.0623f));
                Assert.That(fiveLength,  Is.EqualTo(0f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.LengthSquared))]
        public void Vector3_LengthSquared()
        {
            // Arrange
            Vector3 one   = new Vector3(1f,  1f,   1f);
            Vector3 two   = new Vector3(2f,  -5f,  4f);
            Vector3 three = new Vector3(0f,  1f,   0f);
            Vector3 four  = new Vector3(-1f, 0.5f, -10f);
            Vector3 five  = new Vector3(0f,  0f,   0f);

            // Act
            float oneLengthSquared   = one.LengthSquared().RoundToDecimal(4);
            float twoLengthSquared   = two.LengthSquared().RoundToDecimal(4);
            float threeLengthSquared = three.LengthSquared().RoundToDecimal(4);
            float fourLengthSquared  = four.LengthSquared().RoundToDecimal(4);
            float fiveLengthSquared  = five.LengthSquared().RoundToDecimal(4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(oneLengthSquared,   Is.EqualTo(3f));
                Assert.That(twoLengthSquared,   Is.EqualTo(45f));
                Assert.That(threeLengthSquared, Is.EqualTo(1f));
                Assert.That(fourLengthSquared,  Is.EqualTo(101.25f));
                Assert.That(fiveLengthSquared,  Is.EqualTo(0f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Normalize))]
        public void Vector3_Normalize()
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
                Assert.That(one.x,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(0.2981424f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-0.745356f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(0.5962848f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-0.099380806f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.049690403f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-0.99380803f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results.
        /// After getting Normalized vector the original should remain the same. </summary>
        [Test]
        [TestOf(nameof(Vector3.GetNormalized))]
        public void Vector3_GetNormalized()
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
                Assert.That(oneNormalized.x,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(oneNormalized.y,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(oneNormalized.z,   Is.EqualTo(0.57735026f).Within(1e-5f));
                Assert.That(twoNormalized.x,   Is.EqualTo(0.2981424f).Within(1e-5f));
                Assert.That(twoNormalized.y,   Is.EqualTo(-0.745356f).Within(1e-5f));
                Assert.That(twoNormalized.z,   Is.EqualTo(0.5962848f).Within(1e-5f));
                Assert.That(threeNormalized.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNormalized.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeNormalized.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourNormalized.x,  Is.EqualTo(-0.099380806f).Within(1e-5f));
                Assert.That(fourNormalized.y,  Is.EqualTo(0.049690403f).Within(1e-5f));
                Assert.That(fourNormalized.z,  Is.EqualTo(-0.99380803f).Within(1e-5f));
                Assert.That(fiveNormalized.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNormalized.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNormalized.z,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Cross))]
        public void Vector3_Cross()
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
                Assert.That(oneCrossTwo.x,    Is.EqualTo(9f).Within(1e-5f));
                Assert.That(oneCrossTwo.y,    Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(oneCrossTwo.z,    Is.EqualTo(-7f).Within(1e-5f));
                Assert.That(twoCrossThree.x,  Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(twoCrossThree.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(twoCrossThree.z,  Is.EqualTo(2f).Within(1e-5f));
                Assert.That(threeCrossFour.x, Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(threeCrossFour.y, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeCrossFour.z, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourCrossFive.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourCrossFive.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourCrossFive.z,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Distance))]
        public void Vector3_Distance()
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
                Assert.That(oneDistanceTwo,    Is.EqualTo(6.78233f));
                Assert.That(twoDistanceThree,  Is.EqualTo(7.483315f));
                Assert.That(threeDistanceFour, Is.EqualTo(10.0623055f));
                Assert.That(fourDistanceFive,  Is.EqualTo(10.0623055f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.DistanceSquared))]
        public void Vector3_DistanceSquared()
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
                Assert.That(oneDistanceSquaredTwo,    Is.EqualTo(46f));
                Assert.That(twoDistanceSquaredThree,  Is.EqualTo(56f));
                Assert.That(threeDistanceSquaredFour, Is.EqualTo(101.25f));
                Assert.That(fourDistanceSquaredFive,  Is.EqualTo(101.25f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Dot))]
        public void Vector3_Dot()
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
                Assert.That(oneDotTwo,    Is.EqualTo(1f));
                Assert.That(twoDotThree,  Is.EqualTo(-5f));
                Assert.That(threeDotFour, Is.EqualTo(0.5f));
                Assert.That(fourDotFive,  Is.EqualTo(0f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Reflect))]
        public void Vector3_Reflect()
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
                Assert.That(oneReflectTwo.x,    Is.EqualTo(-3f).Within(1e-5f));
                Assert.That(oneReflectTwo.y,    Is.EqualTo(11f).Within(1e-5f));
                Assert.That(oneReflectTwo.z,    Is.EqualTo(-7f).Within(1e-5f));
                Assert.That(twoReflectThree.x,  Is.EqualTo(2f).Within(1e-5f));
                Assert.That(twoReflectThree.y,  Is.EqualTo(5f).Within(1e-5f));
                Assert.That(twoReflectThree.z,  Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeReflectFour.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeReflectFour.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(threeReflectFour.z, Is.EqualTo(10f).Within(1e-5f));
                Assert.That(fourReflectFive.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fourReflectFive.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(fourReflectFive.z,  Is.EqualTo(-10f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Clamp))]
        public void Vector3_Clamp()
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
                Assert.That(oneClamp.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(oneClamp.y, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneClamp.z, Is.EqualTo(0.7f).Within(1e-5f));
                Assert.That(twoClamp.x, Is.EqualTo(2.5f).Within(1e-5f));
                Assert.That(twoClamp.y, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoClamp.z, Is.EqualTo(3f).Within(1e-5f));

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z, Is.EqualTo(4f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Lerp))]
        public void Vector3_Lerp()
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
                Assert.That(oneLerpTwo.x,    Is.EqualTo(1.25f).Within(1e-5f));
                Assert.That(oneLerpTwo.y,    Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(oneLerpTwo.z,    Is.EqualTo(1.75f).Within(1e-5f));
                Assert.That(twoLerpThree.x,  Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(twoLerpThree.y,  Is.EqualTo(-3.5f).Within(1e-5f));
                Assert.That(twoLerpThree.z,  Is.EqualTo(3f).Within(1e-5f));
                Assert.That(threeLerpFour.x, Is.EqualTo(-0.25f).Within(1e-5f));
                Assert.That(threeLerpFour.y, Is.EqualTo(0.875f).Within(1e-5f));
                Assert.That(threeLerpFour.z, Is.EqualTo(-2.5f).Within(1e-5f));
                Assert.That(fourLerpFive.x,  Is.EqualTo(-0.75f).Within(1e-5f));
                Assert.That(fourLerpFive.y,  Is.EqualTo(0.375f).Within(1e-5f));
                Assert.That(fourLerpFive.z,  Is.EqualTo(-7.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Lerp))]
        public void Vector3_Lerp2()
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
                Assert.That(oneLerpTwo.x,    Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(oneLerpTwo.y,    Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(oneLerpTwo.z,    Is.EqualTo(2.5f).Within(1e-5f));
                Assert.That(twoLerpThree.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoLerpThree.y,  Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoLerpThree.z,  Is.EqualTo(2f).Within(1e-5f));
                Assert.That(threeLerpFour.x, Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(threeLerpFour.y, Is.EqualTo(0.75f).Within(1e-5f));
                Assert.That(threeLerpFour.z, Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(fourLerpFive.x,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fourLerpFive.y,  Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(fourLerpFive.z,  Is.EqualTo(-5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Lerp))]
        public void Vector3_Lerp3()
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
                Assert.That(oneLerpTwo.x,    Is.EqualTo(1.75f).Within(1e-5f));
                Assert.That(oneLerpTwo.y,    Is.EqualTo(-3.5f).Within(1e-5f));
                Assert.That(oneLerpTwo.z,    Is.EqualTo(3.25f).Within(1e-5f));
                Assert.That(twoLerpThree.x,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(twoLerpThree.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(twoLerpThree.z,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeLerpFour.x, Is.EqualTo(-0.75f).Within(1e-5f));
                Assert.That(threeLerpFour.y, Is.EqualTo(0.625f).Within(1e-5f));
                Assert.That(threeLerpFour.z, Is.EqualTo(-7.5f).Within(1e-5f));
                Assert.That(fourLerpFive.x,  Is.EqualTo(-0.25f).Within(1e-5f));
                Assert.That(fourLerpFive.y,  Is.EqualTo(0.125f).Within(1e-5f));
                Assert.That(fourLerpFive.z,  Is.EqualTo(-2.5f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Max))]
        public void Vector3_Max()
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
                Assert.That(oneMaxTwo.x,    Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneMaxTwo.y,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(oneMaxTwo.z,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(twoMaxThree.x,  Is.EqualTo(2f).Within(1e-5f));
                Assert.That(twoMaxThree.y,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoMaxThree.z,  Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeMaxFour.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeMaxFour.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(threeMaxFour.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourMaxFive.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourMaxFive.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(fourMaxFive.z,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Min))]
        public void Vector3_Min()
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
                Assert.That(oneMinTwo.x,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(oneMinTwo.y,    Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(oneMinTwo.z,    Is.EqualTo(1f).Within(1e-5f));
                Assert.That(twoMinThree.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(twoMinThree.y,  Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(twoMinThree.z,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeMinFour.x, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeMinFour.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(threeMinFour.z, Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(fourMinFive.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(fourMinFive.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourMinFive.z,  Is.EqualTo(-10f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.Equals))]
        public void Vector3_Equals()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one.Equals(one),          Is.True);
                Assert.That(one.Equals(two),          Is.False);
                Assert.That(one.Equals(new object()), Is.False);

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z, Is.EqualTo(4f).Within(1e-5f));
            });
        }

        /// <summary> Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3.GetHashCode))]
        public void Vector3_GetHashCode()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one.GetHashCode(), Is.EqualTo(796917760));
                Assert.That(two.GetHashCode(), Is.EqualTo(-736100352));

                Assert.That(one.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y, Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z, Is.EqualTo(4f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator ==. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_Eq()
        {
            // Arrange
            Vector3 one   = new Vector3(1f, 1f,  1f);
            Vector3 two   = new Vector3(2f, -5f, 4f);
            Vector3 three = new Vector3(1f, 1f,  1f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one == three, Is.True);
                Assert.That(one == two,   Is.False);

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(1f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator !=. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_Neq()
        {
            // Arrange
            Vector3 one   = new Vector3(1f, 1f,  1f);
            Vector3 two   = new Vector3(2f, -5f, 4f);
            Vector3 three = new Vector3(1f, 1f,  1f);

            // Act

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one != three, Is.False);
                Assert.That(one != two,   Is.True);

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(1f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator single -. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_SingleMinus()
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
                Assert.That(oneNegated.x,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.y,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneNegated.z,   Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(twoNegated.x,   Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(twoNegated.y,   Is.EqualTo(5f).Within(1e-5f));
                Assert.That(twoNegated.z,   Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeNegated.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(threeNegated.y, Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(threeNegated.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fourNegated.x,  Is.EqualTo(1f).Within(1e-5f));
                Assert.That(fourNegated.y,  Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(fourNegated.z,  Is.EqualTo(10f).Within(1e-5f));
                Assert.That(fiveNegated.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNegated.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(fiveNegated.z,  Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
                Assert.That(five.x,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.y,  Is.EqualTo(0f).Within(1e-5f));
                Assert.That(five.z,  Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator +. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_Plus()
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
                Assert.That(oneTwoSum.x,    Is.EqualTo(3f).Within(1e-5f));
                Assert.That(oneTwoSum.y,    Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(oneTwoSum.z,    Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeFourSum.x, Is.EqualTo(3f).Within(1e-5f));
                Assert.That(threeFourSum.y, Is.EqualTo(1.5f).Within(1e-5f));
                Assert.That(threeFourSum.z, Is.EqualTo(-10f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator -. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_Minus()
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
                Assert.That(oneTwoSub.x,    Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(oneTwoSub.y,    Is.EqualTo(6f).Within(1e-5f));
                Assert.That(oneTwoSub.z,    Is.EqualTo(-3f).Within(1e-5f));
                Assert.That(threeFourSub.x, Is.EqualTo(5f).Within(1e-5f));
                Assert.That(threeFourSub.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(threeFourSub.z, Is.EqualTo(10f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator *. Multiplying two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_Star()
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(2f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(oneTwoMul.z,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(threeFourMul.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator *. Multiplying vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_StarScalar()
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
                Assert.That(oneTwoMul.x,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(oneTwoMul.y,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(oneTwoMul.z,    Is.EqualTo(4f).Within(1e-5f));
                Assert.That(threeFourMul.x, Is.EqualTo(-12f).Within(1e-5f));
                Assert.That(threeFourMul.y, Is.EqualTo(-3f).Within(1e-5f));
                Assert.That(threeFourMul.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator /. Dividing two vectors. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_Slash()
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(-0.2f).Within(1e-5f));
                Assert.That(oneTwoDiv.z,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-4f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(2f).Within(1e-5f));
                Assert.That(threeFourDiv.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(two.x,   Is.EqualTo(2f).Within(1e-5f));
                Assert.That(two.y,   Is.EqualTo(-5f).Within(1e-5f));
                Assert.That(two.z,   Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(four.x,  Is.EqualTo(-1f).Within(1e-5f));
                Assert.That(four.y,  Is.EqualTo(0.5f).Within(1e-5f));
                Assert.That(four.z,  Is.EqualTo(-10f).Within(1e-5f));
            });
        }

        /// <summary> Testing operator /. Dividing vector and scalar. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Operator_SlashScalar()
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
                Assert.That(oneTwoDiv.x,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(oneTwoDiv.y,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(oneTwoDiv.z,    Is.EqualTo(0.25f).Within(1e-5f));
                Assert.That(threeFourDiv.x, Is.EqualTo(-2f).Within(1e-5f));
                Assert.That(threeFourDiv.y, Is.EqualTo(-0.5f).Within(1e-5f));
                Assert.That(threeFourDiv.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one.x,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.y,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(one.z,   Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.x, Is.EqualTo(4f).Within(1e-5f));
                Assert.That(three.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(three.z, Is.EqualTo(0f).Within(1e-5f));
            });
        }

        /// <summary> Testing implicit operator from Vector3 to Vector2. Providing data and expecting mathematically correct results. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Vector2()
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
                Assert.That(newOne.x, Is.EqualTo(1).Within(1e-5f));
                Assert.That(newOne.y, Is.EqualTo(1f).Within(1e-5f));
                Assert.That(newOne.z, Is.EqualTo(0f).Within(1e-5f));
                Assert.That(newTwo.x, Is.EqualTo(7f).Within(1e-5f));
                Assert.That(newTwo.y, Is.EqualTo(8f).Within(1e-5f));
                Assert.That(newTwo.z, Is.EqualTo(0f).Within(1e-5f));

                Assert.That(one, Is.EqualTo(new Vector2(1f, 1f)));
                Assert.That(two, Is.EqualTo(new Vector2(7f, 8f)));
            });
        }

        /// <summary> Testing bracket access operator. </summary>
        [Test]
        [TestOf(nameof(Vector3))]
        public void Vector3_Bracket()
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
                Assert.That(one[0], Is.EqualTo(1f));
                Assert.That(one[1], Is.EqualTo(1f));
                Assert.That(one[2], Is.EqualTo(1f));
                Assert.That(two[0], Is.EqualTo(10f));
                Assert.That(two[1], Is.EqualTo(11f));
                Assert.That(two[2], Is.EqualTo(12f));
            });
        }
    }
}