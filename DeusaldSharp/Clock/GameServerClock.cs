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
    public class GameServerClock
    {
        #region Public Types

        public delegate void TickDelegate(ulong frameNumber, double deltaTime);

        #endregion Public Types

        #region Properties

        /// <summary> Is Clock still executing. </summary>
        public bool IsAlive { get; private set; }

        #endregion Properties

        #region Variables

        /// <summary> This event will be executed with every tick. </summary>
        public event TickDelegate? Tick;

        /// <summary> Used to log information about Clock. </summary>
        public event Action<string>? Log;

        private readonly AutoResetEvent _AutoResetEvent;
        private readonly long           _DeltaTimeInMs;
        private readonly ushort         _LogEveryXFrame;

        #endregion Variables

        #region Special Methods

        public GameServerClock(ushort ticksPerSecond, ushort logEveryXFrame)
        {
            IsAlive         = true;
            _LogEveryXFrame = logEveryXFrame;
            _AutoResetEvent = new AutoResetEvent(true);
            _DeltaTimeInMs  = (long)(1.0m / ticksPerSecond * MathUtils.SEC_TO_MILLISECONDS);

            Thread clockThread = new Thread(ClockLoop)
            {
                IsBackground = true,
                Priority     = ThreadPriority.AboveNormal
            };

            clockThread.Start();
        }

        #endregion Special Methods

        #region Public Methods

        public void Kill()
        {
            IsAlive = false;
        }

        #endregion Public Methods

        #region Private Methods

        private void ClockLoop()
        {
            ulong  frameNumber                   = 0;
            long   nextTick                      = _DeltaTimeInMs;
            long   frameStart                    = 0;
            double sumTickLength                 = 0;
            ulong  framesSinceLastLog            = 0;
            long   longestTickLenghtSinceLastLog = 0;
            ulong  longestTickFrameNumber        = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            while (IsAlive)
            {
                long previousFrameStart = frameStart;
                frameStart = stopwatch.ElapsedMilliseconds;
                Tick?.Invoke(frameNumber, (frameStart - previousFrameStart) * MathUtils.MILLISECONDS_TO_SECONDS);
                long frameEnd = stopwatch.ElapsedMilliseconds;

                long frameLenght = frameEnd - frameStart;
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
                        $"Frame {frameNumber}. Average frame length since last log {(sumTickLength / framesSinceLastLog):0.000} ms. Longest tick length {longestTickLenghtSinceLastLog} ms in frame {longestTickFrameNumber}.");
                    sumTickLength                 = 0;
                    framesSinceLastLog            = 0;
                    longestTickLenghtSinceLastLog = 0;
                    longestTickFrameNumber        = 0;
                }

                if (stopwatch.ElapsedMilliseconds < nextTick)
                {
                    _AutoResetEvent.WaitOne((int)(nextTick - stopwatch.ElapsedMilliseconds));
                }

                nextTick += _DeltaTimeInMs;
            }
        }

        #endregion Private Methods
    }
}