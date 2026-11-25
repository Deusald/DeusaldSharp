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
    internal class CoRoutine : ICoHandle
    {
        #region Types

        private enum CoState : byte
        {
            End,
            Running,
            WaitingForSeconds,
            WaitingUntilDone,
            WaitingUntilTrue,
            WaitingUntilFalse
        }

        #endregion Types

        #region Variables

        private CoRoCtrl.WaitUntilCondition? _Condition;
        private ICoHandle?                   _WaitUntilDone;
        private float                        _SecondsToWait;
        private CoState                      _CoState;
        private bool                         _IsExecuting;
        private bool                         _IsDisposed;

        private readonly uint                 _CoId;
        private readonly IEnumerator<ICoData> _Enumerator;
        private readonly CoRoCtrl             _CoRoCtrl;

        #endregion Variables

        #region Properties

        public bool IsAlive => _CoState != CoState.End;

        public CoTag CoTag                { get; }
        public uint  CoMask               { get; }
        public bool  IsPaused             { get; set; }
        public bool  InnerCreatedAndMoved { get; set; }

        #endregion Properties

        #region Init Methods

        internal CoRoutine(IEnumerator<ICoData> enumerator, uint coId, CoTag coTag, uint coMask, CoRoCtrl coRoCtrl)
        {
            CoTag          = coTag;
            CoMask         = coMask;
            _SecondsToWait = 0f;
            _CoId          = coId;
            _Enumerator    = enumerator;
            _CoState       = CoState.Running;
            _CoRoCtrl      = coRoCtrl;

            Register();
        }

        #endregion Init Methods

        #region Public Methods

        public void Kill()
        {
            End();
        }

        public void MoveCoRoutine(float deltaTime)
        {
            if (IsPaused || _CoState == CoState.End) return;

            switch (_CoState)
            {
                case CoState.WaitingForSeconds:
                {
                    _SecondsToWait -= deltaTime;

                    if (_SecondsToWait > 0f) break;

                    // Overshoot (zero or negative) is carried over to the next WaitForSeconds so repeating waits don't drift
                    CoRoutineNextStep(_SecondsToWait);
                    break;
                }

                case CoState.WaitingUntilDone:
                {
                    if (_WaitUntilDone is { IsAlive: true }) break;

                    CoRoutineNextStep(0f);
                    break;
                }

                case CoState.WaitingUntilTrue:
                {
                    if (_Condition != null && !_Condition()) break;

                    CoRoutineNextStep(0f);
                    break;
                }

                case CoState.WaitingUntilFalse:
                {
                    if (_Condition != null && _Condition()) break;

                    CoRoutineNextStep(0f);
                    break;
                }

                case CoState.Running:
                {
                    CoRoutineNextStep(0f);
                    break;
                }
            }
        }

        public void UnRegister()
        {
            _CoRoCtrl.OrderToCoRoutinesViaCoTag  -= OrderToCoRoutinesViaCoTag;
            _CoRoCtrl.OrderToCoRoutinesViaCoMask -= OrderToCoRoutinesViaCoMask;
        }

        #region Equals

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CoRoutine)obj);
        }

        public override int GetHashCode()
        {
            return (int)_CoId;
        }

        public static bool operator ==(CoRoutine left, CoRoutine right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CoRoutine left, CoRoutine right)
        {
            return !Equals(left, right);
        }

        #endregion Equals

        #endregion Public Methods

        #region Private Methods

        private void CoRoutineNextStep(float carriedSeconds)
        {
            _CoState = CoState.Running;

            bool hasNext = false;
            _IsExecuting = true;

            try
            {
                hasNext = _Enumerator.MoveNext();
            }
            finally
            {
                _IsExecuting = false;

                // Finished, faulted or killed from inside its own body (directly or via tag/mask)
                if (!hasNext || _CoState == CoState.End) End();
            }

            if (_CoState == CoState.End) return;

            ICoData currentState = _Enumerator.Current!;

            switch (currentState.Type)
            {
                case CoDataType.WaitOneTick:
                {
                    return;
                }

                case CoDataType.WaitForSeconds:
                {
                    // Carried debt is limited to one wait length, so after a long stall the waits don't fire every frame
                    _CoState       = CoState.WaitingForSeconds;
                    _SecondsToWait = Math.Max(((CoDataWaitForSeconds)currentState).SecondsToWait + carriedSeconds, 0f);
                    return;
                }

                case CoDataType.WaitUntilDone:
                {
                    _CoState       = CoState.WaitingUntilDone;
                    _WaitUntilDone = ((CoDataWaitUntilDone)currentState).CoRoutineToWaitFor;
                    return;
                }

                case CoDataType.WaitUntilTrue:
                {
                    _CoState   = CoState.WaitingUntilTrue;
                    _Condition = ((CoDataWaitUntilCondition)currentState).Condition;
                    return;
                }

                case CoDataType.WaitUntilFalse:
                {
                    _CoState   = CoState.WaitingUntilFalse;
                    _Condition = ((CoDataWaitUntilCondition)currentState).Condition;
                    return;
                }
            }
        }

        /// <summary> Marks CoRoutine as ended. Unregisters it and disposes the enumerator (exactly once),
        /// unless the enumerator is executing right now - then it's done when MoveNext returns. </summary>
        private void End()
        {
            _CoState = CoState.End;

            if (_IsExecuting || _IsDisposed) return;

            _IsDisposed = true;
            UnRegister();
            _Enumerator.Dispose();
        }

        private void Register()
        {
            _CoRoCtrl.OrderToCoRoutinesViaCoTag  += OrderToCoRoutinesViaCoTag;
            _CoRoCtrl.OrderToCoRoutinesViaCoMask += OrderToCoRoutinesViaCoMask;
        }

        private void OrderToCoRoutinesViaCoTag(CoRoCtrl.CallbackOrder order, CoTag coTag)
        {
            if (coTag != CoTag) return;
            ExecuteOrder(order);
        }

        private void OrderToCoRoutinesViaCoMask(CoRoCtrl.CallbackOrder order, uint coMask, CoRoCtrl.MaskType maskType)
        {
            if (maskType == CoRoCtrl.MaskType.All && !CoMask.HasAllBitsOn(coMask)) return;
            if (maskType == CoRoCtrl.MaskType.Any && !CoMask.HasAnyBitOn(coMask)) return;
            ExecuteOrder(order);
        }

        private void ExecuteOrder(CoRoCtrl.CallbackOrder order)
        {
            switch (order)
            {
                case CoRoCtrl.CallbackOrder.Kill:
                {
                    End();
                    return;
                }

                case CoRoCtrl.CallbackOrder.Pause:
                {
                    IsPaused = true;
                    return;
                }

                case CoRoCtrl.CallbackOrder.Unpause:
                {
                    IsPaused = false;
                    return;
                }
            }
        }

        private bool Equals(CoRoutine other)
        {
            return _CoId == other._CoId;
        }

        #endregion Private Methods
    }
}
