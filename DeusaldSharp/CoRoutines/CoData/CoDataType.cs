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
    /// <summary> Type of next CoRoutine execution order. </summary>
    internal enum CoDataType : byte
    {
        /// <summary> CoRoutine will be hold for one tick. </summary>
        WaitOneTick,
        
        /// <summary> CoRoutine will be hold for certain amount of seconds. </summary>
        WaitForSeconds,
        
        /// <summary> CoRoutine will be hold until the other CoRoutine is alive. </summary>
        WaitUntilDone,

        /// <summary> CoRoutine will be hold until the given condition is false. </summary>
        WaitUntilTrue,
        
        /// <summary> CoRoutine will be hold until the given condition is true. </summary>
        WaitUntilFalse,
    }
}