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

// ReSharper disable InconsistentNaming

namespace DeusaldSharp
{
    [Flags]
    [SerializableEnum(SerializableEnumType.UInt)]
    public enum Platforms : uint
    {
        None           = 0,
        All            = UInt32.MaxValue,
        Windows        = 1 << 0,
        MacOS          = 1 << 1,
        Linux          = 1 << 2,
        iOS            = 1 << 3,
        Android        = 1 << 4,
        Xbox           = 1 << 5,
        PlayStation    = 1 << 6,
        NintendoSwitch = 1 << 7,
        Desktop        = Windows | MacOS | Linux,
        Mobile         = iOS | Android,
        Console        = Xbox | PlayStation | NintendoSwitch
    }
}