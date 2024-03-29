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

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System;
using System.Collections.Generic;

namespace DeusaldSharp
{
    /// <summary> This class represents the 2D spline. </summary>
    public class Spline2D
    {
        #region Types

        private readonly struct DistanceToT
        {
            public float Distance { get; }
            public float T        { get; }
            public int   Index    { get; }

            public DistanceToT(float distance, float t, int index)
            {
                Distance = distance;
                T        = t;
                Index    = index;
            }
        }

        private readonly struct Segment
        {
            public int   SegIndex { get; }
            public float Rest     { get; }

            public Segment(int segIndex, float rest)
            {
                SegIndex = segIndex;
                Rest     = rest;
            }
        }

        #endregion Types

        #region Variables

        private          bool              _Closed;
        private          float             _Curvature;
        private          int               _LengthSamplesPerSegment;
        private          bool              _TangentsDirty;
        private          bool              _LengthSamplesDirty;
        private          DistanceToT       _PreviousSearchedDistance;
        private readonly List<Vector2>     _Points;
        private readonly List<Vector2>     _Tangents;
        private readonly List<DistanceToT> _Distances;

        #endregion Variables

        #region Properties

        /// <summary> Is the spline closed. In closed spline the first point is also the last. </summary>
        public bool IsClosed
        {
            get => _Closed;
            set
            {
                _Closed             = value;
                _TangentsDirty      = true;
                _LengthSamplesDirty = true;
            }
        }

        /// <summary> The amount of the curvature in the spline. </summary>
        public float Curvature
        {
            get => _Curvature;
            set
            {
                _Curvature          = value;
                _TangentsDirty      = true;
                _LengthSamplesDirty = true;
            }
        }

        /// <summary> The accuracy of sampling curve in calculating its length. </summary>
        public int LengthSamplesPerSegment
        {
            get => _LengthSamplesPerSegment;
            set
            {
                _LengthSamplesPerSegment = value;
                _LengthSamplesDirty      = true;
            }
        }

        /// <summary> Number of control points. </summary>
        public int Count => _Points.Count;

        /// <summary> The approximate length of the curve. </summary>
        public float Length
        {
            get
            {
                RecalculateTangents();
                RecalculateLength();

                int count = _Distances.Count;

                if (count == 0)
                    return 0f;

                return _Distances[count - 1].Distance;
            }
        }

        #endregion Properties

        #region Init Methods

        public Spline2D()
        {
            _Closed                  = false;
            _Curvature               = 0.5f;
            _LengthSamplesPerSegment = 5;
            _TangentsDirty           = true;
            _LengthSamplesDirty      = true;
            _Points                  = new List<Vector2>();
            _Tangents                = new List<Vector2>();
            _Distances               = new List<DistanceToT>();
        }

        public Spline2D(List<Vector2> controlPoints, bool closed = false, float curvature = 0.5f, int lengthSamplesPerSegment = 5)
        {
            _Closed                  = closed;
            _Curvature               = curvature;
            _LengthSamplesPerSegment = lengthSamplesPerSegment;
            _TangentsDirty           = true;
            _LengthSamplesDirty      = true;
            _Points                  = new List<Vector2>(controlPoints);
            _Tangents                = new List<Vector2>();
            _Distances               = new List<DistanceToT>();
        }

        #endregion Init Methods

        #region Public Methods

        #region Points Management

        /// <summary> Add single point at the end of spline. </summary>
        public void AddPoint(Vector2 point)
        {
            _Points.Add(point);
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Add single point at the end of spline and remove the first point of spline. </summary>
        public void AddPointScroll(Vector2 point)
        {
            if (_Closed)
                throw new Exception("Can't use AddPointScroll when the spline is closed!");

            _TangentsDirty      = true;
            _LengthSamplesDirty = true;

            if (Count == 0)
            {
                AddPoint(point);
                return;
            }

            _Points.RemoveAt(0);
            AddPoint(point);
        }

        /// <summary> Add range of the points at the end of spline. </summary>
        public void AddPoints(List<Vector2> points)
        {
            _Points.AddRange(points);
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Replace range of points starting from index. </summary>
        public void ReplacePoints(List<Vector2> points, int fromIndex = 0)
        {
            _Points.RemoveRange(fromIndex, Count - fromIndex);
            _Points.AddRange(points);
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Set point at index to given value. </summary>
        public void SetPoint(int index, Vector2 point)
        {
            _Points[index]      = point;
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Remove point at index. </summary>
        public void RemovePoint(int index)
        {
            _Points.RemoveAt(index);
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Insert point at index. </summary>
        public void InsertPoint(int index, Vector2 point)
        {
            _Points.Insert(index, point);
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Clear all spline points. </summary>
        public void Clear()
        {
            _Points.Clear();
            _TangentsDirty      = true;
            _LengthSamplesDirty = true;
        }

        /// <summary> Get point at index. </summary>
        public Vector2 GetPoint(int index)
        {
            return _Points[index];
        }

        #endregion Points Management

        #region Interpolate

        /// <summary> Interpolate position on given percent on the entire curve. </summary>
        /// <param name="t"> Percent of the curve. Should be from 0.0f to 1.0f. </param>
        public Vector2 Interpolate(float t)
        {
            RecalculateTangents();
            Segment segment = GetSegment(t);
            return Interpolate(segment.SegIndex, segment.Rest);
        }

        /// <summary> Interpolate position based on given distance. </summary>
        /// <param name="distance"> The distance on which we want to get the position. </param>
        public Vector2 InterpolateDistance(float distance)
        {
            return Interpolate(DistanceToLinearT(distance));
        }

        /// <summary> Interpolate position on the curve. The interpolated position is between "fromIndex" and the next one.
        /// The "t" represents percentage between these two points. </summary>
        /// <param name="fromIndex"> First index of interpolated position. The next one is next to this one.</param>
        /// <param name="t"> Percentage between "fromIndex" and its successor. Should be from 0.0f to 1.0f. </param>
        public Vector2 Interpolate(int fromIndex, float t)
        {
            RecalculateTangents();

            int toIndex = fromIndex + 1;
            int count   = Count;

            if (toIndex >= Count)
            {
                if (_Closed)
                {
                    fromIndex = fromIndex % count;
                    toIndex   = toIndex % count;
                }
                else
                    return _Points[count - 1];
            }

            if (t.AreFloatsEquals(0f))
                return _Points[fromIndex];

            if (t.AreFloatsEquals(1f))
                return _Points[toIndex];

            // Pre-calculate powers
            float t2 = t * t;
            float t3 = t2 * t;

            // Calculate hermite basis parts
            float h1 = 2f * t3 - 3f * t2 + 1f;
            float h2 = -2f * t3 + 3f * t2;
            float h3 = t3 - 2f * t2 + t;
            float h4 = t3 - t2;

            return h1 * _Points[fromIndex] +
                   h2 * _Points[toIndex] +
                   h3 * _Tangents[fromIndex] +
                   h4 * _Tangents[toIndex];
        }

        #endregion Interpolate

        #region Derivative

        /// <summary> Get derivative at position on given percent on the entire curve. </summary>
        /// <param name="t"> Percent of the curve. Should be from 0.0f to 1.0f. </param>
        public Vector2 Derivative(float t)
        {
            RecalculateTangents();
            Segment segment = GetSegment(t);
            return Derivative(segment.SegIndex, segment.Rest);
        }

        /// <summary> Get derivative at position based on given distance. </summary>
        /// <param name="distance"> The distance on which we want to get the derivative. </param>
        public Vector2 DerivativeDistance(float distance)
        {
            return Derivative(DistanceToLinearT(distance));
        }

        /// <summary> Get derivative at position on the curve. The point of derivative is between "fromIndex" and the next one.
        /// The "t" represents percentage between these two points. </summary>
        /// <param name="fromIndex"> First index of position of derivative. The next one is next to this one. </param>
        /// <param name="t"> Percentage between "fromIndex" and its successor. Should be from 0.0f to 1.0f. </param>
        public Vector2 Derivative(int fromIndex, float t)
        {
            RecalculateTangents();

            int toIndex = fromIndex + 1;
            int count   = Count;

            if (toIndex >= Count)
            {
                if (_Closed)
                {
                    fromIndex = fromIndex % count;
                    toIndex   = toIndex % count;
                }
                else
                    toIndex = fromIndex;
            }

            // Pre-calculate power
            float t2 = t * t;

            // Derivative of hermite basis parts
            float h1 = 6f * t2 - 6f * t;
            float h2 = -6f * t2 + 6f * t;
            float h3 = 3f * t2 - 4f * t + 1;
            float h4 = 3f * t2 - 2f * t;

            return h1 * _Points[fromIndex] +
                   h2 * _Points[toIndex] +
                   h3 * _Tangents[fromIndex] +
                   h4 * _Tangents[toIndex];
        }

        #endregion Derivative

        #region Distance

        /// <summary> Approximate the distance of the curve at the given index. </summary>
        /// <param name="index"> Index point for distance approximation. </param>
        public float DistanceAtPoint(int index)
        {
            if (index == 0)
                return 0f;

            RecalculateTangents();
            RecalculateLength();
            return _Distances[index * _LengthSamplesPerSegment - 1].Distance;
        }

        /// <summary> Convert the distance to a linear [0f, 1f] "t" position on the curve.
        /// Because distance is approximate the "t" value will also be approximated. </summary>
        /// <param name="distance"> The distance on which we want to get linear "t". </param>
        public float DistanceToLinearT(float distance)
        {
            RecalculateTangents();
            RecalculateLength();

            if (_Distances.Count == 0)
                return 0f;

            float length = Length;

            if (distance >= length)
            {
                if (_Closed)
                    distance = distance % length;
                else
                    return 1f;
            }

            if (_PreviousSearchedDistance.Distance < distance ||
                (_PreviousSearchedDistance.Index != 0 && _Distances[_PreviousSearchedDistance.Index - 1].Distance > distance))
                _PreviousSearchedDistance = new DistanceToT(0f, 0f, 0);

            for (int i = _PreviousSearchedDistance.Index; i < _Distances.Count; ++i)
            {
                DistanceToT distToT = _Distances[i];

                if (distance < distToT.Distance)
                {
                    float distanceT = MathUtils.InverseLerp(_PreviousSearchedDistance.Distance, distToT.Distance, distance);
                    return MathUtils.Lerp(_PreviousSearchedDistance.T, distToT.T, distanceT);
                }

                _PreviousSearchedDistance = distToT;
            }

            return 1.0f;
        }

        #endregion Distance

        #region Operators

        public Vector2 this[int key]
        {
            get => GetPoint(key);
            set => SetPoint(key, value);
        }

        #endregion Operators

        #endregion Public Methods

        #region Private Methods

        private Segment GetSegment(float t)
        {
            int   count        = _Closed ? Count : Count - 1;
            float segmentIndex = t * count;
            int   index        = (int) segmentIndex;
            return new Segment(index, segmentIndex - index);
        }

        private void RecalculateTangents()
        {
            if (!_TangentsDirty)
                return;

            int count     = Count;
            int lastIndex = count - 1;

            if (count < 2)
                return;

            _Tangents.Clear();
            _Tangents.Capacity = count;

            for (int i = 0; i < count; ++i)
            {
                Vector2 tangent;

                if (i == 0)
                    tangent = CalculateTangent(_Closed ? _Points[lastIndex] : _Points[0], _Points[1]);
                else if (i == lastIndex)
                    tangent = CalculateTangent(_Points[lastIndex - 1], _Closed ? _Points[0] : _Points[lastIndex]);
                else
                    tangent = CalculateTangent(_Points[i - 1], _Points[i + 1]);

                _Tangents.Add(tangent);
            }

            _TangentsDirty = false;
        }

        private Vector2 CalculateTangent(Vector2 a, Vector2 b)
        {
            return _Curvature * (b - a);
        }

        private void RecalculateLength()
        {
            if (!_LengthSamplesDirty)
                return;

            int count = Count;

            if (count < 2)
                return;

            int numberOfSamples = _LengthSamplesPerSegment * (_Closed ? count : count - 1);

            _Distances.Clear();
            _Distances.Capacity = numberOfSamples;

            float   distanceSum  = 0f;
            float   step         = 1f / numberOfSamples;
            float   current      = step;
            Vector2 lastPosition = _Points[0];

            for (int i = 1; i <= numberOfSamples; ++i)
            {
                Vector2 nextPosition = Interpolate(current);
                distanceSum += Vector2.Distance(lastPosition, nextPosition);
                _Distances.Add(new DistanceToT(distanceSum, current, i));
                lastPosition =  nextPosition;
                current      += step;
            }

            _LengthSamplesDirty = false;
        }

        #endregion Private Methods
    }
}