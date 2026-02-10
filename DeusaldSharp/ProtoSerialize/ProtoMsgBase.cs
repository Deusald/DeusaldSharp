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
using System.IO;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    /// <summary>
    /// Base type for all proto messages.
    /// Generated code must override Serialize(BinaryWriter) and Deserialize(BinaryReader).
    /// </summary>
    [PublicAPI]
    public abstract class ProtoMsgBase
    {
        // These are the "must override" entry points.
        public virtual void Serialize(BinaryWriter w)
            => throw new NotImplementedException("Serialize(BinaryWriter) was not generated for this message type.");

        public virtual void Deserialize(BinaryReader r)
            => throw new NotImplementedException("Deserialize(BinaryReader) was not generated for this message type.");
        
        public byte[] Serialize()
        {
            return Helpers.SerializeByBinaryWriter(Serialize);
        }
        
        public void Deserialize(byte[] data)
        {
            Helpers.DeserializeFromBinaryReader(data, Deserialize);
        }
        
        public static T Deserialize<T>(byte[] data)
            where T : ProtoMsgBase, new()
        {
            T msg = new T();
            Helpers.DeserializeFromBinaryReader(data, r => msg.Deserialize(r));
            return msg;
        }
    }
}