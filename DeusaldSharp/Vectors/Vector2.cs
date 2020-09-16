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

// ReSharper disable NonReadonlyMemberInGetHashCode
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Text;

namespace DeusaldSharp
{
    /// <summary> Representation of 2D vectors and points. </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        #region Variables

        /// <summary> X component of the vector. </summary>
        public float X;
        
        /// <summary> Y component of the vector. </summary>
        public float Y;

        #endregion Variables

        #region Properties

        /// <summary> Shorthand for writing Vector2(0, 0). </summary>
        public static Vector2 Zero { get; } = new Vector2(0f, 0f);

        /// <summary> Shorthand for writing Vector2(1, 1). </summary>
        public static Vector2 One { get; } = new Vector2(1f, 1f);

        /// <summary> Shorthand for writing Vector2(0, 1). </summary>
        public static Vector2 Up { get; } = new Vector2(0f, 1f);

        /// <summary> Shorthand for writing Vector2(0, -1). </summary>
        public static Vector2 Down { get; } = new Vector2(0f, -1f);

        /// <summary> Shorthand for writing Vector2(-1, 0). </summary>
        public static Vector2 Left { get; } = new Vector2(-1f, 0f);

        /// <summary> Shorthand for writing Vector2(1, 0). </summary>
        public static Vector2 Right { get; } = new Vector2(1f, 0f);

        /// <summary> Returns the length of this vector (Read Only). </summary>
        public float Magnitude    => Length();
        
        /// <summary> Returns the squared length of this vector (Read Only). </summary>
        public float SqrMagnitude => LengthSquared();

        /// <summary> Returns this vector with a magnitude of 1 (Read Only). </summary>
        public Vector2 Normalized
        {
            get
            {
                var normal = new Vector2(X, Y);
                normal.Normalize();
                return normal;
            }
        }

        /// <summary> Returns negated version of this vector (Read Only). </summary>
        public Vector2 Negated
        {
            get
            {
                var negate = new Vector2(X, Y);
                negate.Negate();
                return negate;
            }
        }

        /// <summary> Checks if all vector components are not infinity (Read Only). </summary>
        public bool IsValid => !float.IsInfinity(X) && !float.IsInfinity(Y);

        /// <summary> Get the skew vector such that dot(skew_vec, other) == cross(vec, other)
        /// https://brilliant.org/wiki/3d-coordinate-geometry-skew-lines/ </summary>
        public Vector2 Skew => new Vector2(-Y, X);

        #endregion Properties

        #region Init Methods

        /// <summary> Creates a new vector with given x, y components. </summary>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary> Creates a new vector with all x, y components set to value. </summary>
        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        /// <summary> Creates a new vector with given x, y components from Vector2. </summary>
        public Vector2(Vector2 value)
        {
            X = value.X;
            Y = value.Y;
        }

        /// <summary> Creates a new vector with given x, y components from Vector3 ignoring z component. </summary>
        public Vector2(Vector3 value)
        {
            X = value.X;
            Y = value.Y;
        }

        #endregion Init Methods

        #region Public Methods

        #region Setters

        /// <summary> Set x and y components of an existing Vector2. </summary>
        public void Set(float newX, float newY)
        {
            X = newX;
            Y = newY;
        }

        /// <summary> Set x and y components of an existing Vector2 to 0. </summary>
        public void SetZero()
        {
            X = 0f;
            Y = 0f;
        }

        #endregion Setters

        #region Basic Math

        /// <summary> Adds two vectors component-wise. </summary>
        public static Vector2 Add(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        /// <summary> Subtracts two vectors component-wise. </summary>
        public static Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        /// <summary> Multiplies two vectors component-wise. </summary>
        public static Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }

        /// <summary> Multiplies every component of this vector by the same component of scale. </summary>
        public static Vector2 Multiply(Vector2 a, float s)
        {
            return new Vector2(a.X * s, a.Y * s);
        }

        /// <summary> Divides two vectors component-wise. </summary>
        public static Vector2 Divide(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X / b.X, a.Y / b.Y);
        }

        /// <summary> Divides every component of this vector by the same component of scale. </summary>
        public static Vector2 Divide(Vector2 a, float s)
        {
            var factor = 1f / s;
            return new Vector2(a.X * factor, a.Y * factor);
        }

        /// <summary> Returns negated version of this vector. </summary>
        public static Vector2 GetNegated(Vector2 a)
        {
            return new Vector2(-a.X, -a.Y);
        }

        /// <summary> Negates this vector. </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
        }

        #endregion Basic Math

        #region Vector Math

        /// <summary> Returns the length of this vector. </summary>
        public float Length()
        {
            return MathF.Sqrt(DistanceSquared(this, Zero));
        }

        /// <summary> Returns the squared length of this vector. </summary>
        public float LengthSquared()
        {
            return DistanceSquared(this, Zero);
        }

        /// <summary> Makes this vector have a magnitude of 1. </summary>
        public void Normalize()
        {
            var length = Length();

            if (MathUtils.IsFloatZero(length)) return;

            var invLength = 1f / length;
            X *= invLength;
            Y *= invLength;
        }

        /// <summary> Returns given vector with a magnitude of 1. </summary>
        public static Vector2 GetNormalized(Vector2 a)
        {
            var normalized = new Vector2(a.X, a.Y);
            normalized.Normalize();
            return normalized;
        }

        /// <summary> Cross Product of two vectors. </summary>
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        /// <summary> Cross Product of this vector and vector b. </summary>
        public float Cross(Vector2 b)
        {
            return Cross(this, b);
        }

        /// <summary> Cross Product of Vector and scalar. </summary>
        public static Vector2 Cross(Vector2 a, float s)
        {
            return new Vector2(s * a.Y, -s * a.X);
        }

        /// <summary> Cross Product of scalar and Vector. </summary>
        public static Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.Y, s * a.X);
        }

        /// <summary> Returns the distance between a and b. </summary>
        public static float Distance(Vector2 a, Vector2 b)
        {
            return MathF.Sqrt(DistanceSquared(a, b));
        }

        /// <summary> Returns the distance between this vector and vector b. </summary>
        public float Distance(Vector2 b)
        {
            return Distance(this, b);
        }

        /// <summary> Returns the squared distance between a and b. </summary>
        public static float DistanceSquared(Vector2 a, Vector2 b)
        {
            var c = a - b;
            return Dot(c, c);
        }

        /// <summary> Returns the squared distance between this vector and vector b. </summary>
        public float DistanceSquared(Vector2 b)
        {
            return DistanceSquared(this, b);
        }

        /// <summary> Dot Product of two vectors. </summary>
        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary> Dot Product of this vector and vector b. </summary>
        public float Dot(Vector2 b)
        {
            return Dot(this, b);
        }

        /// <summary> Reflects a vector off the vector defined by a normal. </summary>
        public static Vector2 Reflect(Vector2 inDir, Vector2 normal)
        {
            var dot = Dot(inDir, normal);
            var result = new Vector2
            {
                X = inDir.X - 2f * dot * normal.X,
                Y = inDir.Y - 2f * dot * normal.Y
            };

            return result;
        }

        /// <summary> Reflects a vector off the vector defined by a normal. </summary>
        public Vector3 Reflect(Vector2 normal)
        {
            return Reflect(this, normal);
        }

        #endregion Vector Math

        #region Other Math

        /// <summary> Returns vector where each component is between min and max values. </summary>
        public static Vector2 Clamp(Vector2 a, Vector2 min, Vector2 max)
        {
            return new Vector2(
                MathUtils.Clamp(a.X, min.X, max.X),
                MathUtils.Clamp(a.Y, min.Y, max.Y));
        }

        /// <summary> Linearly interpolates between two points. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(
                MathUtils.Lerp(a.X, b.X, t),
                MathUtils.Lerp(a.Y, b.Y, t));
        }

        /// <summary> Returns vector where each component is max value from both vectors. </summary>
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(
                MathF.Max(a.X, b.X),
                MathF.Max(a.Y, b.Y));
        }

        /// <summary> Returns vector where each component is min value from both vectors. </summary>
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(
                MathF.Min(a.X, b.X),
                MathF.Min(a.Y, b.Y));
        }

        /// <summary> Returns vector where each component is an absolute value. </summary>
        public static Vector2 Abs(Vector2 a)
        {
            return new Vector2(MathF.Abs(a.X), MathF.Abs(a.Y));
        }

        /// <summary> Rounds the vector component to given decimal point. </summary>
        public void RoundToDecimal(int decimalPoint)
        {
            var decimalPow = MathF.Pow(10f, decimalPoint);
            this =  this * decimalPow;
            X    =  MathF.Round(X);
            Y    =  MathF.Round(Y);
            this /= decimalPow;
        }

        /// <summary> Rotate a vector by angle. </summary>
        /// <param name="angle"> The angle in radians. </param>
        /// <param name="v"> Vector to rotate. </param>
        public static Vector2 Rotate(float angle, Vector2 v)
        {
            var sin = MathF.Sin(angle);
            var cos = MathF.Cos(angle);
            return new Vector2(cos * v.X - sin * v.Y, sin * v.X + cos * v.Y);
        }
        
        /// <summary> Inverse rotate a vector by angle. </summary>
        /// <param name="angle"> The angle in radians. </param>
        /// <param name="v"> Vector to rotate. </param>
        public static Vector2 RotateInverse(float angle, Vector2 v)
        {
            var sin = MathF.Sin(angle);
            var cos = MathF.Cos(angle);
            return new Vector2(cos * v.X + sin * v.Y, -sin * v.X + cos * v.Y);
        }

        /// <summary> Reduce length of the vector by reductionLength value. </summary>
        public void ShortenLength(float reductionLength)
        {
            if (Magnitude <= reductionLength)
            {
                SetZero();
                return;
            }

            var shorter = Normalized * reductionLength;
            this -= shorter;
        }

        /// <summary> Lerp between vectors using max distance delta. </summary>
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            var vector2   = target - current;
            var magnitude = vector2.Magnitude;
            if (magnitude <= (double) maxDistanceDelta || magnitude < double.Epsilon)
                return target;

            return current + vector2 / magnitude * maxDistanceDelta;
        }

        #endregion Other Math

        #region Equals

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public bool Equals(Vector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public override bool Equals(object obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }

        #endregion Equals

        #region Operators

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return MathUtils.AreFloatsEquals(a.X, b.X) && MathUtils.AreFloatsEquals(a.Y, b.Y);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return a.Negated;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return Add(a, b);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return Subtract(a, b);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return Multiply(a, b);
        }

        public static Vector2 operator *(Vector2 a, float s)
        {
            return Multiply(a, s);
        }

        public static Vector2 operator *(float s, Vector2 a)
        {
            return Multiply(a, s);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return Divide(a, b);
        }

        public static Vector2 operator /(Vector2 a, float s)
        {
            return Divide(a, s);
        }

        public static implicit operator Vector2(Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }

        public float this[int key]
        {
            get
            {
                switch (key)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
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
                        X = value;
                        break;
                    }
                    case 1:
                    {
                        Y = value;
                        break;
                    }
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion Operators

        public override string ToString()
        {
            var sb = new StringBuilder(24);
            sb.Append("{X:");
            sb.Append(X);
            sb.Append(" Y:");
            sb.Append(Y);
            sb.Append("}");
            return sb.ToString();
        }

        #endregion Public Methods
    }
}