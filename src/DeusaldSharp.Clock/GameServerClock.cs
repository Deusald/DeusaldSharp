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
    /// <summary> Clock for multiplayer game servers. </summary>
    public class GameServerClock : IDisposable
    {
        #region Public Types

        public delegate void TickDelegate(ulong frameNumber, double deltaTime);

        #endregion Public Types

        #region Properties

        /// <summary> Is Clock still executing. </summary>
        public bool IsAlive => _IsAlive;

        #endregion Properties

        #region Variables

        /// <summary> This event will be executed with every tick. </summary>
        public event TickDelegate? Tick;

        /// <summary> Used to log information about Clock. </summary>
        public event Action<string>? Log;

        /// <summary> Raised when a Tick handler throws. When there is a subscriber the clock keeps ticking,
        /// without subscribers the exception is rethrown on the clock thread (which terminates the process). </summary>
        public event Action<Exception>? TickException;

        private const int _KILL_JOIN_TIMEOUT_MS = 2000;

        private readonly AutoResetEvent _AutoResetEvent;
        private readonly object         _EventLock = new object();
        private readonly Thread         _ClockThread;
        private readonly double         _StopwatchTicksPerFrame;
        private readonly ushort         _LogEveryXFrame;

        private volatile bool _IsAlive;
        private          bool _IsEventDisposed;

        #endregion Variables

        #region Special Methods

        public GameServerClock(ushort ticksPerSecond, ushort logEveryXFrame)
        {
            if (ticksPerSecond == 0) throw new ArgumentOutOfRangeException(nameof(ticksPerSecond), "Number of ticks per second must be greater than 0.");

            _IsAlive                = true;
            _LogEveryXFrame         = logEveryXFrame;
            _AutoResetEvent         = new AutoResetEvent(false);
            _StopwatchTicksPerFrame = (double)Stopwatch.Frequency / ticksPerSecond;

            _ClockThread = new Thread(ClockLoop)
            {
                IsBackground = true,
                Priority     = ThreadPriority.AboveNormal
            };

            _ClockThread.Start();
        }

        #endregion Special Methods

        #region Public Methods

        /// <summary> Stops the clock. When called from other thread than the clock thread, it waits (with a timeout)
        /// for the currently executed tick to finish, so no tick is executed after this method returns. </summary>
        public void Kill()
        {
            _IsAlive = false;

            lock (_EventLock)
            {
                if (!_IsEventDisposed) _AutoResetEvent.Set();
            }

            if (Thread.CurrentThread != _ClockThread) _ClockThread.Join(_KILL_JOIN_TIMEOUT_MS);
        }

        public void Dispose()
        {
            Kill();
        }

        #endregion Public Methods

        #region Private Methods

        private void ClockLoop()
        {
            ulong  frameNumber                   = 0;
            double nextTick                      = 0;
            long   frameStart                    = 0;
            double sumTickLength                 = 0;
            ulong  framesSinceLastLog            = 0;
            double longestTickLenghtSinceLastLog = 0;
            ulong  longestTickFrameNumber        = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                while (_IsAlive)
                {
                    long previousFrameStart = frameStart;
                    frameStart = stopwatch.ElapsedTicks;
                    InvokeTick(frameNumber, (frameStart - previousFrameStart) / (double)Stopwatch.Frequency);
                    long frameEnd = stopwatch.ElapsedTicks;

                    double frameLenght = (frameEnd - frameStart) * MathUtils.SEC_TO_MILLISECONDS / (double)Stopwatch.Frequency;
                    sumTickLength += frameLenght;

                    if (frameLenght > longestTickLenghtSinceLastLog)
                    {
                        longestTickLenghtSinceLastLog = frameLenght;
                        longestTickFrameNumber        = frameNumber;
                    }

                    ++frameNumber;
                    ++framesSinceLastLog;

                    if (framesSinceLastLog == _LogEveryXFrame)
                    {
                        Log?.Invoke(
                            $"Frame {frameNumber}. Average frame length since last log {(sumTickLength / framesSinceLastLog):0.000} ms. Longest tick length {longestTickLenghtSinceLastLog:0.000} ms in frame {longestTickFrameNumber}.");
                        sumTickLength                 = 0;
                        framesSinceLastLog            = 0;
                        longestTickLenghtSinceLastLog = 0;
                        longestTickFrameNumber        = 0;
                    }

                    nextTick += _StopwatchTicksPerFrame;

                    // After a stall don't fire a burst of catch-up ticks, skip missed frames instead
                    long now = stopwatch.ElapsedTicks;
                    if (now - nextTick > _StopwatchTicksPerFrame) nextTick = now + _StopwatchTicksPerFrame;

                    WaitUntil(stopwatch, nextTick);
                }
            }
            finally
            {
                _IsAlive = false;

                lock (_EventLock)
                {
                    _IsEventDisposed = true;
                    _AutoResetEvent.Dispose();
                }
            }
        }

        private void InvokeTick(ulong frameNumber, double deltaTime)
        {
            try
            {
                Tick?.Invoke(frameNumber, deltaTime);
            }
            catch (Exception exception)
            {
                Action<Exception>? tickException = TickException;
                if (tickException == null) throw;
                tickException(exception);
            }
        }

        private void WaitUntil(Stopwatch stopwatch, double targetTicks)
        {
            while (_IsAlive)
            {
                double remainingMs = (targetTicks - stopwatch.ElapsedTicks) * MathUtils.SEC_TO_MILLISECONDS / Stopwatch.Frequency;
                if (remainingMs <= 0) return;

                if (remainingMs >= 1.0) _AutoResetEvent.WaitOne((int)remainingMs);
                else Thread.Yield();
            }
        }

        #endregion Private Methods
    }
}
