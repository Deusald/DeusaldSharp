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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public struct TVector3<T> : IEnumerable<T>, IEquatable<TVector3<T>> where T : struct
    {
        #region Variables

        /// <summary> X component of the vector. </summary>
        public T x;

        /// <summary> Y component of the vector. </summary>
        public T y;

        /// <summary> Z component of the vector. </summary>
        public T z;

        #endregion Variables

        #region Init Methods

        /// <summary> Creates a new vector with given x, y, z components. </summary>
        public TVector3(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary> Creates a new vector with all x, y, z components set to value. </summary>
        public TVector3(T value)
        {
            x = value;
            y = value;
            z = value;
        }

        /// <summary> Creates a new vector where x and y components are taken from given Vector2 and z component is given. </summary>
        public TVector3(TVector2<T> value, T z)
        {
            x      = value.x;
            y      = value.y;
            this.z = z;
        }

        /// <summary> Creates a new vector with given x, y components from Vector2 and sets z to default. </summary>
        public TVector3(TVector2<T> value)
        {
            x = value.x;
            y = value.y;
            z = default;
        }

        /// <summary> Creates a new vector with given x, y components and sets z to zero. </summary>
        public TVector3(T x, T y)
        {
            this.x = x;
            this.y = y;
            z      = default;
        }

        /// <summary> Creates a new vector3 based on given Vector3. </summary>
        public TVector3(TVector3<T> value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        #endregion Init Methods
        
        #region Public Methods

        #region Enumerable

        public IEnumerator<T> GetEnumerator()
        {
            yield return x;
            yield return y;
            yield return z;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Enumerable

        #region Operators

        public T this[int key]
        {
            get
            {
                switch (key)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (key)
                {
                    case 0:
                    {
                        x = value;
                        break;
                    }
                    case 1:
                    {
                        y = value;
                        break;
                    }
                    case 2:
                    {
                        z = value;
                        break;
                    }
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion Operators
        
        #region Equals

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public bool Equals(TVector3<T> other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public override bool Equals(object? obj)
        {
            return obj is TVector3<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(x, y).GetHashCode();
        }

        #endregion Equals

        #endregion Public Methods
    }
}