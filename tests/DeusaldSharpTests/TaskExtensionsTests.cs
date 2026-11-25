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
using DeusaldSharp;
using NUnit.Framework;
using DeusaldTaskExtensions = DeusaldSharp.TaskExtensions;

namespace DeusaldSharpTests
{
    // Tests mutate the static DeusaldTaskExtensions.UnhandledException, so they must never run in parallel.
    [NonParallelizable]
    public class TaskExtensionsTests
    {
        private static readonly TimeSpan _Timeout = TimeSpan.FromSeconds(5);

        [TearDown]
        public void TearDown()
        {
            DeusaldTaskExtensions.UnhandledException = null;
        }

        private static async Task<T> WithTimeout<T>(Task<T> task)
        {
            Task finished = await Task.WhenAny(task, Task.Delay(_Timeout));
            if (finished != task) Assert.Fail("Timed out waiting for the exception handler.");
            return await task;
        }

        private static async Task FaultAsync(Exception exception)
        {
            await Task.Yield();
            throw exception;
        }

        [Test]
        public async Task Forget_FaultedTask_PerCallHandlerReceivesException()
        {
            TaskCompletionSource<Exception> received = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
            InvalidOperationException       thrown   = new InvalidOperationException("boom");

            FaultAsync(thrown).Forget(e => received.TrySetResult(e));

            Assert.That(await WithTimeout(received.Task), Is.SameAs(thrown));
        }

        [Test]
        public async Task Forget_FaultedTaskWithoutHandler_GlobalHandlerReceivesException()
        {
            TaskCompletionSource<Exception> received = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
            InvalidOperationException       thrown   = new InvalidOperationException("boom");
            DeusaldTaskExtensions.UnhandledException = e => received.TrySetResult(e);

            FaultAsync(thrown).Forget();

            Assert.That(await WithTimeout(received.Task), Is.SameAs(thrown));
        }

        [Test]
        public async Task Forget_PerCallHandler_TakesPrecedenceOverGlobal()
        {
            TaskCompletionSource<Exception> received     = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
            bool                            globalCalled = false;
            DeusaldTaskExtensions.UnhandledException = _ => globalCalled = true;

            FaultAsync(new Exception("boom")).Forget(e => received.TrySetResult(e));

            await WithTimeout(received.Task);
            Assert.That(globalCalled, Is.False);
        }

        [Test]
        public async Task Forget_CancelledTask_NoHandlerCalled()
        {
            bool called = false;
            DeusaldTaskExtensions.UnhandledException = _ => called = true;

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            Task cancelled = Task.Delay(Timeout.Infinite, cts.Token);
            cancelled.Forget(_ => called = true);

            // An OperationCanceledException thrown from inside an async method must be ignored too.
            Task throwsCancel = FaultAsync(new OperationCanceledException());
            throwsCancel.Forget(_ => called = true);

            await Task.WhenAny(Task.WhenAll(cancelled, throwsCancel), Task.Delay(_Timeout));
            await Task.Delay(50); // let the Forget continuations run
            Assert.That(called, Is.False);
        }

        [Test]
        public async Task Forget_ThrowingHandler_DoesNotCrash()
        {
            TaskCompletionSource<bool> handlerEntered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            FaultAsync(new Exception("boom")).Forget(_ =>
            {
                handlerEntered.TrySetResult(true);
                throw new InvalidOperationException("handler failure");
            });

            Assert.That(await WithTimeout(handlerEntered.Task), Is.True);

            // Same through the global handler.
            TaskCompletionSource<bool> globalEntered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            DeusaldTaskExtensions.UnhandledException = _ =>
            {
                globalEntered.TrySetResult(true);
                throw new InvalidOperationException("global handler failure");
            };
            FaultAsync(new Exception("boom")).Forget();

            Assert.That(await WithTimeout(globalEntered.Task), Is.True);
            await Task.Delay(50); // give a potential escaping exception the chance to surface
        }

        [Test]
        public async Task Forget_SuccessfulTask_NoHandlerCalled()
        {
            bool called = false;
            DeusaldTaskExtensions.UnhandledException = _ => called = true;

            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task                       task   = source.Task;
            task.Forget(_ => called = true);
            source.SetResult(true);

            await task;
            await Task.Delay(50);
            Assert.That(called, Is.False);
        }

        [Test]
        public async Task Forget_GenericTask_Works()
        {
            TaskCompletionSource<Exception> received = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
            ArgumentException               thrown   = new ArgumentException("boom");

            Task<int> generic = Task.Run(new Func<int>(() => throw thrown));
            generic.Forget(e => received.TrySetResult(e));

            Assert.That(await WithTimeout(received.Task), Is.SameAs(thrown));
        }
    }
}
