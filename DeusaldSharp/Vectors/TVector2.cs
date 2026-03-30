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

using JetBrains.Annotations;

namespace DeusaldSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary> A two-element array in the form of a vector.  </summary>
    [PublicAPI]
    public struct TVector2<T> : IEnumerable<T>, IEquatable<TVector2<T>> where T : struct
    {
        #region Variables

        /// <summary> X component of the vector. </summary>
        public T x;
        
        /// <summary> Y component of the vector. </summary>
        public T y;

        #endregion Variables

        #region Init Methods

        /// <summary> Creates a new vector with given x, y components. </summary>
        public TVector2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary> Creates a new vector with all x, y components set to value. </summary>
        public TVector2(T value)
        {
            x = value;
            y = value;
        }

        /// <summary> Creates a new vector with given x, y components from Vector2. </summary>
        public TVector2(TVector2<T> value)
        {
            x = value.x;
            y = value.y;
        }

        /// <summary> Creates a new vector with given x, y components from Vector3 ignoring z component. </summary>
        public TVector2(TVector3<T> value)
        {
            x = value.x;
            y = value.y;
        }

        #endregion Init Methods
        
        #region Public Methods

        #region Enumerable

        public IEnumerator<T> GetEnumerator()
        {
            yield return x;
            yield return y;
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
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion Operators

        #region Equals

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public bool Equals(TVector2<T> other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public override bool Equals(object? obj)
        {
            return obj is TVector2<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(x, y).GetHashCode();
        }

        #endregion Equals
        
        #endregion Public Methods
    }
}