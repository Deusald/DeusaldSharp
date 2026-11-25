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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class ClockTests
    {
        /// <summary> Testing Precision Clock. </summary>
        [Test]
        [TestOf(nameof(PrecisionClock))]
        public async Task PrecisionClock_Base()
        {
            // Arrange
            ulong  lastFrameNumber = 0;

            PrecisionClock serverClock = new PrecisionClock(50);
            serverClock.Tick += frameNumber =>
            {
                lastFrameNumber = frameNumber;
                Thread.Sleep(5);
            };
            serverClock.TooLongFrame += () => Console.WriteLine("Too long frame");

            // Act
            await Task.Delay(5 * MathUtils.SEC_TO_MILLISECONDS);
            serverClock.Kill();
            
            int expected = 50 * 5; // 250
            Assert.That(lastFrameNumber, Is.InRange(expected - 5, expected + 5));

            Assert.That(serverClock.IsAlive, Is.False);
        }

        /// <summary> Testing standard Game Server Clock. </summary>
        [Test]
        [TestOf(nameof(PrecisionClock))]
        public async Task PrecisionClock_Standard()
        {
            // Arrange
            ulong  lastFrameNumber = 0;

            GameServerClock serverClock = new GameServerClock(50, 100);
            serverClock.Tick += (frameNumber, deltaTime) =>
            {
                lastFrameNumber = frameNumber;
                Thread.Sleep(5);
            };
            serverClock.Log += Console.WriteLine;
            
            // Act
            await Task.Delay(5 * MathUtils.SEC_TO_MILLISECONDS);
            serverClock.Kill();
            
            int expected = 50 * 5; // 250
            Assert.That(lastFrameNumber,     Is.InRange(expected - 5, expected + 5));
            Assert.That(serverClock.IsAlive, Is.False);
        }

        /// <summary> 60 Hz clock runs at 60 Hz (not 62.5 Hz because of whole milliseconds interval) with accurate delta time. </summary>
        [Test]
        [TestOf(nameof(GameServerClock))]
        public void GameServerClock_TickRate_IsAccurate()
        {
            // Arrange
            Stopwatch    stopwatch = Stopwatch.StartNew();
            List<double> tickTimes = new List<double>();
            double       sumDeltas = 0;

            using GameServerClock serverClock = new GameServerClock(60, 1000);
            serverClock.Tick += (frameNumber, deltaTime) =>
            {
                lock (tickTimes)
                {
                    tickTimes.Add(stopwatch.Elapsed.TotalSeconds);
                    if (frameNumber > 0) sumDeltas += deltaTime;
                }
            };

            // Act
            Thread.Sleep(1500);
            serverClock.Kill();

            // Assert
            lock (tickTimes)
            {
                double rate         = (tickTimes.Count - 1) / (tickTimes[^1] - tickTimes[0]);
                double averageDelta = sumDeltas / (tickTimes.Count - 1);

                Assert.That(rate,         Is.InRange(58.5, 61.5));
                Assert.That(averageDelta, Is.EqualTo(1.0 / 60.0).Within(0.0005));
            }
        }

        /// <summary> After a stall the clock doesn't fire a burst of catch-up ticks. </summary>
        [Test]
        [TestOf(nameof(GameServerClock))]
        public void GameServerClock_Stall_NoCatchUpBurst()
        {
            // Arrange
            Stopwatch    stopwatch  = Stopwatch.StartNew();
            List<double> tickStarts = new List<double>();
            double       stallEnd   = 0;

            using GameServerClock serverClock = new GameServerClock(60, 1000);
            serverClock.Tick += (frameNumber, deltaTime) =>
            {
                lock (tickStarts) tickStarts.Add(stopwatch.Elapsed.TotalMilliseconds);
                if (frameNumber != 5) return;
                Thread.Sleep(300);
                lock (tickStarts) stallEnd = stopwatch.Elapsed.TotalMilliseconds;
            };

            // Act
            Thread.Sleep(800);
            serverClock.Kill();

            // Assert
            AssertNoBurst(tickStarts, stallEnd, 1000.0 / 60.0);
        }

        /// <summary> After a stall the precision clock doesn't fire a burst of catch-up ticks. </summary>
        [Test]
        [TestOf(nameof(PrecisionClock))]
        public void PrecisionClock_Stall_NoCatchUpBurst()
        {
            // Arrange
            Stopwatch    stopwatch  = Stopwatch.StartNew();
            List<double> tickStarts = new List<double>();
            double       stallEnd   = 0;
            int          tooLong    = 0;

            using PrecisionClock serverClock = new PrecisionClock(50);
            serverClock.TooLongFrame += () => Interlocked.Increment(ref tooLong);
            serverClock.Tick += frameNumber =>
            {
                lock (tickStarts) tickStarts.Add(stopwatch.Elapsed.TotalMilliseconds);
                if (frameNumber != 5) return;
                Thread.Sleep(300);
                lock (tickStarts) stallEnd = stopwatch.Elapsed.TotalMilliseconds;
            };

            // Act
            Thread.Sleep(800);
            serverClock.Kill();

            // Assert
            AssertNoBurst(tickStarts, stallEnd, 1000.0 / 50.0);
            Assert.That(tooLong, Is.GreaterThanOrEqualTo(1));
        }

        /// <summary> Exception in tick handler is reported through TickException and clock keeps ticking. </summary>
        [Test]
        [TestOf(nameof(GameServerClock))]
        public void GameServerClock_TickException_KeepsTicking()
        {
            // Arrange
            int       ticks    = 0;
            Exception reported = null;

            using GameServerClock serverClock = new GameServerClock(100, 1000);
            serverClock.TickException += exception => reported = exception;
            serverClock.Tick += (frameNumber, deltaTime) =>
            {
                Interlocked.Increment(ref ticks);
                if (frameNumber == 2) throw new ArithmeticException("Test");
            };

            // Act
            Thread.Sleep(300);
            serverClock.Kill();

            // Assert
            Assert.That(reported, Is.InstanceOf<ArithmeticException>());
            Assert.That(ticks,    Is.GreaterThan(10));
        }

        /// <summary> Exception in tick handler is reported through TickException and precision clock keeps ticking. </summary>
        [Test]
        [TestOf(nameof(PrecisionClock))]
        public void PrecisionClock_TickException_KeepsTicking()
        {
            // Arrange
            int       ticks    = 0;
            Exception reported = null;

            using PrecisionClock serverClock = new PrecisionClock(100);
            serverClock.TickException += exception => reported = exception;
            serverClock.Tick += frameNumber =>
            {
                Interlocked.Increment(ref ticks);
                if (frameNumber == 2) throw new ArithmeticException("Test");
            };

            // Act
            Thread.Sleep(300);
            serverClock.Kill();

            // Assert
            Assert.That(reported, Is.InstanceOf<ArithmeticException>());
            Assert.That(ticks,    Is.GreaterThan(10));
        }

        /// <summary> No tick is executed after Kill returns. </summary>
        [Test]
        [TestOf(nameof(GameServerClock))]
        public void Clocks_Kill_NoTicksAfterReturn()
        {
            // Arrange
            int gameServerTicks = 0;
            int precisionTicks  = 0;

            GameServerClock gameServerClock = new GameServerClock(100, 1000);
            PrecisionClock  precisionClock  = new PrecisionClock(100);
            gameServerClock.Tick += (frameNumber, deltaTime) =>
            {
                Interlocked.Increment(ref gameServerTicks);
                Thread.Sleep(3);
            };
            precisionClock.Tick += frameNumber =>
            {
                Interlocked.Increment(ref precisionTicks);
                Thread.Sleep(3);
            };

            Thread.Sleep(200);

            // Act
            gameServerClock.Kill();
            precisionClock.Kill();
            int gameServerTicksAtKill = Volatile.Read(ref gameServerTicks);
            int precisionTicksAtKill  = Volatile.Read(ref precisionTicks);
            Thread.Sleep(100);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(gameServerTicksAtKill,   Is.GreaterThan(0));
                Assert.That(precisionTicksAtKill,    Is.GreaterThan(0));
                Assert.That(gameServerTicks,         Is.EqualTo(gameServerTicksAtKill));
                Assert.That(precisionTicks,          Is.EqualTo(precisionTicksAtKill));
                Assert.That(gameServerClock.IsAlive, Is.False);
                Assert.That(precisionClock.IsAlive,  Is.False);
            });
        }

        /// <summary> Zero ticks per second is rejected. </summary>
        [Test]
        [TestOf(nameof(GameServerClock))]
        public void Clocks_ZeroTicksPerSecond_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GameServerClock(0, 100));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PrecisionClock(0));
        }

        private static void AssertNoBurst(List<double> tickStarts, double stallEnd, double intervalMs)
        {
            lock (tickStarts)
            {
                Assert.That(stallEnd, Is.GreaterThan(0));

                // Ticks started within half of the interval after the stall ended are catch-up ticks
                int burst = 0;
                foreach (double tickStart in tickStarts)
                {
                    if (tickStart >= stallEnd && tickStart < stallEnd + intervalMs / 2) ++burst;
                }

                Assert.That(burst, Is.LessThanOrEqualTo(2));
                Assert.That(tickStarts[^1], Is.GreaterThan(stallEnd + intervalMs));
            }
        }
    }
}