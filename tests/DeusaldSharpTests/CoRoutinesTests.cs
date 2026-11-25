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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class CoRoutinesTests
    {
        public static readonly CoRoCtrl CoRoCtrl = new();
        
        [SetUp]
        public void Setup()
        {
            CoRoCtrl.Reset();
        }
        
        /// <summary> Testing standard coRoutine flow and waitForOneTick state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_Base()
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
            Assert.That(value, Is.EqualTo(0));
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(2));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(3));
        }
        
        /// <summary> Testing coRoutine in coRoutine and WaitUntilDone state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_WaitUntilDone()
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
            Assert.That(value, Is.EqualTo(0));
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.That(value, Is.EqualTo(2));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(2));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(3));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(4));
        }
        
        /// <summary> Testing standard coRoutine flow and waitForSeconds state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_WaitForSeconds()
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
            Assert.That(value, Is.EqualTo(0));
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(2));
        }
        
        /// <summary> Testing standard coRoutine flow and waitForCondition state. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_WaitForCondition()
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
            Assert.That(value, Is.EqualTo(0));
            CoRoCtrl.RunCoRoutine(TestMethod());
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(1));
            pass = true;
            CoRoCtrl.Update(0.33f);
            Assert.That(value, Is.EqualTo(2));
        }
        
        /// <summary> Testing coTags and coHandles. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_CoHandles()
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
            Assert.That(value, Is.EqualTo(0));
            ICoHandle coHandle = CoRoCtrl.RunCoRoutine(TestMethod(), new CoTag(1));
            Assert.That(value, Is.EqualTo(1));
            CoRoCtrl.Update(0.33f);
            Assert.That(coHandle.CoTag, Is.EqualTo(new CoTag(1)));
            coHandle.Kill();
            Assert.That(coHandle.IsAlive, Is.False);
        }

        /// <summary> Finished CoRoutines are unregistered from controller events and removed from the list. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_FinishedCoRoutines_AreUnregistered()
        {
            // Arrange
            CoRoCtrl ctrl = new CoRoCtrl();

            IEnumerator<ICoData> TestMethod()
            {
                yield return ctrl.WaitForOneTick();
            }

            // Act
            for (int x = 0; x < 1000; ++x) ctrl.RunCoRoutine(TestMethod(), new CoTag(1), 1);
            for (int x = 0; x < 3; ++x) ctrl.Update(0.1f);

            // Assert
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            Assert.Multiple(() =>
            {
                Assert.That(typeof(CoRoCtrl).GetField(nameof(CoRoCtrl.OrderToCoRoutinesViaCoTag),  flags)!.GetValue(ctrl), Is.Null);
                Assert.That(typeof(CoRoCtrl).GetField(nameof(CoRoCtrl.OrderToCoRoutinesViaCoMask), flags)!.GetValue(ctrl), Is.Null);
                Assert.That(((ICollection)typeof(CoRoCtrl).GetField("_CoRoutines", flags)!.GetValue(ctrl))!.Count, Is.EqualTo(0));
            });
        }

        /// <summary> CoRoutine killed from inside its own body never continues, its finally block runs once. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_SelfKill_StopsCoRoutine()
        {
            // Arrange
            CoRoCtrl ctrl         = new CoRoCtrl();
            int      value        = 0;
            int      finallyCount = 0;

            IEnumerator<ICoData> TestMethod()
            {
                try
                {
                    value = 1;
                    yield return ctrl.WaitForOneTick();
                    ctrl.GetCurrentCoHandle().Kill();
                    yield return ctrl.WaitForSeconds(0.1f);
                    value = 2;
                }
                finally
                {
                    ++finallyCount;
                }
            }

            // Act
            ICoHandle handle = ctrl.RunCoRoutine(TestMethod());
            for (int x = 0; x < 10; ++x) ctrl.Update(0.1f);
            ctrl.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(value,          Is.EqualTo(1));
                Assert.That(handle.IsAlive, Is.False);
                Assert.That(finallyCount,   Is.EqualTo(1));
            });
        }

        /// <summary> CoRoutine killed via CoTag by a nested child during child's first step never continues. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_TagKillFromNestedChild_StopsParent()
        {
            // Arrange
            CoRoCtrl ctrl       = new CoRoCtrl();
            int      value      = 0;
            int      childValue = 0;

            IEnumerator<ICoData> Parent()
            {
                value = 1;
                yield return ctrl.WaitForOneTick();
                yield return ctrl.WaitUntilDone(Child());
                value = 2;
            }

            IEnumerator<ICoData> Child()
            {
                ctrl.KillCoRoutines(new CoTag(5));
                yield return ctrl.WaitForOneTick();
                childValue = 1;
            }

            // Act
            ICoHandle parent = ctrl.RunCoRoutine(Parent(), new CoTag(5));
            for (int x = 0; x < 10; ++x) ctrl.Update(0.1f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(value,          Is.EqualTo(1));
                Assert.That(childValue,     Is.EqualTo(1));
                Assert.That(parent.IsAlive, Is.False);
            });
        }

        /// <summary> Killing CoRoutine disposes its enumerator, so finally blocks run. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_Kill_RunsFinally()
        {
            // Arrange
            CoRoCtrl ctrl         = new CoRoCtrl();
            int      finallyCount = 0;

            IEnumerator<ICoData> TestMethod()
            {
                try
                {
                    while (true) yield return ctrl.WaitForOneTick();
                }
                finally
                {
                    ++finallyCount;
                }
            }

            // Act & Assert
            ICoHandle handle = ctrl.RunCoRoutine(TestMethod());
            ctrl.Update(0.1f);
            ctrl.Update(0.1f);
            Assert.That(finallyCount, Is.EqualTo(0));

            handle.Kill();
            Assert.That(finallyCount, Is.EqualTo(1));

            handle.Kill();
            ctrl.Update(0.1f);
            ctrl.Reset();
            Assert.That(finallyCount, Is.EqualTo(1));
        }

        /// <summary> Exception in CoRoutine propagates, kills only that CoRoutine and doesn't leave stale current CoRoutine. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl))]
        public void CoRoCtrl_Exception_KillsFaultedCoRoutineOnly()
        {
            // Arrange
            CoRoCtrl ctrl      = new CoRoCtrl();
            int      goodTicks = 0;

            IEnumerator<ICoData> Faulted()
            {
                yield return ctrl.WaitForOneTick();
                throw new ArithmeticException("Test");
            }

            IEnumerator<ICoData> Good()
            {
                while (true)
                {
                    ++goodTicks;
                    yield return ctrl.WaitForOneTick();
                }
            }

            ICoHandle faulted = ctrl.RunCoRoutine(Faulted());
            ICoHandle good    = ctrl.RunCoRoutine(Good());
            ctrl.Update(0.1f);

            // Act & Assert
            Assert.Throws<ArithmeticException>(() => ctrl.Update(0.1f));
            Assert.Throws<InvalidOperationException>(() => ctrl.GetCurrentCoHandle());
            Assert.That(faulted.IsAlive, Is.False);

            int ticksBefore = goodTicks;
            Assert.DoesNotThrow(() => ctrl.Update(0.1f));
            Assert.That(goodTicks,    Is.EqualTo(ticksBefore + 1));
            Assert.That(good.IsAlive, Is.True);
        }

        /// <summary> GetCurrentCoHandle returns own handle during the first step of nested CoRoutine. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl.GetCurrentCoHandle))]
        public void CoRoCtrl_GetCurrentCoHandle_NestedFirstStep()
        {
            // Arrange
            CoRoCtrl  ctrl            = new CoRoCtrl();
            ICoHandle childHandle     = null;
            ICoHandle childSeen       = null;
            ICoHandle parentSeenFirst = null;
            ICoHandle parentSeenAfter = null;

            IEnumerator<ICoData> Parent()
            {
                parentSeenFirst = ctrl.GetCurrentCoHandle();
                yield return ctrl.WaitForOneTick();
                childHandle     = ctrl.RunCoRoutine(Child());
                parentSeenAfter = ctrl.GetCurrentCoHandle();
            }

            IEnumerator<ICoData> Child()
            {
                childSeen = ctrl.GetCurrentCoHandle();
                yield return ctrl.WaitForOneTick();
            }

            // Act
            ICoHandle parentHandle = ctrl.RunCoRoutine(Parent());
            ctrl.Update(0.1f);
            ctrl.Update(0.1f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(parentSeenFirst, Is.SameAs(parentHandle));
                Assert.That(childSeen,       Is.SameAs(childHandle));
                Assert.That(parentSeenAfter, Is.SameAs(parentHandle));
            });
            Assert.Throws<InvalidOperationException>(() => ctrl.GetCurrentCoHandle());
        }

        /// <summary> Repeated WaitForSeconds carries over overshoot, so it doesn't drift. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl.WaitForSeconds))]
        public void CoRoCtrl_WaitForSeconds_Repeated_NoDrift()
        {
            // Arrange
            CoRoCtrl ctrl  = new CoRoCtrl();
            int      count = 0;

            IEnumerator<ICoData> TestMethod()
            {
                while (true)
                {
                    yield return ctrl.WaitForSeconds(0.1f);
                    ++count;
                }
            }

            // Act
            ctrl.RunCoRoutine(TestMethod());
            for (int x = 0; x < 600; ++x) ctrl.Update(1f / 60f);

            // Assert
            Assert.That(count, Is.InRange(98, 102));
        }

        /// <summary> Reset kills all CoRoutines. </summary>
        [Test]
        [TestOf(nameof(CoRoCtrl.Reset))]
        public void CoRoCtrl_Reset_KillsCoRoutines()
        {
            // Arrange
            CoRoCtrl ctrl         = new CoRoCtrl();
            int      finallyCount = 0;

            IEnumerator<ICoData> TestMethod()
            {
                try
                {
                    while (true) yield return ctrl.WaitForOneTick();
                }
                finally
                {
                    ++finallyCount;
                }
            }

            ICoHandle handle = ctrl.RunCoRoutine(TestMethod());
            ctrl.Update(0.1f);

            // Act
            ctrl.Reset();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(handle.IsAlive, Is.False);
                Assert.That(finallyCount,   Is.EqualTo(1));
            });
        }
    }
}