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

using System.IO;

namespace DeusaldSharp
{
    public abstract class ProtoMsg<TSelf>
        where TSelf : ProtoMsg<TSelf>, new()
    {
        protected static ProtoModel<TSelf> _model = null!;

        public byte[] Serialize()
        {
            return Helpers.SerializeByBinaryWriter(w =>
            {
                TSelf self = (TSelf)this;
                _model.Serialize(w, ref self);
            });
        }
        
        public void Serialize(BinaryWriter w)
        {
            TSelf self = (TSelf)this;
            _model.Serialize(w, ref self);
        }

        public static TSelf Deserialize(byte[] data)
        {
            TSelf msg = new TSelf();
            
            Helpers.DeserializeFromBinaryReader(data, r =>
            {
                _model.Deserialize(r, ref msg);
            });
            
            return msg;
        }
        
        public static TSelf Deserialize(BinaryReader r)
        {
            TSelf msg = new TSelf();
            _model.Deserialize(r, ref msg);
            return msg;
        }
    }
}