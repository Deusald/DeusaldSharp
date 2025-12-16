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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeusaldSharp
{
    public sealed class ProtoModel<T>
    {
        private readonly ProtoField<T>[]                   _Fields;
        private readonly Dictionary<ushort, ProtoField<T>> _ById;

        public ProtoModel(params ProtoField<T>[] fields)
        {
            _Fields = fields;
            _ById   = fields.ToDictionary(f => f.Id);
        }

        public void Serialize(BinaryWriter writer, ref T obj)
        {
            foreach (ProtoField<T> field in _Fields)
            {
                using MemoryStream ms = new MemoryStream();
                using BinaryWriter fw = new BinaryWriter(ms, Encoding.UTF8, false);

                field.Write(fw, ref obj);
                fw.Flush();

                byte[] payload = ms.ToArray();

                writer.Write(field.Id);
                writer.Write(payload.Length);
                writer.Write(payload);
            }
        }

        public void Deserialize(BinaryReader reader, ref T obj)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                ushort id     = reader.ReadUInt16();
                int    length = reader.ReadInt32();
                long   end    = reader.BaseStream.Position + length;

                if (_ById.TryGetValue(id, out var field))
                    field.Read(reader, ref obj);
                else
                    reader.BaseStream.Seek(length, SeekOrigin.Current);

                reader.BaseStream.Position = end;
            }
        }
    }

}