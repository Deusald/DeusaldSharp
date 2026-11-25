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
    /// The DeusaldSharp.Analyzers source generator overrides <see cref="Serialize(BinaryWriter)"/>,
    /// <see cref="DeserializeProtoFields"/> and <see cref="ResetProtoFields"/> for every partial message type.
    /// </summary>
    [PublicAPI]
    public abstract class ProtoMsgBase
    {
        /// <summary>Default value of <see cref="MaxDepth"/>.</summary>
        public const int DEFAULT_MAX_DEPTH = 64;

        private const int _FIELD_HEADER_SIZE  = sizeof(ushort) + sizeof(int);
        private const int _LENGTH_PREFIX_SIZE = sizeof(int);

        private static int _MaxDepth = DEFAULT_MAX_DEPTH;

        [ThreadStatic] private static int _CurrentDepth;

        /// <summary>
        /// Maximum number of nested message levels (the root message counts as level 1) accepted while deserializing.
        /// Deeper input is rejected with <see cref="InvalidDataException"/> instead of exhausting the stack.
        /// </summary>
        public static int MaxDepth
        {
            get => _MaxDepth;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), value, "MaxDepth must be at least 1.");
                _MaxDepth = value;
            }
        }

        public virtual void Serialize(BinaryWriter w)
            => throw new NotImplementedException("Serialize(BinaryWriter) was not generated for this message type.");

        /// <summary>
        /// Resets all proto fields to the values of a freshly constructed instance and then reads fields
        /// from the current position of the reader until the end of its stream.
        /// </summary>
        public virtual void Deserialize(BinaryReader r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            ResetProtoFields();
            DeserializeNested(this, r, r.BaseStream.Length);
        }

        public byte[] Serialize()
        {
            return Helpers.SerializeByBinaryWriter(Serialize);
        }

        /// <summary>
        /// Deserializes <paramref name="data"/> into this instance. Malformed data throws <see cref="InvalidDataException"/>.
        /// </summary>
        public void Deserialize(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            try
            {
                Helpers.DeserializeFromBinaryReader(data, Deserialize);
            }
            catch (Exception exception) when (_IsDecodingFailure(exception))
            {
                throw _WrapDecodingFailure(exception);
            }
        }

        /// <summary>
        /// Creates a new <typeparamref name="T"/> and deserializes <paramref name="data"/> into it.
        /// Malformed data throws <see cref="InvalidDataException"/>.
        /// </summary>
        public static T Deserialize<T>(byte[] data)
            where T : ProtoMsgBase, new()
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            T msg = new T();

            try
            {
                Helpers.DeserializeFromBinaryReader(data, r => DeserializeNested(msg, r, r.BaseStream.Length));
            }
            catch (Exception exception) when (_IsDecodingFailure(exception))
            {
                throw _WrapDecodingFailure(exception);
            }

            return msg;
        }

        #region Generated code contract

        /// <summary>Reads fields from the reader's current position up to <paramref name="end"/> (exclusive).</summary>
        protected virtual void DeserializeProtoFields(BinaryReader reader, long end)
            => throw new NotImplementedException("DeserializeProtoFields(BinaryReader, long) was not generated for this message type.");

        /// <summary>Resets all proto fields to the values of a freshly constructed instance.</summary>
        protected virtual void ResetProtoFields() { }

        /// <summary>Deserializes a message that occupies the bytes from the reader's current position up to <paramref name="end"/>.</summary>
        protected static void DeserializeNested(ProtoMsgBase message, BinaryReader reader, long end)
        {
            if (_CurrentDepth >= _MaxDepth)
                throw new InvalidDataException($"Proto message nesting exceeds the maximum depth of {_MaxDepth}.");

            ++_CurrentDepth;

            try
            {
                message.DeserializeProtoFields(reader, end);
            }
            finally
            {
                --_CurrentDepth;
            }
        }

        /// <summary>Writes the field header with a length placeholder and returns the payload start position.</summary>
        protected static long BeginField(BinaryWriter writer, ushort id)
        {
            writer.Write(id);
            return BeginLengthPrefixed(writer);
        }

        /// <summary>Writes a length placeholder and returns the payload start position.</summary>
        protected static long BeginLengthPrefixed(BinaryWriter writer)
        {
            writer.Write(0);
            return writer.BaseStream.Position;
        }

        /// <summary>Patches the length placeholder written by <see cref="BeginLengthPrefixed"/> or <see cref="BeginField"/>.</summary>
        protected static void EndLengthPrefixed(BinaryWriter writer, long start)
        {
            Stream stream = writer.BaseStream;
            long   end    = stream.Position;
            int    length = checked((int)(end - start));

            stream.Position = start - _LENGTH_PREFIX_SIZE;
            writer.Write(length);
            stream.Position = end;
        }

        /// <summary>Reads a field header and returns the position where the field payload ends.</summary>
        protected static long ReadFieldHeader(BinaryReader reader, long end, out ushort id)
        {
            Stream stream = reader.BaseStream;

            if (end - stream.Position < _FIELD_HEADER_SIZE)
                throw new InvalidDataException("Proto data ends in the middle of a field header.");

            id = reader.ReadUInt16();
            int length = reader.ReadInt32();

            if (length < 0 || length > end - stream.Position)
                throw new InvalidDataException($"Proto field {id} declares an invalid payload length of {length} bytes.");

            return stream.Position + length;
        }

        /// <summary>Verifies that the field payload was not overrun and moves to the end of the field.</summary>
        protected static void EndField(BinaryReader reader, ushort id, long fieldEnd)
        {
            Stream stream = reader.BaseStream;

            if (stream.Position > fieldEnd)
                throw new InvalidDataException($"Proto field {id} payload is longer than its declared length.");

            stream.Position = fieldEnd;
        }

        /// <summary>Reads a collection element count and verifies that it fits in the remaining bytes.</summary>
        protected static int ReadCount(BinaryReader reader, long end, int minElementSize)
        {
            int  count     = reader.ReadInt32();
            long remaining = end - reader.BaseStream.Position;

            if (count < 0 || remaining < 0 || (long)count * minElementSize > remaining)
                throw new InvalidDataException($"Proto collection declares an invalid element count of {count}.");

            return count;
        }

        /// <summary>Reads a length prefix and returns the position where the prefixed payload ends.</summary>
        protected static long ReadLengthPrefix(BinaryReader reader, long end)
        {
            int  length    = reader.ReadInt32();
            long remaining = end - reader.BaseStream.Position;

            if (length < 0 || remaining < 0 || length > remaining)
                throw new InvalidDataException($"Proto item declares an invalid length of {length} bytes.");

            return reader.BaseStream.Position + length;
        }

        #endregion Generated code contract

        private static bool _IsDecodingFailure(Exception exception)
            => exception is EndOfStreamException
                   or OverflowException
                   or ArgumentException
                   or InvalidOperationException
                   or FormatException;

        private static InvalidDataException _WrapDecodingFailure(Exception exception)
            => new InvalidDataException($"Proto data is malformed: {exception.Message}", exception);
    }
}
