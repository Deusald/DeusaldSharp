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

using System;
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
            Spline2D one = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) });
            Spline2D two = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2) });

            // Act
            float lengthOne = one.Length;
            float lengthTwo = two.Length;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(lengthOne, Is.EqualTo(2));
                Assert.That(lengthTwo, Is.EqualTo(2.82842731f));
            });
        }

        /// <summary> Testing interpolate position on spline. Providing points and expecting correct position approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.Interpolate))]
        public void Spline2D_Interpolate()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) });
            Spline2D two = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2) });

            // Act
            Vector2 positionOne = one.Interpolate(0.75f);
            Vector2 positionTwo = two.Interpolate(0.75f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(positionOne.x, Is.EqualTo(0).Within(1e-5f));
                Assert.That(positionOne.y, Is.EqualTo(1.5625f).Within(1e-5f));
                Assert.That(positionTwo.x, Is.EqualTo(1.5625f).Within(1e-5f));
                Assert.That(positionTwo.y, Is.EqualTo(1.5625f).Within(1e-5f));
            });
        }

        /// <summary> Testing interpolate position on spline. Providing points and expecting correct position approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.InterpolateDistance))]
        public void Spline2D_InterpolateDistance()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) });
            Spline2D two = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2) });

            // Act
            Vector2 positionOne = one.InterpolateDistance(1.5f);
            Vector2 positionTwo = two.InterpolateDistance(1.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(positionOne.x, Is.EqualTo(0).Within(1e-5f));
                Assert.That(positionOne.y, Is.EqualTo(1.5015914f).Within(1e-5f));
                Assert.That(positionTwo.x, Is.EqualTo(1.0576555f).Within(1e-5f));
                Assert.That(positionTwo.y, Is.EqualTo(1.0576555f).Within(1e-5f));
            });
        }

        /// <summary> Testing multiply request of interpolate position on spline. Providing points and expecting correct position approximation. </summary>
        [Test]
        [TestOf(nameof(Spline2D.InterpolateDistance))]
        public void Spline2D_InterpolateDistance_Multiply()
        {
            // Arrange
            Spline2D one = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) });
            Spline2D two = new Spline2D(new List<Vector2> { new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 2) });

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
                Assert.That(positionOne.x,   Is.EqualTo(0).Within(1e-5f));
                Assert.That(positionOne.y,   Is.EqualTo(1.5015914f).Within(1e-5f));
                Assert.That(positionTwo.x,   Is.EqualTo(1.0576555f).Within(1e-5f));
                Assert.That(positionTwo.y,   Is.EqualTo(1.0576555f).Within(1e-5f));
                Assert.That(positionThree.x, Is.EqualTo(0).Within(1e-5f));
                Assert.That(positionThree.y, Is.EqualTo(0.49840856f).Within(1e-5f));
                Assert.That(positionFour.x,  Is.EqualTo(0.35238677f).Within(1e-5f));
                Assert.That(positionFour.y,  Is.EqualTo(0.35238677f).Within(1e-5f));
                Assert.That(positionFive.x,  Is.EqualTo(0).Within(1e-5f));
                Assert.That(positionFive.y,  Is.EqualTo(1.5015914f).Within(1e-5f));
                Assert.That(positionSix.x,   Is.EqualTo(1.0576555f).Within(1e-5f));
                Assert.That(positionSix.y,   Is.EqualTo(1.0576555f).Within(1e-5f));
            });
        }

        private static readonly Comparison<Vector2> _ApproxComparer = (a, b) => a == b ? 0 : 1;

        /// <summary> Distance search cache must be reset when samples are recalculated (e.g. after ReplacePoints). </summary>
        [Test]
        [TestOf(nameof(Spline2D.DistanceToLinearT))]
        public void Spline2D_DistanceToLinearT_AfterReplacePoints()
        {
            // Arrange
            List<Vector2> longLine = new List<Vector2>();
            for (int x = 0; x < 20; ++x) longLine.Add(new Vector2(0, x));
            Spline2D spline = new Spline2D(longLine);
            spline.DistanceToLinearT(18.5f);
            spline.DistanceToLinearT(18.5f);

            // Act
            spline.ReplacePoints(new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) });
            float   t        = spline.DistanceToLinearT(1.5f);
            Vector2 position = spline.InterpolateDistance(1.5f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(t,          Is.InRange(0f, 1f));
                Assert.That(position.y, Is.EqualTo(1.5015914f).Within(1e-5f));
            });
        }

        /// <summary> Forward and backward sequential distance queries give the same results as fresh splines. </summary>
        [Test]
        [TestOf(nameof(Spline2D.DistanceToLinearT))]
        public void Spline2D_DistanceToLinearT_SequentialQueries_MatchFresh()
        {
            // Arrange
            List<Vector2> points = new List<Vector2> { new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0), new Vector2(3, 1) };
            Spline2D      reused = new Spline2D(points);
            float[]       inputs = { 0.1f, 0.5f, 1.2f, 2.9f, 0.3f, 2.9f, 2.9f, 1f };

            // Assert
            Assert.Multiple(() =>
            {
                foreach (float distance in inputs)
                    Assert.That(reused.DistanceToLinearT(distance), Is.EqualTo(new Spline2D(points).DistanceToLinearT(distance)));
            });
        }

        /// <summary> Dropping below two points must not keep stale tangents/distances. </summary>
        [Test]
        [TestOf(nameof(Spline2D.Length))]
        public void Spline2D_LessThanTwoPoints_ClearsStaleData()
        {
            // Arrange
            List<Vector2> longLine = new List<Vector2>();
            for (int x = 0; x < 21; ++x) longLine.Add(new Vector2(0, x));
            Spline2D spline = new Spline2D(longLine);
            float    before = spline.Length;

            // Act
            spline.Clear();
            spline.AddPoint(new Vector2(5, 5));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(before,                         Is.EqualTo(20f).Within(1e-3f));
                Assert.That(spline.Length,                  Is.EqualTo(0f));
                Assert.That(spline.DistanceToLinearT(3f),   Is.EqualTo(0f));
                Assert.That(spline.Interpolate(0.5f),       Is.EqualTo(new Vector2(5, 5)));
                Assert.That(spline.InterpolateDistance(3f), Is.EqualTo(new Vector2(5, 5)));
                Assert.That(spline.DistanceAtPoint(1),      Is.EqualTo(0f));
            });
        }

        /// <summary> Edge case inputs must not crash. </summary>
        [Test]
        [TestOf(nameof(Spline2D.Interpolate))]
        public void Spline2D_Interpolate_EdgeCases()
        {
            // Arrange
            List<Vector2> points      = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2) };
            Spline2D      open        = new Spline2D(points);
            Spline2D      closed      = new Spline2D(points, true);
            Spline2D      empty       = new Spline2D();
            Spline2D      emptyClosed = new Spline2D { IsClosed = true };
            Spline2D      single      = new Spline2D(new List<Vector2> { new Vector2(3, 4) }, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(open.Interpolate(-1000f),                    Is.EqualTo(new Vector2(0, 0)));
                Assert.That(open.Interpolate(-0.5f),                     Is.EqualTo(new Vector2(0, 0)));
                Assert.That(open.Interpolate(1.5f),                      Is.EqualTo(new Vector2(0, 2)));
                Assert.That(open.InterpolateDistance(-1f),               Is.EqualTo(new Vector2(0, 0)));
                Assert.That(closed.Interpolate(-0.25f),                  Is.EqualTo(closed.Interpolate(0.75f)).Using(_ApproxComparer));
                Assert.That(closed.Interpolate(1.25f),                   Is.EqualTo(closed.Interpolate(0.25f)).Using(_ApproxComparer));
                Assert.That(closed.Interpolate(-1000f),                  Is.EqualTo(new Vector2(0, 0)));
                Assert.DoesNotThrow(() => closed.InterpolateDistance(-1f));
                Assert.DoesNotThrow(() => closed.Derivative(-0.3f));
                Assert.That(empty.Interpolate(0.5f),                     Is.EqualTo(Vector2.Zero));
                Assert.That(empty.Derivative(0.5f),                      Is.EqualTo(Vector2.Zero));
                Assert.That(emptyClosed.Interpolate(0.5f),               Is.EqualTo(Vector2.Zero));
                Assert.That(emptyClosed.Length,                          Is.EqualTo(0f));
                Assert.That(single.Interpolate(0.5f),                    Is.EqualTo(new Vector2(3, 4)));
                Assert.That(single.Length,                               Is.EqualTo(0f));
                Assert.Throws<ArgumentOutOfRangeException>(() => open.LengthSamplesPerSegment = -1);
                Assert.Throws<ArgumentOutOfRangeException>(() => open.LengthSamplesPerSegment = 0);
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Spline2D(points, false, 0.5f, 0));
            });
        }
    }
}