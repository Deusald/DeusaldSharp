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

using System;
using System.Diagnostics;
using System.Threading;

namespace DeusaldSharp
{
    public class PrecisionClock
    {
        public delegate void TickDelegate(ulong frameNumber);

        public bool IsAlive { get; private set; }

        public event TickDelegate? Tick;
        public event Action?       TooLongFrame;

        public PrecisionClock(ushort ticksPerSecond)
        {
            IsAlive = true;

            if (MathUtils.SEC_TO_MILLISECONDS % ticksPerSecond != 0)
                throw new Exception("Number of ticks per second must be factor of 1000.");

            double msPerFrame = MathUtils.SEC_TO_MILLISECONDS / (double)ticksPerSecond;
            var    interval   = TimeSpan.FromMilliseconds(msPerFrame);

            Thread clockThread = new Thread(() => ClockStep(interval))
            {
                IsBackground = true,
                Priority     = ThreadPriority.AboveNormal
            };

            clockThread.Start();
        }

        public void Kill() => IsAlive = false;

        private void ClockStep(TimeSpan interval)
        {
            // Ticks in Stopwatch are based on Stopwatch.Frequency
            long intervalTicks = (long)(interval.TotalSeconds * Stopwatch.Frequency);

            // Stage thresholds (tune if you want)
            const int stage1SleepMs = 2; // coarse sleep when far away
            long      stage1Ticks   = (long)(stage1SleepMs / 1000.0 * Stopwatch.Frequency);

            // warmup: align first tick to next interval boundary
            Stopwatch sw    = Stopwatch.StartNew();
            ulong     frame = 0;

            long nextTick = ((sw.ElapsedTicks / intervalTicks) + 1) * intervalTicks;

            while (IsAlive)
            {
                long now       = sw.ElapsedTicks;
                long remaining = nextTick - now;

                // Coarse sleep if we have time (avoid oversleep by leaving a little margin)
                if (remaining > stage1Ticks)
                {
                    // Sleep most of the remaining time, keep a margin
                    double remainingMs = (remaining - stage1Ticks) * 1000.0 / Stopwatch.Frequency;
                    if (remainingMs > 1)
                        Thread.Sleep((int)remainingMs);
                }

                // Spin until the target
                while (sw.ElapsedTicks < nextTick)
                    Thread.SpinWait(64);

                Tick?.Invoke(frame++);
                nextTick += intervalTicks;

                // If we’re already behind nextTick, we missed the budget
                if (sw.ElapsedTicks >= nextTick)
                    TooLongFrame?.Invoke();
            }
        }
    }
}