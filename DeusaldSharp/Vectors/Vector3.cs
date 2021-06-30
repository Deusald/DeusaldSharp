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

using System;
using System.Text;

namespace DeusaldSharp
{
    /// <summary> Representation of 3D vectors and points. </summary>
    public struct Vector3 : IEquatable<Vector3>
    {
        #region Variables

        /// <summary> X component of the vector. </summary>
        public float x;

        /// <summary> Y component of the vector. </summary>
        public float y;

        /// <summary> Z component of the vector. </summary>
        public float z;

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
                var normal = new Vector3(x, y, z);
                normal.Normalize();
                return normal;
            }
        }

        /// <summary> Returns negated version of this vector (Read Only). </summary>
        public Vector3 Negated
        {
            get
            {
                var negate = new Vector3(x, y, z);
                negate.Negate();
                return negate;
            }
        }

        /// <summary> Checks if all vector components are not infinity (Read Only). </summary>
        public bool IsValid => !float.IsInfinity(x) && !float.IsInfinity(y) && !float.IsInfinity(z);

        #endregion Properties

        #region Init Methods

        /// <summary> Creates a new vector with given x, y, z components. </summary>
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary> Creates a new vector with all x, y, z components set to value. </summary>
        public Vector3(float value)
        {
            x = value;
            y = value;
            z = value;
        }

        /// <summary> Creates a new vector where x and y components are taken from given Vector2 and z component is given. </summary>
        public Vector3(Vector2 value, float z)
        {
            x      = value.x;
            y      = value.y;
            this.z = z;
        }

        /// <summary> Creates a new vector with given x, y components from Vector2 and sets z to zero. </summary>
        public Vector3(Vector2 value)
        {
            x = value.x;
            y = value.y;
            z = 0f;
        }
        
        /// <summary> Creates a new vector with given x, y components and sets z to zero. </summary>
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z      = 0f;
        }

        /// <summary> Creates a new vector3 based on given Vector3. </summary>
        public Vector3(Vector3 value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        #endregion Init Methods

        #region Public Methods

        #region Setters

        /// <summary> Set x, y and z components of an existing Vector3. </summary>
        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        /// <summary> Set x, y and z components of an existing Vector3 to 0. </summary>
        public void SetZero()
        {
            x = 0f;
            y = 0f;
            z = 0f;
        }

        #endregion Setters

        #region Basic Math

        /// <summary> Adds two vectors component-wise. </summary>
        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary> Subtracts two vectors component-wise. </summary>
        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        /// <summary> Multiplies two vectors component-wise. </summary>
        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary> Multiplies every component of this vector by the same component of scale. </summary>
        public static Vector3 Multiply(Vector3 a, float s)
        {
            return new Vector3(a.x * s, a.y * s, a.z * s);
        }

        /// <summary> Divides two vectors component-wise. </summary>
        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        /// <summary> Divides every component of this vector by the same component of scale. </summary>
        public static Vector3 Divide(Vector3 a, float s)
        {
            var factor = 1f / s;
            return new Vector3(a.x * factor, a.y * factor, a.z * factor);
        }

        /// <summary> Returns negated version of this vector. </summary>
        public static Vector3 GetNegated(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        /// <summary> Negates this vector. </summary>
        public void Negate()
        {
            x = -x;
            y = -y;
            z = -z;
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
            x *= invLength;
            y *= invLength;
            z *= invLength;
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
            var x = a.y * b.z - a.z * b.y;
            var y = a.z * b.x - a.x * b.z;
            var z = a.x * b.y - a.y * b.x;

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
            return a.x * b.x + a.y * b.y + a.z * b.z;
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
                x = inDir.x - 2f * dot * normal.x,
                y = inDir.y - 2f * dot * normal.y,
                z = inDir.z - 2f * dot * normal.z
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
                MathUtils.Clamp(a.x, min.x, max.x),
                MathUtils.Clamp(a.y, min.y, max.y),
                MathUtils.Clamp(a.z, min.z, max.z));
        }

        /// <summary> Linearly interpolates between two points. </summary>
        /// <param name="a"> Start value, returned when t = 0. </param>
        /// <param name="b"> End value, returned when t = 1. </param>
        /// <param name="t"> Value used to interpolate between a and b. Should be between 0 and 1. </param>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                MathUtils.Lerp(a.x, b.x, t),
                MathUtils.Lerp(a.y, b.y, t),
                MathUtils.Lerp(a.z, b.z, t));
        }

        /// <summary> Returns vector where each component is max value from both vectors. </summary>
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(
                MathF.Max(a.x, b.x),
                MathF.Max(a.y, b.y),
                MathF.Max(a.z, b.z));
        }

        /// <summary> Returns vector where each component is min value from both vectors. </summary>
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(
                MathF.Min(a.x, b.x),
                MathF.Min(a.y, b.y),
                MathF.Min(a.z, b.z));
        }

        #endregion Other Math

        #region Equals

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public bool Equals(Vector3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        /// <summary> Returns true if the given vector is exactly equal to this vector. </summary>
        public override bool Equals(object obj)
        {
            return obj is Vector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(x, y, z).GetHashCode();
        }

        #endregion Equals

        #region Operators

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return MathUtils.AreFloatsEquals(a.x, b.x)
                && MathUtils.AreFloatsEquals(a.y, b.y)
                && MathUtils.AreFloatsEquals(a.z, b.z);
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
            return new Vector3(vector2.x, vector2.y, 0f);
        }

        public float this[int key]
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

        public override string ToString()
        {
            var sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(x);
            sb.Append(" Y:");
            sb.Append(y);
            sb.Append(" Z:");
            sb.Append(z);
            sb.Append("}");
            return sb.ToString();
        }

        #endregion Public methods
    }
}