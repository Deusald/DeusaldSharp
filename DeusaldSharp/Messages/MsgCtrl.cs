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
using System.Linq.Expressions;
using System.Reflection;

namespace DeusaldSharp.Messages
{
    /// <summary> Class for controlling messages system. </summary>
    public static class MsgCtrl
    {
        #region Types

        private class Binder
        {
            public Type     MessageType { get; set; }
            public Delegate Binding     { get; set; }
            public int      Priority    { get; set; }
        }

        private struct MethodData
        {
            public MethodInfo Info        { get; set; }
            public Type       MessageType { get; set; }
            public int        Priority    { get; set; }
        }

        private enum State
        {
            Idle,
            Signaling,
            BindingsEdit
        }

        #endregion Types

        #region Variables

        private static readonly Dictionary<Type, List<Binder>>     _Bindings       = new Dictionary<Type, List<Binder>>();
        private static readonly HashSet<Delegate>                  _BoundDelegates = new HashSet<Delegate>();
        private static readonly Dictionary<Type, List<MethodData>> _MethodCache    = new Dictionary<Type, List<MethodData>>();
        private static readonly Queue<(bool add, Binder binder)>   _BindersQueue   = new Queue<(bool, Binder)>();
        private static readonly Queue<object>                      _MessagesQueue  = new Queue<object>();

        private static State _State = State.Idle;
        private static bool  _Consume;

        #endregion Variables

        #region Public Methods

        /// <summary> Bind dynamically method for a message type.  </summary>
        public static void Bind<T>(Action<T> method, int priority = 0) where T : class, new()
        {
            ParameterInfo[] parameters = method.Method.GetParameters();

            if (parameters.Length != 1)
            {
                throw new InvalidOperationException("Delegate method needs to have only one parameter!");
            }

            if (parameters[0].ParameterType != typeof(T))
            {
                throw new InvalidOperationException($"First parameter of method needs to be of type {nameof(T)}");
            }

            Type messageType = parameters[0].ParameterType;
            _BindersQueue.Enqueue((true, new Binder {MessageType = messageType, Binding = method, Priority = priority}));

            if (_State != State.Idle) return;
            ExecuteBinderQueue();
        }

        /// <summary> Unbind dynamically method for a message type.  </summary>
        public static void Unbind<T>(Action<T> method) where T : class, new()
        {
            Type messageType = typeof(T);
            _BindersQueue.Enqueue((false, new Binder {MessageType = messageType, Binding = method, Priority = 0}));

            if (_State != State.Idle) return;
            ExecuteBinderQueue();
        }

        /// <summary> Register all class methods that are marked by [MessageSlot] custom attribute.  </summary>
        public static void Register<T>(T classObjectToRegister) where T : class
        {
            Type classType = classObjectToRegister.GetType();

            if (!_MethodCache.ContainsKey(classType))
            {
                _MethodCache.Add(classType, CreateClassMethodCache(classType));
            }

            if (_MethodCache[classType].Count == 0) throw new InvalidOperationException("Trying to register class without [MessageSlot]-annotated methods.");

            for (int i = 0; i < _MethodCache[classType].Count; ++i)
            {
                MethodData      methodData = _MethodCache[classType][i];
                ParameterInfo[] parameters = methodData.Info.GetParameters();

                if (parameters.Length != 1) throw new InvalidOperationException("Can't bind method that have more or less parameters than 1.");

                Type delegateType = Expression.GetActionType(parameters[0].ParameterType);

                if (!delegateType.IsClass) throw new InvalidOperationException("Trying to bind method with parameter which is not a class.");

                Delegate callback = Delegate.CreateDelegate(delegateType, classObjectToRegister, methodData.Info);
                _BindersQueue.Enqueue((true, new Binder {MessageType = methodData.MessageType, Binding = callback, Priority = methodData.Priority}));
            }

            if (_State != State.Idle) return;
            ExecuteBinderQueue();
        }

        /// <summary> Unregister all class methods that are marked by [MessageSlot] custom attribute.  </summary>
        public static void Unregister<T>(T classObjectToUnregister) where T : class
        {
            Type classType = classObjectToUnregister.GetType();

            if (!_MethodCache.ContainsKey(classType))
            {
                throw new InvalidOperationException("Trying to unregister not registered class.");
            }

            List<MethodData> methodData = _MethodCache[classType];

            foreach (MethodData method in methodData)
            {
                Type     delegateType = Expression.GetActionType(method.MessageType);
                Delegate callback     = Delegate.CreateDelegate(delegateType, classObjectToUnregister, method.Info);
                _BindersQueue.Enqueue((false, new Binder {MessageType = method.MessageType, Binding = callback, Priority = 0}));
            }

            if (_State != State.Idle) return;
            ExecuteBinderQueue();
        }

        /// <summary> Clear all messages controller data. Unregister all bindings. </summary>
        public static void Clear()
        {
            _Bindings.Clear();
            _BoundDelegates.Clear();
            _MethodCache.Clear();
            _BindersQueue.Clear();
            _MessagesQueue.Clear();

            _State   = State.Idle;
            _Consume = false;
        }

        /// <summary> Allocate new message. </summary>
        public static T Allocate<T>() where T : class, new()
        {
            return MessagesPool<T>.Get();
        }

        /// <summary> Deallocate message that wasn't send. </summary>
        public static void Deallocate<T>(T message) where T : class, new()
        {
            MessagesPool<T>.Return(message);
        }

        /// <summary> Consume message. This will stop message propagation. </summary>
        public static void Consume()
        {
            _Consume = true;
        }

        /// <summary> Send message to all receivers. </summary>
        public static void Send<T>(T message) where T : class, new()
        {
            _MessagesQueue.Enqueue(message);
            if (_State != State.Idle) return;
            ExecuteMessagesQueue();
        }

        #endregion Public Methods

        #region Private Methods

        private static List<MethodData> CreateClassMethodCache(Type classType)
        {
            List<MethodData> result = new List<MethodData>();

            MethodInfo[] methods = classType.GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                                        BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; ++i)
            {
                var method = methods[i];

                if (method.IsAbstract || method.IsConstructor)
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                {
                    continue;
                }

                var messageType = parameters[0].ParameterType;

                var attributes = method.GetCustomAttributes(typeof(MessageSlotAttribute), true) as MessageSlotAttribute[];

                if (!(attributes is {Length: 1}))
                {
                    continue;
                }

                MethodData methodData = new MethodData {Info = method, MessageType = messageType, Priority = attributes[0].Priority};
                result.Add(methodData);
            }

            return result;
        }

        private static void ExecuteMessagesQueue()
        {
            _State = State.Signaling;

            while (_MessagesQueue.Count != 0)
            {
                object message     = _MessagesQueue.Dequeue();
                Type   messageType = message.GetType();

                if (!_Bindings.ContainsKey(messageType)) continue;

                for (int i = 0; i < _Bindings[messageType].Count; ++i)
                {
                    _Bindings[messageType][i].Binding.DynamicInvoke(message);

                    if (_Consume) break;
                }

                _Consume = false;
                Deallocate(message);

                if (_BindersQueue.Count != 0) break;
            }

            _State = State.Idle;
            if (_BindersQueue.Count == 0) return;
            ExecuteBinderQueue();
        }

        private static void ExecuteBinderQueue()
        {
            _State = State.BindingsEdit;

            while (_BindersQueue.Count != 0)
            {
                (bool add, Binder binder) = _BindersQueue.Dequeue();

                if (add)
                {
                    if (_BoundDelegates.Contains(binder.Binding)) continue;

                    _BoundDelegates.Add(binder.Binding);

                    Binder newBinder = new Binder {MessageType = binder.MessageType, Binding = binder.Binding, Priority = binder.Priority};

                    if (_Bindings.ContainsKey(binder.MessageType))
                    {
                        _Bindings[binder.MessageType].Add(newBinder);
                    }
                    else
                    {
                        _Bindings[binder.MessageType] = new List<Binder> {newBinder};
                    }
                }
                else
                {
                    if (!_BoundDelegates.Contains(binder.Binding)) continue;

                    _BoundDelegates.Remove(binder.Binding);
                    _Bindings[binder.MessageType].RemoveAll(x => x.Binding == binder.Binding);
                }
            }

            foreach (List<Binder> bindings in _Bindings.Values)
            {
                bindings.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            }

            _State = State.Idle;
            if (_MessagesQueue.Count == 0) return;
            ExecuteMessagesQueue();
        }

        #endregion Private Methods
    }
}