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
    public class TVector3Tests
    {
        /// <summary> Testing enumerator. </summary>
        [Test]
        [TestOf(nameof(TVector3<int>))]
        public void TVector3_int_Enumerator()
        {
            // Arrange
            TVector3<int> one = new TVector3<int> {x = 10, y = 15, z = 20};

            // Act
            using IEnumerator<int> enumerator = one.GetEnumerator();
            
            // Assert
            Assert.Multiple(() =>
            {
                enumerator.MoveNext();
                Assert.That(enumerator.Current, Is.EqualTo(10));
                enumerator.MoveNext();
                Assert.That(enumerator.Current, Is.EqualTo(15));
                enumerator.MoveNext();
                Assert.That(enumerator.Current, Is.EqualTo(20));
            });
        }
        
        /// <summary> Testing bracket access operator. </summary>
        [Test]
        [TestOf(nameof(Vector2))]
        public void Vector2_Bracket()
        {
            // Arrange
            TVector3<int> two = new TVector3<int> {x = 7, y = 8, z = 4};
            TVector3<int> one = new TVector3<int> {x = 1, y = 1, z = 1};

            // Act
            two[0] = 10;
            two[1] = 11;
            two[2] = 12;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(one[0], Is.EqualTo(1));
                Assert.That(one[1], Is.EqualTo(1));
                Assert.That(one[2], Is.EqualTo(1));
                Assert.That(two[0], Is.EqualTo(10));
                Assert.That(two[1], Is.EqualTo(11));
                Assert.That(two[2], Is.EqualTo(12));
            });
        }
    }
}