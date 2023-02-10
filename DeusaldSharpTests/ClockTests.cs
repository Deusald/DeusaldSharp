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
using System.Threading;
using System.Threading.Tasks;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class ClockTests
    {
        /// <summary> Testing standard coRoutine flow and waitForOneTick state. </summary>
        [Test]
        [TestOf(nameof(PrecisionClock))]
        public async Task XWTMN()
        {
            // Arrange
            ulong  lastFrameNumber = 0;

            PrecisionClock serverClock = new PrecisionClock(50);
            serverClock.Tick += (frameNumber) =>
            {
                lastFrameNumber = frameNumber;
                Thread.Sleep(5);
            };
            serverClock.TooLongFrame += () => Console.WriteLine("Too long frame");

            // Act
            await Task.Delay(5 * MathUtils.SEC_TO_MILLISECONDS);

            // Assert (50 frames per second * 5 seconds - 2 frames for warmup start)
            Assert.AreEqual(50*5-2, lastFrameNumber);
        }
    }
}