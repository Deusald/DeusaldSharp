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
using System.Runtime.ExceptionServices;

namespace DeusaldSharp
{
    /// <summary> Class for controlling messages system.
    /// It is not thread-safe: all methods must be called from a single thread. </summary>
    public static class MsgCtrl
    {
        #region Types

        private class Binder
        {
            public Type     MessageType { get; set; } = null!;
            public Delegate Binding     { get; set; } = null!;
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

        /// <summary> Type specific operations on messages, so dispatching and pooling work on the runtime type of a message. </summary>
        private abstract class MessageOps
        {
            public abstract void Invoke(Delegate binding, object message);
            public abstract void Return(object message);
        }

        private sealed class MessageOps<T> : MessageOps where T : class, new()
        {
            public override void Invoke(Delegate binding, object message) => ((Action<T>)binding)((T)message);
            public override void Return(object message)                   => MessagesPool<T>.Return((T)message);
        }

        /// <summary> Used for message types that can't be pooled (no public parameterless constructor). </summary>
        private sealed class NotPooledMessageOps : MessageOps
        {
            public override void Invoke(Delegate binding, object message)
            {
                try
                {
                    binding.DynamicInvoke(message);
                }
                catch (TargetInvocationException exception) when (exception.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
            }

            public override void Return(object message) { }
        }

        #endregion Types

        #region Variables

        private static readonly Dictionary<Type, List<Binder>>     _Bindings       = new Dictionary<Type, List<Binder>>();
        private static readonly HashSet<Delegate>                  _BoundDelegates = new HashSet<Delegate>();
        private static readonly Dictionary<Type, List<MethodData>> _MethodCache    = new Dictionary<Type, List<MethodData>>();
        private static readonly Dictionary<Type, MessageOps>       _MessageOps     = new Dictionary<Type, MessageOps>();
        private static readonly Queue<(bool add, Binder binder)>   _BindersQueue   = new Queue<(bool, Binder)>();
        private static readonly Queue<object>                      _MessagesQueue  = new Queue<object>();

        private static State _State = State.Idle;
        private static bool  _Consume;
        private static int   _ClearGeneration;

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
            _BindersQueue.Enqueue((true, new Binder { MessageType = messageType, Binding = method, Priority = priority }));
            ProcessQueues();
        }

        /// <summary> Unbind dynamically method for a message type.  </summary>
        public static void Unbind<T>(Action<T> method) where T : class, new()
        {
            Type messageType = typeof(T);
            _BindersQueue.Enqueue((false, new Binder { MessageType = messageType, Binding = method, Priority = 0 }));
            ProcessQueues();
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

            for (int x = 0; x < _MethodCache[classType].Count; ++x)
            {
                MethodData      methodData = _MethodCache[classType][x];
                ParameterInfo[] parameters = methodData.Info.GetParameters();

                if (parameters.Length != 1) throw new InvalidOperationException("Can't bind method that have more or less parameters than 1.");

                Type parameterType = parameters[0].ParameterType;

                if (!parameterType.IsClass)
                {
                    throw new InvalidOperationException($"Trying to bind method {methodData.Info.Name} with parameter of type {parameterType.Name} which is not a class.");
                }

                Type     delegateType = Expression.GetActionType(parameterType);
                Delegate callback     = Delegate.CreateDelegate(delegateType, classObjectToRegister, methodData.Info);
                _BindersQueue.Enqueue((true, new Binder { MessageType = methodData.MessageType, Binding = callback, Priority = methodData.Priority }));
            }

            ProcessQueues();
        }

        /// <summary> Unregister all class methods that are marked by [MessageSlot] custom attribute.  </summary>
        public static void Unregister<T>(T classObjectToUnregister) where T : class
        {
            Type classType = classObjectToUnregister.GetType();

            if (!_MethodCache.TryGetValue(classType, out List<MethodData>? methodData))
            {
                throw new InvalidOperationException("Trying to unregister not registered class.");
            }

            foreach (MethodData method in methodData)
            {
                Type     delegateType = Expression.GetActionType(method.MessageType);
                Delegate callback     = Delegate.CreateDelegate(delegateType, classObjectToUnregister, method.Info);
                _BindersQueue.Enqueue((false, new Binder { MessageType = method.MessageType, Binding = callback, Priority = 0 }));
            }

            ProcessQueues();
        }

        /// <summary> Clear all messages controller data. Unregister all bindings and drop queued messages.
        /// Can be called from inside a message receiver - remaining receivers won't get the current message. </summary>
        public static void Clear()
        {
            _Bindings.Clear();
            _BoundDelegates.Clear();
            _MethodCache.Clear();
            _BindersQueue.Clear();

            while (_MessagesQueue.Count != 0)
            {
                object message = _MessagesQueue.Dequeue();
                GetMessageOps(message.GetType()).Return(message);
            }

            _Consume = false;
            ++_ClearGeneration;
        }

        /// <summary> Allocate new message. </summary>
        public static T Allocate<T>() where T : class, new()
        {
            return MessagesPool<T>.Get();
        }

        /// <summary> Deallocate message that wasn't send. </summary>
        public static void Deallocate<T>(T message) where T : class, new()
        {
            if (message.GetType() == typeof(T))
            {
                MessagesPool<T>.Return(message);
                return;
            }

            GetMessageOps(message.GetType()).Return(message);
        }

        /// <summary> Consume message. This will stop message propagation.
        /// Has no effect when called outside of a message receiver. </summary>
        public static void Consume()
        {
            if (_State != State.Signaling) return;
            _Consume = true;
        }

        /// <summary> Send message to all receivers.
        /// Exception thrown by a receiver propagates to the caller of Send. Messages that are still queued at that moment
        /// stay in the queue and are delivered with the next Send. </summary>
        public static void Send<T>(T message) where T : class, new()
        {
            if (message.GetType() == typeof(T) && !_MessageOps.ContainsKey(typeof(T)))
            {
                _MessageOps[typeof(T)] = new MessageOps<T>();
            }

            _MessagesQueue.Enqueue(message);
            ProcessQueues();
        }

        #endregion Public Methods

        #region Private Methods

        private static List<MethodData> CreateClassMethodCache(Type classType)
        {
            List<MethodData>    result          = new List<MethodData>();
            HashSet<MethodInfo> baseDefinitions = new HashSet<MethodInfo>();

            // Private methods of base classes are not returned for a derived type, so the hierarchy is walked manually
            for (Type? type = classType; type != null && type != typeof(object); type = type.BaseType)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly
                                                     | BindingFlags.Public   | BindingFlags.NonPublic);

                foreach (MethodInfo method in methods)
                {
                    if (method.IsAbstract || method.IsConstructor) continue;

                    // Overridden method is registered only once (the delegate is dispatched virtually anyway)
                    if (!baseDefinitions.Add(method.GetBaseDefinition())) continue;

                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length != 1) continue;

                    Type messageType = parameters[0].ParameterType;

                    if (!(method.GetCustomAttributes(typeof(MessageSlotAttribute), true) is MessageSlotAttribute[] { Length: 1 } attributes)) continue;

                    MethodData methodData = new MethodData { Info = method, MessageType = messageType, Priority = attributes[0].Priority };
                    result.Add(methodData);
                }
            }

            return result;
        }

        private static MessageOps GetMessageOps(Type messageType)
        {
            if (_MessageOps.TryGetValue(messageType, out MessageOps? ops)) return ops;

            try
            {
                ops = (MessageOps)Activator.CreateInstance(typeof(MessageOps<>).MakeGenericType(messageType))!;
            }
            catch (ArgumentException)
            {
                // Type doesn't satisfy class, new() constraints
                ops = new NotPooledMessageOps();
            }

            _MessageOps[messageType] = ops;
            return ops;
        }

        private static void ProcessQueues()
        {
            if (_State != State.Idle) return;

            try
            {
                while (true)
                {
                    if (_BindersQueue.Count != 0)
                    {
                        _State = State.BindingsEdit;
                        ExecuteBinderQueue();
                        continue;
                    }

                    if (_MessagesQueue.Count == 0) break;

                    _State = State.Signaling;
                    DispatchMessage(_MessagesQueue.Dequeue());
                }
            }
            finally
            {
                _State   = State.Idle;
                _Consume = false;
            }
        }

        private static void DispatchMessage(object message)
        {
            Type       messageType = message.GetType();
            MessageOps ops         = GetMessageOps(messageType);

            try
            {
                if (!_Bindings.TryGetValue(messageType, out List<Binder>? bindings)) return;

                // Bindings edits are queued during signaling, only Clear can drop them
                int clearGeneration = _ClearGeneration;

                for (int x = 0; x < bindings.Count; ++x)
                {
                    ops.Invoke(bindings[x].Binding, message);

                    if (_Consume || clearGeneration != _ClearGeneration) break;
                }
            }
            finally
            {
                _Consume = false;
                ops.Return(message);
            }
        }

        private static void ExecuteBinderQueue()
        {
            while (_BindersQueue.Count != 0)
            {
                (bool add, Binder binder) = _BindersQueue.Dequeue();

                if (add)
                {
                    if (!_BoundDelegates.Add(binder.Binding)) continue;

                    Binder newBinder = new Binder { MessageType = binder.MessageType, Binding = binder.Binding, Priority = binder.Priority };

                    if (_Bindings.TryGetValue(binder.MessageType, out List<Binder>? binding))
                    {
                        binding.Add(newBinder);
                    }
                    else
                    {
                        _Bindings[binder.MessageType] = new List<Binder> { newBinder };
                    }
                }
                else
                {
                    if (!_BoundDelegates.Remove(binder.Binding)) continue;

                    if (_Bindings.TryGetValue(binder.MessageType, out List<Binder>? binding))
                    {
                        binding.RemoveAll(x => x.Binding == binder.Binding);
                    }
                }
            }

            foreach (List<Binder> bindings in _Bindings.Values)
            {
                bindings.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            }
        }

        #endregion Private Methods
    }
}
