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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DeusaldSharp
{
    internal static class MessagesPool<T> where T : class, new()
    {
        #region Types

        private sealed class ReferenceComparer : IEqualityComparer<T>
        {
            public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
            public int  GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        }

        #endregion Types

        #region Variables

        internal static int NumberOfFreeMessages => _FreeMessages.Count;

        private static readonly Stack<T>                  _FreeMessages    = new Stack<T>();
        private static readonly HashSet<T>                _FreeMessagesSet = new HashSet<T>(new ReferenceComparer());
        private static readonly PropertyInfo[]            _Properties      = CollectProperties();
        private static readonly FieldInfo[]               _Fields          = CollectFields();
        private static readonly Dictionary<Type, object?> _DefaultValues   = new Dictionary<Type, object?>();

        #endregion Variables

        #region Public Methods

        public static T Get()
        {
            if (_FreeMessages.Count == 0) return new T();

            T result = _FreeMessages.Pop();
            _FreeMessagesSet.Remove(result);
            return result;
        }

        public static void Return(T message)
        {
            if (!_FreeMessagesSet.Add(message)) return;

            foreach (PropertyInfo property in _Properties)
            {
                property.SetValue(message, GetDefaultValue(property.PropertyType));
            }

            foreach (FieldInfo field in _Fields)
            {
                field.SetValue(message, GetDefaultValue(field.FieldType));
            }

            _FreeMessages.Push(message);
        }

        #endregion Public Methods

        #region Private Methods

        private static PropertyInfo[] CollectProperties()
        {
            List<PropertyInfo> result = new List<PropertyInfo>();

            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanWrite || property.GetIndexParameters().Length != 0) continue;
                result.Add(property);
            }

            return result.ToArray();
        }

        private static FieldInfo[] CollectFields()
        {
            List<FieldInfo> result = new List<FieldInfo>();

            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsInitOnly) continue;
                result.Add(field);
            }

            return result.ToArray();
        }

        private static object? GetDefaultValue(Type type)
        {
            if (!type.IsValueType) return null;

            if (_DefaultValues.TryGetValue(type, out object? value)) return value;

            value                = Activator.CreateInstance(type);
            _DefaultValues[type] = value;
            return value;
        }

        #endregion Private Methods
    }
}