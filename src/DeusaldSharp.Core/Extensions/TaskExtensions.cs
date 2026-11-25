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
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable AsyncVoidMethod

namespace DeusaldSharp
{
    [PublicAPI]
    public static class TaskExtensions
    {
        /// <summary>
        /// Global fallback handler for exceptions thrown by tasks passed to <see cref="Forget"/>
        /// when no per-call handler was provided. Cancellation is never reported.
        /// </summary>
        public static Action<Exception>? UnhandledException { get; set; }

        /// <summary>
        /// Runs the task without awaiting it. Exceptions never escape to the synchronization context
        /// (which would crash the process); they are passed to <paramref name="onException"/> or,
        /// if that is null, to <see cref="UnhandledException"/>.
        /// </summary>
        public static async void Forget(this Task task, Action<Exception>? onException = null)
        {
            try
            {
                // No ConfigureAwait(false) on purpose: the handler should run on the caller context (e.g. Unity main thread).
                await task;
            }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                try
                {
                    (onException ?? UnhandledException)?.Invoke(exception);
                }
                catch
                {
                    // A throwing handler must not crash the process either.
                }
            }
        }
    }
}