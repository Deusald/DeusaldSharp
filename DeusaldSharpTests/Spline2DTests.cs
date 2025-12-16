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

using System.Collections.Generic;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class Spline2DTests
    {
        /// <summary> Testing length of spline calculations. Providing points and expecting correct length approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.Length))]
        public void Spline2D_Length()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)});
            Spline2D two = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2)});

            // Act
            float lengthOne = one.Length;
            float lengthTwo = two.Length;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2,           lengthOne);
                Assert.AreEqual(2.82842731f, lengthTwo);
            });
        }

        /// <summary> Testing interpolate position on spline. Providing points and expecting correct position approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.Interpolate))]
        public void Spline2D_Interpolate()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)});
            Spline2D two = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2)});

            // Act
            Vector2 positionOne = one.Interpolate(0.75f);
            Vector2 positionTwo = two.Interpolate(0.75f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0,       1.5625f), positionOne);
                Assert.AreEqual(new Vector2(1.5625f, 1.5625f), positionTwo);
            });
        }
        
        /// <summary> Testing interpolate position on spline. Providing points and expecting correct position approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.InterpolateDistance))]
        public void Spline2D_InterpolateDistance()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)});
            Spline2D two = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2)});

            // Act
            Vector2 positionOne = one.InterpolateDistance(1.5f);
            Vector2 positionTwo = two.InterpolateDistance(1.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0,          1.5015914f), positionOne);
                Assert.AreEqual(new Vector2(1.0576555f, 1.0576555f), positionTwo);
            });
        }
        
        /// <summary> Testing multiply request of interpolate position on spline. Providing points and expecting correct position approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.InterpolateDistance))]
        public void Spline2D_InterpolateDistance_Multiply()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2)});
            Spline2D two = new Spline2D(new List<Vector2> {new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2)});

            // Act
            Vector2 positionOne   = one.InterpolateDistance(1.5f);
            Vector2 positionTwo   = two.InterpolateDistance(1.5f);
            Vector2 positionThree = one.InterpolateDistance(0.5f);
            Vector2 positionFour  = two.InterpolateDistance(0.5f);
            Vector2 positionFive  = one.InterpolateDistance(1.5f);
            Vector2 positionSix   = two.InterpolateDistance(1.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(new Vector2(0,           1.5015914f),  positionOne);
                Assert.AreEqual(new Vector2(1.0576555f,  1.0576555f),  positionTwo);
                Assert.AreEqual(new Vector2(0,           0.49840856f), positionThree);
                Assert.AreEqual(new Vector2(0.35238677f, 0.35238677f), positionFour);
                Assert.AreEqual(new Vector2(0,           1.5015914f),  positionFive);
                Assert.AreEqual(new Vector2(1.0576555f,  1.0576555f),  positionSix);
            });
        }
    }
}