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

namespace DeusaldSharp.Messages
{
    internal static class MessagesPool<T> where T : class, new()
    {
        #region Variables

        internal static         int     NumberOfFreeMessages => _FreeMessages.Count;
        private static readonly List<T> _FreeMessages = new List<T>();

        #endregion Variables

        #region Public Methods

        public static T Get()
        {
            if (_FreeMessages.Count != 0)
            {
                T result = _FreeMessages[0];
                _FreeMessages.RemoveAt(0);
                return result;
            }

            return new T();
        }

        public static void Return(T message)
        {
            if (_FreeMessages.Contains(message)) return;

            Type           type       = message.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                property.SetValue(message, default);
            }

            _FreeMessages.Add(message);
        }

        #endregion Public Methods
    }
}