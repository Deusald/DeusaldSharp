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

namespace DeusaldSharp
{
    /// <summary> Interface for getting information about RoCoroutine and interacting with it. </summary>
    public interface ICoHandle
    {
        /// <summary> Assigned CoTag for this CoRoutine. </summary>
        CoTag CoTag { get; }
        
        /// <summary> Assigned bitwise mask for this CoRoutine. </summary>
        uint CoMask { get; }

        /// <summary> Returns true if CoRoutine is still processing otherwise false. </summary>
        bool IsAlive { get; }

        /// <summary> Returns true if executing coroutine is currently on hold otherwise false.
        /// Set true to pause execution CoRoutine, set false to unpause. </summary>
        bool IsPaused { get; set; }

        /// <summary> Call to end execution of this CoRoutine. </summary>
        void Kill();
    }
}