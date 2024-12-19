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
using System.Threading;
using System.Threading.Tasks;

namespace DeusaldSharp
{
    /// <summary> Clock for multiplayer game servers. </summary>
    public class PrecisionClock
    {
        #region Public Types

        public delegate void TickDelegate(ulong frameNumber);

        #endregion Public Types

        #region Properties

        /// <summary> Is Clock still executing. </summary>
        public bool IsAlive { get; private set; }

        #endregion Properties

        #region Variables

        /// <summary> This event will be executed with every tick. </summary>
        public event TickDelegate? Tick;

        /// <summary> This event will be executed if one frame will take more time we can give. </summary>
        public event Action? TooLongFrame;

        #endregion Variables

        #region Special Methods

        public PrecisionClock(ushort ticksPerSecond)
        {
            IsAlive = true;

            if (MathUtils.SEC_TO_MILLISECONDS % ticksPerSecond != 0)
            {
                throw new Exception("Number of ticks per second must be factor of 1000.");
            }

            double millisecondsForFrame = MathUtils.SEC_TO_MILLISECONDS / (double)ticksPerSecond;

            Thread clockThread = new Thread(ClockStep(TimeSpan.FromMilliseconds(millisecondsForFrame)).Wait)
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

        private async Task ClockStep(TimeSpan interval)
        {
            ulong frame       = 0;
            long  stage1Delay = 20;
            long  stage2Delay = 5 * TimeSpan.TicksPerMillisecond;

            DateTime target = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)stage1Delay + 2);
            bool     warmup = true;

            while (IsAlive)
            {
                // Getting closer to 'target' - Lets do the less precise but least cpu intensive wait
                TimeSpan timeLeft = target - DateTime.Now;

                if (timeLeft.TotalMilliseconds >= stage1Delay)
                {
                    await Task.Delay((int)(timeLeft.TotalMilliseconds - stage1Delay));
                }

                // Getting closer to 'target' - Lets do the semi-precise but mild cpu intensive wait - Task.Yield()
                while (DateTime.Now < target - new TimeSpan(stage2Delay))
                {
                    await Task.Yield();
                }

                // Extremely close to 'target' - Lets do the most precise but very cpu/battery intensive 
                while (DateTime.Now < target)
                {
                    Thread.SpinWait(64);
                }

                if (!warmup)
                {
                    Tick?.Invoke(frame++);
                    target += interval;

                    if (DateTime.Now >= target) TooLongFrame?.Invoke();
                }
                else
                {
                    long start1   = DateTime.Now.Ticks + (long)interval.TotalMilliseconds * TimeSpan.TicksPerMillisecond;
                    long alignVal = start1 - start1 % ((long)interval.TotalMilliseconds * TimeSpan.TicksPerMillisecond);
                    target = new DateTime(alignVal);
                    warmup = false;
                }
            }
        }

        #endregion Private Methods
    }
}