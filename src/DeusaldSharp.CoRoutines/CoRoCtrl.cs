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
using System.Collections.Generic;

namespace DeusaldSharp
{
    /// <summary> Controller for all CoRoutines actions and logic. </summary>
    public class CoRoCtrl
    {
        #region Types

        /// <summary> Enum that defines how CoMask should be checked. </summary>
        public enum MaskType
        {
            /// <summary> All bits from given mask needs to be set for operation to continue. </summary>
            All,

            /// <summary> Any of the bits provided by mask needs to be set for operation to continue. </summary>
            Any
        }

        internal enum CallbackOrder
        {
            Kill,
            Pause,
            Unpause
        }

        public delegate bool WaitUntilCondition();

        internal delegate void CoTagCallback(CallbackOrder order, CoTag coTag);

        internal delegate void CoMaskCallback(CallbackOrder order, uint coMask, MaskType maskType);

        #endregion Types

        #region Variables

        internal event CoTagCallback?  OrderToCoRoutinesViaCoTag;
        internal event CoMaskCallback? OrderToCoRoutinesViaCoMask;

        private uint                       _NextCoId = 1;
        private float                      _DeltaTime;
        private LinkedListNode<CoRoutine>? _CurrentExecutedCoRoutine;

        private readonly LinkedList<CoRoutine> _CoRoutines = new LinkedList<CoRoutine>();

        #endregion Variables

        #region Properties

        private uint _GetNextCoId => _NextCoId++;

        #endregion Properties

        #region Public Methods

        /// <summary> Execute all CoRoutines.
        /// WARNING: You should always execute engine tick first and at the end call this method. </summary>
        public void Update(float deltaTime)
        {
            _DeltaTime = deltaTime;
            MoveCoRoutines(_CoRoutines.First, deltaTime);
        }

        /// <summary> Kill all CoRoutines form all segments and clear RoRoutineController for fresh start. </summary>
        public void Reset()
        {
            _CurrentExecutedCoRoutine = null;

            for (LinkedListNode<CoRoutine>? node = _CoRoutines.First; node != null;)
            {
                node.Value.UnRegister();
                node = node.Next;
            }

            _CoRoutines.Clear();
        }

        /// <summary> This method returns current executed CoHandle. This method must be called inside CoRoutine body. </summary>
        public ICoHandle GetCurrentCoHandle()
        {
            if (_CurrentExecutedCoRoutine == null)
            {
                throw new InvalidOperationException("Method called outside of CoRoutine body!");
            }

            return _CurrentExecutedCoRoutine.Value;
        }

        /// <summary> Kill all CoRoutines with given CoTag. </summary>
        public void KillCoRoutines(CoTag coTag)
        {
            OrderToCoRoutinesViaCoTag?.Invoke(CallbackOrder.Kill, coTag);
        }

        /// <summary> Kill all CoRoutines with given CoMask. </summary>
        public void KillCoRoutines(uint coMask, MaskType maskType)
        {
            OrderToCoRoutinesViaCoMask?.Invoke(CallbackOrder.Kill, coMask, maskType);
        }

        /// <summary> Pause all CoRoutines with given CoTag. </summary>
        public void PauseCoRoutines(CoTag coTag)
        {
            OrderToCoRoutinesViaCoTag?.Invoke(CallbackOrder.Pause, coTag);
        }

        /// <summary> Pause all CoRoutines with given CoMask. </summary>
        public void PauseCoRoutines(uint coMask, MaskType maskType)
        {
            OrderToCoRoutinesViaCoMask?.Invoke(CallbackOrder.Pause, coMask, maskType);
        }

        /// <summary> Unpause all CoRoutines with given CoTag. </summary>
        public void UnpauseCoRoutines(CoTag coTag)
        {
            OrderToCoRoutinesViaCoTag?.Invoke(CallbackOrder.Unpause, coTag);
        }

        /// <summary> Unpause all CoRoutines with given CoMask. </summary>
        public void UnpauseCoRoutines(uint coMask, MaskType maskType)
        {
            OrderToCoRoutinesViaCoMask?.Invoke(CallbackOrder.Unpause, coMask, maskType);
        }

        #region CoRoutine Yield Api

        /// <summary> Creates new CoRoutine and perform first execution.  </summary>
        public ICoHandle RunCoRoutine(IEnumerator<ICoData> newCoRoutine, CoTag coTag = default, uint coMask = 0)
        {
            CoRoutine coRoutine = new CoRoutine(newCoRoutine, _GetNextCoId, coTag, coMask, this);
            AddNewCoRoutine(coRoutine);

            coRoutine.MoveCoRoutine(_DeltaTime);
            coRoutine.InnerCreatedAndMoved = true;

            return coRoutine;
        }

        /// <summary> Yield return this to make CoRoutine wait one segment tick. </summary>
        public ICoData WaitForOneTick()
        {
            return new CoDataWaitForTick();
        }

        /// <summary> Yield return this to make CoRoutine wait given number of seconds. </summary>
        public ICoData WaitForSeconds(float seconds)
        {
            return new CoDataWaitForSeconds(seconds);
        }

        /// <summary> Yield return this to make CoRoutine wait until the other CoRoutine is alive. </summary>
        public ICoData WaitUntilDone(IEnumerator<ICoData> newCoRoutine, CoTag coTag = default, uint coMask = 0)
        {
            ICoHandle coRoutine = RunCoRoutine(newCoRoutine, coTag, coMask);
            return new CoDataWaitUntilDone(coRoutine);
        }

        /// <summary> Yield return this to make CoRoutine wait until the other CoRoutine is alive. </summary>
        public ICoData WaitUntilDone(ICoHandle coRoutine)
        {
            return new CoDataWaitUntilDone(coRoutine);
        }

        /// <summary> Yield return this to make CoRoutine wait until the condition will be true. </summary>
        public ICoData WaitUntilTrue(WaitUntilCondition condition)
        {
            return new CoDataWaitUntilCondition(true, condition);
        }

        /// <summary> Yield return this to make CoRoutine wait until the condition will be false. </summary>
        public ICoData WaitUntilFalse(WaitUntilCondition condition)
        {
            return new CoDataWaitUntilCondition(false, condition);
        }

        #endregion CoRoutine Yield Api

        #endregion Public Methods

        #region Private Methods

        private void MoveCoRoutines(LinkedListNode<CoRoutine>? first, float deltaTime)
        {
            for (LinkedListNode<CoRoutine>? node = first; node != null;)
            {
                _CurrentExecutedCoRoutine = node;

                if (!node.Value.InnerCreatedAndMoved)
                {
                    node.Value.MoveCoRoutine(deltaTime);
                }

                LinkedListNode<CoRoutine>? next = node.Next;
                node.Value.InnerCreatedAndMoved = false;

                if (!node.Value.IsAlive)
                {
                    node.List!.Remove(node);
                }

                node = next;
            }

            _CurrentExecutedCoRoutine = null;
        }

        private void AddNewCoRoutine(CoRoutine coRoutine)
        {
            if (_CurrentExecutedCoRoutine != null)
            {
                _CurrentExecutedCoRoutine.List!.AddAfter(_CurrentExecutedCoRoutine, coRoutine);
            }
            else
            {
                _CoRoutines.AddLast(coRoutine);
            }
        }

        #endregion Private Methods
    }
}