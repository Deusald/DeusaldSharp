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
    /// <summary> Representation of 3D vectors and points. </summary>
    public struct Vector3 : IEquatable<Vector3>
    {
        #region Variables

        /// <summary> X component of the vector. </summary>
        public float X;

        /// <summary> Y component of the vector. </summary>
        public float Y;

        /// <summary> Z component of the vector. </summary>
        public float Z;

        #endregion Variables

        #region Properties

        /// <summary> Shorthand for writing Vector3(0, 0, 0). </summary>
        public static Vector3 Zero { get; } = new Vector3(0f, 0f, 0f);

        /// <summary> Shorthand for writing Vector3(1, 1, 1). </summary>
        public static Vector3 One { get; } = new Vector3(1f, 1f, 1f);

        /// <summary> Shorthand for writing Vector3(1, 0, 0). </summary>
        public static Vector3 UnitX { get; } = new Vector3(1f, 0f, 0f);

        /// <summary> Shorthand for writing Vector3(0, 1, 0). </summary>
        public static Vector3 UnitY { get; } = new Vector3(0f, 1f, 0f);

        /// <summary> Shorthand for writing Vector3(0, 0, 1). </summary>
        public static Vector3 UnitZ { get; } = new Vector3(0f, 0f, 1f);

        /// <summary> Shorthand for writing Vector3(0, 1, 0). </summary>
        public static Vector3 Up { get; } = new Vector3(0f, 1f, 0f);

        /// <summary> Shorthand for writing Vector3(0, -1, 0). </summary>
        public static Vector3 Down { get; } = new Vector3(0f, -1f, 0f);

        /// <summary> Shorthand for writing Vector3(1, 0, 0). </summary>
        public static Vector3 Right { get; } = new Vector3(1f, 0f, 0f);

        /// <summary> Shorthand for writing Vector3(-1, 0, 0). </summary>
        public static Vector3 Left { get; } = new Vector3(-1f, 0f, 0f);

        /// <summary> Shorthand for writing Vector3(0, 0, -1). </summary>
        public static Vector3 Forward { get; } = new Vector3(0f, 0f, -1f);

        /// <summary> Shorthand for writing Vector3(0, 0, 1). </summary>
        public static Vector3 Backward { get; } = new Vector3(0f, 0f, 1f);

        /// <summary> Returns the length of this vector (Read Only). </summary>
        public float Magnitude => Length();

        /// <summary> Returns the squared length of this vector (Read Only). </summary>
        public float SqrMagnitude => LengthSquared();

        /// <summary> Returns this vector with a magnitude of 1 (Read Only). </summary>
        public Vector3 Normalized
        {
            get
            {
                var normal = new Vector3(X, Y, Z);
                normal.Normalize();
                return normal;
            }
        }

        /// <summary> Returns negated version of this vector (Read Only). </summary>
        public Vector3 Negated
        {
            get
            {
                var negate = new Vector3(X, Y, Z);
                negate.Negate();
                return negate;
            }
        }

        /// <summary> Checks if all vector components are not infinity (Read Only). </summary>
        public bool IsValid => !float.IsInfinity(X) && !float.IsInfinity(Y) && !float.IsInfinity(Z);

        #endregion Properties

        #region Init Methods

        /// <summary> Creates a new vector with given x, y, z components. </summary>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary> Creates a new vector with all x, y, z components set to value. </summary>
        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary> Creates a new vector where x and y components are taken from given Vector2 and z component is given. </summary>
        public Vector3(Vector2 value, float z)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
        }

        /// <summary> Creates a new vector with given x, y components from Vector2 and sets z to zero. </summary>
        public Vector3(Vector2 value)
        {
            X = value.X;
            Y = value.Y;
            Z = 0f;
        }
        
        /// <summary> Creates a new vector with given x, y components and sets z to zero. </summary>
        public Vector3(float x, float y)
        {
            X = x;
            Y = y;
            Z = 0f;
        }

        /// <summary> Creates a new vector3 based on given Vector3. </summary>
        public Vector3(Vector3 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }

        #endregion Init Methods

        #region Public Methods

        #region Setters

        /// <summary> Set x, y and z components of an existing Vector3. </summary>
        public void Set(float newX, float newY, float newZ)
        {
            X = newX;
            Y = newY;
            Z = newZ;
        }

        /// <summary> Set x, y and z components of an existing Vector3 to 0. </summary>
        public void SetZero()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        #endregion Setters

        #region Basic Math

        /// <summary> Adds two vectors component-wise. </summary>
        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary> Subtracts two vectors component-wise. </summary>
        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary> Multiplies two vectors component-wise. </summary>
        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary> Multiplies every component of this vector by the same component of scale. </summary>
        public static Vector3 Multiply(Vector3 a, float s)
        {
            return new Vector3(a.X * s, a.Y * s, a.Z * s);
        }

        /// <summary> Divides two vectors component-wise. </summary>
        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary> Divides every component of this vector by the same component of scale. </summary>
        public static Vector3 Divide(Vector3 a, float s)
        {
            var factor = 1f / s;
            return new Vector3(a.X * factor, a.Y * factor, a.Z * factor);
        }

        /// <summary> Returns negated version of this vector. </summary>
        public static Vector3 GetNegated(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }

        /// <summary> Negates this vector. </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
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
            Z *= invLength;
        }

        /// <summary> Returns given vector with a magnitude of 1. </summary>
        public static Vector3 GetNormalized(Vector3 a)
        {
            var normalized = new Vector3(a);
            normalized.Normalize();
            return normalized;
        }

        /// <summary> Cross Product of two vectors. </summary>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            var x = a.Y * b.Z - a.Z * b.Y;
            var y = a.Z * b.X - a.X * b.Z;
            var z = a.X * b.Y - a.Y * b.X;

            return new Vector3(x, y, z);
        }

        /// <summary> Cross Product of this vector and vector b. </summary>
        public Vector3 Cross(Vector3 b)
        {
            return Cross(this, b);
        }

        /// <summary> Returns the distance between a and b. </summary>
        public static float Distance(Vector3 a, Vector3 b)
        {
            return MathF.Sqrt(DistanceSquared(a, b));
        }

        /// <summary> Returns the distance between this vector and vector b. </summary>
        public float Distance(Vector3 b)
        {
            return Distance(this, b);
        }

        /// <summary> Returns the squared distance between a and b. </summary>
        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            var c = a - b;
            return Dot(c, c);
        }

        /// <summary> Returns the squared distance between this vector and vector b. </summary>
        public float DistanceSquared(Vector3 b)
        {
            return DistanceSquared(this, b);
        }

        /// <summary> Dot Product of two vectors. </summary>
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary> Dot Product of this vector and vector b. </summary>
        public float Dot(Vector3 b)
        {
            return Dot(this, b);
        }

        /// <summary> Reflects a vector off the plane defined by a normal. </summary>
        /// <param name="inDir"> The direction of a vector. </param>
        /// <param name="normal"> The normal vector that defines a plane. </param>
        public static Vector3 Reflect(Vector3 inDir, Vector3 normal)
        {
            var dot = Dot(inDir, normal);
            var result = new Vector3
            {
                X = inDir.X - 2f * dot * normal.X,
                Y = inDir.Y - 2f * dot * normal.Y,
                Z = inDir.Z - 2f * dot * normal.Z
            };

            return result;
        }

        /// <summary> Reflects this vector off the plane defined by a normal. </summary>
        /// <param name="normal"> The normal vector that defines a plane. </param>
        public Vector3 Reflect(Vector3 normal)
        {
            return Reflect(this, normal);
        }

        #endregion Vector Math

        #region Other Math

        /// <summary> Returns vector where each component is between min and max values. </summary>
        public static Vector3 Clamp(Vector3 a, Vector3 min, Vector3 max)
        {
            return new Vector3(
                MathUtils.Clamp(a.X, min.X, max.X),
                MathUtils.Clamp(a.Y, min.Y, max.Y),
                MathUtils.Clamp(a.Z, min.Z, max.Z));
        }

        /// <summary> Linearly interpolates between two points. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                MathUtils.Lerp(a.X, b.X, t),
                MathUtils.Lerp(a.Y, b.Y, t),
                MathUtils.Lerp(a.Z, b.Z, t));
        }

        /// <summary> Returns vector where each component is max value from both vectors. </summary>
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(
                MathF.Max(a.X, b.X),
                MathF.Max(a.Y, b.Y),
                MathF.Max(a.Z, b.Z));
        }

        /// <summary> Returns vector where each component is min value from both vectors. </summary>
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(
                MathF.Min(a.X, b.X),
                MathF.Min(a.Y, b.Y),
                MathF.Min(a.Z, b.Z));
        }

        #endregion Other Math

        #region Equals

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public bool Equals(Vector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public override bool Equals(object obj)
        {
            return obj is Vector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y, Z).GetHashCode();
        }

        #endregion Equals

        #region Operators

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return MathUtils.AreFloatsEquals(a.X, b.X)
                && MathUtils.AreFloatsEquals(a.Y, b.Y)
                && MathUtils.AreFloatsEquals(a.Z, b.Z);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return a.Negated;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return Add(a, b);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return Subtract(a, b);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return Multiply(a, b);
        }

        public static Vector3 operator *(Vector3 a, float s)
        {
            return Multiply(a, s);
        }

        public static Vector3 operator *(float s, Vector3 a)
        {
            return Multiply(a, s);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return Divide(a, b);
        }

        public static Vector3 operator /(Vector3 a, float s)
        {
            return Divide(a, s);
        }

        public static implicit operator Vector3(Vector2 vector2)
        {
            return new Vector3(vector2.X, vector2.Y, 0f);
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
                    case 2:
                        return Z;
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
                    case 2:
                    {
                        Z = value;
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
            var sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(X);
            sb.Append(" Y:");
            sb.Append(Y);
            sb.Append(" Z:");
            sb.Append(Z);
            sb.Append("}");
            return sb.ToString();
        }

        #endregion Public methods
    }
}