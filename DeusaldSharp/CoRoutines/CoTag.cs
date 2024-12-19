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

namespace DeusaldSharp
{
    /// <summary> CoTag is a tag to mark group of logically connected CoRoutines.
    /// The CoTag can be later used to pause or kill all CoRoutines marked with specific tag.
    /// WARNING: CoTag 0 is reserved for default CoTag. </summary>
    public readonly struct CoTag : IEquatable<CoTag>
    {
        private readonly uint _Tag;

        public CoTag(uint newTag)
        {
            _Tag = newTag;
        }

        public static implicit operator uint(CoTag i)
        {
            return i._Tag;
        }

        public bool Equals(CoTag other)
        {
            return _Tag == other._Tag;
        }

        public override bool Equals(object obj)
        {
            return obj is CoTag other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)_Tag;
        }

        public static bool operator ==(CoTag left, CoTag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoTag left, CoTag right)
        {
            return !left.Equals(right);
        }
    }
}