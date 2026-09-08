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
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public class Injector
    {
        private readonly Dictionary<Type, object> _Binders = new();

        public Injector()
        {
            _Binders[typeof(Injector)] = this;
        }

        public T Get<T>()
        {
            return (T)_Binders[typeof(T)];
        }

        public void Bind<T>(T binder)
        {
            _Binders[typeof(T)] = binder!;
        }

        public void BindAllInterfaces(object objectToBind)
        {
            Type[] interfaces = objectToBind.GetType().GetInterfaces();

            foreach (Type iInterface in interfaces) _Binders[iInterface] = objectToBind;
        }

        public void TriggerInjectOnBinders()
        {
            HashSet<object> filledObjects = new HashSet<object>();

            foreach (object binderObject in _Binders.Values)
            {
                if (filledObjects.Contains(binderObject)) continue;
                Inject(binderObject);
                filledObjects.Add(binderObject);
            }
        }

        public void Inject(object objectToFill)
        {
            IEnumerable<PropertyInfo> properties = objectToFill.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                               .Where(prop => prop.IsDefined(typeof(InjectAttribute)));

            foreach (PropertyInfo propertyInfo in properties)
            {
                propertyInfo.SetValue(objectToFill, _Binders[propertyInfo.PropertyType]);
            }

            IEnumerable<FieldInfo> fields = objectToFill.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                        .Where(field => field.IsDefined(typeof(InjectAttribute)));

            foreach (FieldInfo fieldInfo in fields)
            {
                fieldInfo.SetValue(objectToFill, _Binders[fieldInfo.FieldType]);
            }
        }
    }
}