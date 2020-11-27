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
        [Test]
        [Description("Testing if all standard Vector3 static values are correct.")]
        public void SCMWP()
        {
            // Arrange

            // Act

            // Assert
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
        }

        [Test]
        [TestOf(nameof(Vector3.Magnitude))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(1.7321f,  oneMagnitude);
            Assert.AreEqual(6.7082f,  twoMagnitude);
            Assert.AreEqual(1f,       threeMagnitude);
            Assert.AreEqual(10.0623f, fourMagnitude);
            Assert.AreEqual(0f,       fiveMagnitude);
        }

        [Test]
        [TestOf(nameof(Vector3.SqrMagnitude))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(3f,      oneSqrMagnitude);
            Assert.AreEqual(45f,     twoSqrMagnitude);
            Assert.AreEqual(1f,      threeSqrMagnitude);
            Assert.AreEqual(101.25f, fourSqrMagnitude);
            Assert.AreEqual(0f,      fiveSqrMagnitude);
        }

        [Test]
        [TestOf(nameof(Vector3.Normalized))]
        [Description("Providing data and expecting correct results. After getting Normalized vector the original should remain the same.")]
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
        }

        [Test]
        [TestOf(nameof(Vector3.Negated))]
        [Description("Providing data and expecting correct results. After getting Negated vector the original should remain the same.")]
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
        }

        [Test]
        [TestOf(nameof(Vector3.IsValid))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.IsTrue(oneIsValid);
            Assert.IsTrue(twoIsValid);
            Assert.IsFalse(threeIsValid);
            Assert.IsFalse(fourIsValid);
            Assert.IsTrue(fiveIsValid);
        }

        [Test]
        [TestOf(nameof(Vector3))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(1f, one.X);
            Assert.AreEqual(1f, one.Y);
            Assert.AreEqual(1f, one.Z);

            Assert.AreEqual(5f, two.X);
            Assert.AreEqual(5f, two.Y);
            Assert.AreEqual(5f, two.Z);

            Assert.AreEqual(1f, three.X);
            Assert.AreEqual(2f, three.Y);
            Assert.AreEqual(3f, three.Z);

            Assert.AreEqual(6f, four.X);
            Assert.AreEqual(7f, four.Y);
            Assert.AreEqual(0f, four.Z);

            Assert.AreEqual(4f, five.X);
            Assert.AreEqual(2f, five.Y);
            Assert.AreEqual(0f, five.Z);

            Assert.AreEqual(8f, six.X);
            Assert.AreEqual(2f, six.Y);
            Assert.AreEqual(1f, six.Z);
        }

        [Test]
        [TestOf(nameof(Vector3.Set))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(2f,  3f, 4f),  one);
            Assert.AreEqual(new Vector3(-5f, 4f, 88f), two);
            Assert.AreEqual(new Vector3(1f,  0f, -1f), three);
            Assert.AreEqual(new Vector3(3f,  6f, 3f),  four);
            Assert.AreEqual(new Vector3(1f,  1f, 1f),  five);
        }

        [Test]
        [TestOf(nameof(Vector3.SetZero))]
        [Description("Providing data and expecting correct results.")]
        public void WLNHS()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f, 1f);

            // Act
            one.SetZero();

            // Assert
            Assert.AreEqual(new Vector3(0f, 0f, 0f), one);
        }

        [Test]
        [TestOf(nameof(Vector3.Add))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(3f, -4f,  5f),   oneTwoSum);
            Assert.AreEqual(new Vector3(3f, 1.5f, -10f), threeFourSum);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
            Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
            Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
        }

        [Test]
        [TestOf(nameof(Vector3.Subtract))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(-1f, 6f,   -3f), oneTwoSub);
            Assert.AreEqual(new Vector3(5f,  0.5f, 10f), threeFourSub);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
            Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
            Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
        }

        [Test]
        [TestOf(nameof(Vector3.Multiply))]
        [Description("Multiplying two vectors. Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(2f,  -5f,  4f), oneTwoMul);
            Assert.AreEqual(new Vector3(-4f, 0.5f, 0f), threeFourMul);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
            Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
            Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
        }
        
        [Test]
        [TestOf(nameof(Vector3.Multiply))]
        [Description("Multiplying vector and scalar. Providing data and expecting correct results.")]
        public void XRCXS()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            float two = 4f;

            Vector3 three = new Vector3(4f,  1f,   0f);
            float four  = -3f;

            // Act
            Vector3 oneTwoMul    = Vector3.Multiply(one,   two);
            Vector3 threeFourMul = Vector3.Multiply(three, four);

            // Assert
            Assert.AreEqual(new Vector3(4f,  4f,  4f), oneTwoMul);
            Assert.AreEqual(new Vector3(-12f, -3f, 0f), threeFourMul);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
        }
        
        [Test]
        [TestOf(nameof(Vector3.Divide))]
        [Description("Dividing two vectors. Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(0.5f,  -0.2f,  0.25f), oneTwoDiv);
            Assert.AreEqual(new Vector3(-4f, 2f, 0f), threeFourDiv);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
            Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
            Assert.AreEqual(new Vector3(-1f, 0.5f, -10f), four);
        }
        
        [Test]
        [TestOf(nameof(Vector3.Divide))]
        [Description("Dividing vector and scalar. Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(0.25f,   0.25f,  0.25f), oneTwoDiv);
            Assert.AreEqual(new Vector3(-2f, -0.5f, 0f), threeFourDiv);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(4f,  1f,   0f),   three);
        }
        
        [Test]
        [TestOf(nameof(Vector3.GetNegated))]
        [Description("Providing data and expecting correct results.")]
        public void SZLKG()
        {
            // Arrange
            Vector3 one = new Vector3(1f, 1f,  1f);
            Vector3 two = new Vector3(2f, -5f, 4f);

            // Act
            Vector3 oneNegated    = Vector3.GetNegated(one);
            Vector3 threeNegated = Vector3.GetNegated(two);

            // Assert
            Assert.AreEqual(new Vector3(-1f, -1f, -1f), oneNegated);
            Assert.AreEqual(new Vector3(-2f,  5f,    -4f),    threeNegated);
            
            Assert.AreEqual(new Vector3(1f,  1f,   1f),   one);
            Assert.AreEqual(new Vector3(2f,  -5f,  4f),   two);
        }
        
        [Test]
        [TestOf(nameof(Vector3.Negate))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(new Vector3(-1f, -1f,   -1f), one);
            Assert.AreEqual(new Vector3(-2f, 5f,    -4f), two);
            Assert.AreEqual(new Vector3(0f,  -1f,   0f),  three);
            Assert.AreEqual(new Vector3(1f,  -0.5f, 10f), four);
            Assert.AreEqual(new Vector3(0f,  0f,    0f),  five);
        }
        
        [Test]
        [TestOf(nameof(Vector3.Length))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(1.7321f,  oneLength);
            Assert.AreEqual(6.7082f,  twoLength);
            Assert.AreEqual(1f,       threeLength);
            Assert.AreEqual(10.0623f, fourLength);
            Assert.AreEqual(0f,       fiveLength);
        }

        [Test]
        [TestOf(nameof(Vector3.LengthSquared))]
        [Description("Providing data and expecting correct results.")]
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
            Assert.AreEqual(3f,      oneLengthSquared);
            Assert.AreEqual(45f,     twoLengthSquared);
            Assert.AreEqual(1f,      threeLengthSquared);
            Assert.AreEqual(101.25f, fourLengthSquared);
            Assert.AreEqual(0f,      fiveLengthSquared);
        }
        
        [Test]
        [TestOf(nameof(Vector3.Normalize))]
        [Description("Providing data and expecting correct results. After getting Normalized vector the original should remain the same.")]
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
            Assert.AreEqual(new Vector3(0.57735026f,   0.57735026f,  0.57735026f),  one);
            Assert.AreEqual(new Vector3(0.2981424f,    -0.745356f,   0.5962848f),   two);
            Assert.AreEqual(new Vector3(0f,            1f,           0f),           three);
            Assert.AreEqual(new Vector3(-0.099380806f, 0.049690403f, -0.99380803f), four);
            Assert.AreEqual(new Vector3(0f,            0f,           0f),           five);
        }
    }
}