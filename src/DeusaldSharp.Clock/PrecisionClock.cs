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
    public class PrecisionClock : IDisposable
    {
        public delegate void TickDelegate(ulong frameNumber);

        public bool IsAlive => _IsAlive;

        public event TickDelegate? Tick;
        public event Action?       TooLongFrame;

        /// <summary> Raised when a Tick handler throws. When there is a subscriber the clock keeps ticking,
        /// without subscribers the exception is rethrown on the clock thread (which terminates the process). </summary>
        public event Action<Exception>? TickException;

        private const int _KILL_JOIN_TIMEOUT_MS = 2000;
        private const int _STAGE_1_SLEEP_MS     = 2; // coarse sleep margin when far away from the next tick

        private readonly ManualResetEventSlim _KillEvent = new ManualResetEventSlim(false);
        private readonly object               _EventLock = new object();
        private readonly Thread               _ClockThread;

        private volatile bool _IsAlive;
        private          bool _IsEventDisposed;

        public PrecisionClock(ushort ticksPerSecond)
        {
            if (ticksPerSecond == 0) throw new ArgumentOutOfRangeException(nameof(ticksPerSecond), "Number of ticks per second must be greater than 0.");

            if (MathUtils.SEC_TO_MILLISECONDS % ticksPerSecond != 0)
                throw new Exception("Number of ticks per second must be factor of 1000.");

            _IsAlive = true;

            double msPerFrame = MathUtils.SEC_TO_MILLISECONDS / (double)ticksPerSecond;
            var    interval   = TimeSpan.FromMilliseconds(msPerFrame);

            _ClockThread = new Thread(() => ClockStep(interval))
            {
                IsBackground = true,
                Priority     = ThreadPriority.AboveNormal
            };

            _ClockThread.Start();
        }

        /// <summary> Stops the clock. When called from other thread than the clock thread, it waits (with a timeout)
        /// for the currently executed tick to finish, so no tick is executed after this method returns. </summary>
        public void Kill()
        {
            _IsAlive = false;

            lock (_EventLock)
            {
                if (!_IsEventDisposed) _KillEvent.Set();
            }

            if (Thread.CurrentThread != _ClockThread) _ClockThread.Join(_KILL_JOIN_TIMEOUT_MS);
        }

        public void Dispose()
        {
            Kill();
        }

        private void ClockStep(TimeSpan interval)
        {
            // Ticks in Stopwatch are based on Stopwatch.Frequency
            long intervalTicks = (long)(interval.TotalSeconds * Stopwatch.Frequency);
            long stage1Ticks   = (long)(_STAGE_1_SLEEP_MS / 1000.0 * Stopwatch.Frequency);

            // warmup: align first tick to next interval boundary
            Stopwatch sw    = Stopwatch.StartNew();
            ulong     frame = 0;

            long nextTick = ((sw.ElapsedTicks / intervalTicks) + 1) * intervalTicks;

            try
            {
                while (_IsAlive)
                {
                    long now       = sw.ElapsedTicks;
                    long remaining = nextTick - now;

                    // Coarse sleep if we have time (avoid oversleep by leaving a little margin)
                    if (remaining > stage1Ticks)
                    {
                        // Sleep most of the remaining time, keep a margin
                        double remainingMs = (remaining - stage1Ticks) * 1000.0 / Stopwatch.Frequency;
                        if (remainingMs > 1)
                            _KillEvent.Wait((int)remainingMs);
                    }

                    // Spin until the target
                    while (_IsAlive && sw.ElapsedTicks < nextTick)
                        Thread.SpinWait(64);

                    if (!_IsAlive) break;

                    InvokeTick(frame++);
                    nextTick += intervalTicks;

                    // If we’re already behind nextTick, we missed the budget
                    now = sw.ElapsedTicks;
                    if (now < nextTick) continue;

                    TooLongFrame?.Invoke();

                    // After a stall don't fire a burst of catch-up ticks, skip missed frames instead
                    if (now - nextTick > intervalTicks) nextTick = now + intervalTicks;
                }
            }
            finally
            {
                _IsAlive = false;

                lock (_EventLock)
                {
                    _IsEventDisposed = true;
                    _KillEvent.Dispose();
                }
            }
        }

        private void InvokeTick(ulong frame)
        {
            try
            {
                Tick?.Invoke(frame);
            }
            catch (Exception exception)
            {
                Action<Exception>? tickException = TickException;
                if (tickException == null) throw;
                tickException(exception);
            }
        }
    }
}
