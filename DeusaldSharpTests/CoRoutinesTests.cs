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
    public class CoRoutinesTests
    {
        [SetUp]
        public void Setup()
        {
            CoRoCtrl.Reset();
        }
        
        /// <summary> Testing standard coRoutine flow and waitForOneTick state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void XWTMN()
        {
            // Arrange
            int value = 0;
            
            IEnumerator<ICoData> TestMethod()
            {
                value = 1;
                yield return CoRoCtrl.WaitForOneTick();
                value = 2;
                yield return CoRoCtrl.WaitForOneTick();
                value = 3;
            }

            // Act & Assert
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            Assert.AreEqual(0, value);
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.AreEqual(1, value);
            CoRoCtrl.Update();
            Assert.AreEqual(1, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(2, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(3, value);
        }
        
        /// <summary> Testing coRoutine in coRoutine and WaitUntilDone state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void GWTZL()
        {
            // Arrange
            int value = 0;
            
            IEnumerator<ICoData> TestMethod()
            {
                value = 1;
                yield return CoRoCtrl.WaitUntilDone(CoRoCtrl.RunCoRoutine(TestMethodTwo()));
                value = 4;
            }
            
            IEnumerator<ICoData> TestMethodTwo()
            {
                value = 2;
                yield return CoRoCtrl.WaitForOneTick();
                value = 3;
            }

            // Act & Assert
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            Assert.AreEqual(0, value);
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.AreEqual(2, value);
            CoRoCtrl.Update();
            Assert.AreEqual(2, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(3, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(4, value);
        }
        
        /// <summary> Testing standard coRoutine flow and waitForSeconds state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void BELZH()
        {
            // Arrange
            int value = 0;
            
            IEnumerator<ICoData> TestMethod()
            {
                value = 1;
                yield return CoRoCtrl.WaitForSeconds(0.5f);
                value = 2;
            }

            // Act & Assert
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            Assert.AreEqual(0, value);
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.AreEqual(1, value);
            CoRoCtrl.Update();
            Assert.AreEqual(1, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(1, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(2, value);
        }
        
        /// <summary> Testing standard coRoutine flow and waitForCondition state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void SMKEE()
        {
            // Arrange
            int  value = 0;
            bool pass  = false;
            
            IEnumerator<ICoData> TestMethod()
            {
                value = 1;
                // ReSharper disable once AccessToModifiedClosure
                yield return CoRoCtrl.WaitUntilTrue(() => pass);
                value = 2;
            }

            // Act & Assert
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            Assert.AreEqual(0, value);
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.AreEqual(1, value);
            CoRoCtrl.Update();
            Assert.AreEqual(1, value);
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(1, value);
            pass = true;
            CoRoCtrl.SetNextCoState(CoSegment.Normal, 0.33f);
            CoRoCtrl.Update();
            Assert.AreEqual(2, value);
        }
    }
}