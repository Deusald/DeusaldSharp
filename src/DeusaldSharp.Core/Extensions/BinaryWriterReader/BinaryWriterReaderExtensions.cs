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
using System.Collections.Generic;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace DeusaldSharp
{
    [PublicAPI]
    public static class BinaryWriterReaderExtensions
    {
        private const int _GUID_SIZE                 = 16;
        private const int _VERSION_SIZE              = 16; // Major, Minor, Build, Revision as int32
        private const int _MAX_UNTRUSTED_PREALLOCATE = 1024;

        #region Collection Helpers

        /// <summary>
        /// Reads a collection length prefix and validates it, as it comes from untrusted data.
        /// On seekable streams the length must fit the remaining bytes, given the minimal wire size of one element.
        /// </summary>
        private static int ReadCollectionLength(BinaryReader reader, int minElementSize)
        {
            int size = reader.ReadInt32();
            if (size < 0) throw new InvalidDataException($"Invalid collection length {size}.");

            Stream stream = reader.BaseStream;
            if (!stream.CanSeek) return size;

            long remaining = stream.Length - stream.Position;
            if ((long)size * minElementSize > remaining)
                throw new InvalidDataException($"Collection length {size} exceeds the {remaining} bytes remaining in the stream.");

            return size;
        }

        private static List<T> ReadList<T>(BinaryReader reader, int minElementSize, Func<BinaryReader, T> readElement)
        {
            int size = ReadCollectionLength(reader, minElementSize);
            return ReadElements(reader, size, readElement);
        }

        private static T[] ReadArray<T>(BinaryReader reader, int minElementSize, Func<BinaryReader, T> readElement)
        {
            int size = ReadCollectionLength(reader, minElementSize);

            // Non-seekable stream: the length was not validated, so grow with the data actually received.
            if (!reader.BaseStream.CanSeek) return ReadElements(reader, size, readElement).ToArray();

            T[] array = new T[size];
            for (int x = 0; x < size; ++x) array[x] = readElement(reader);
            return array;
        }

        private static List<T> ReadElements<T>(BinaryReader reader, int size, Func<BinaryReader, T> readElement)
        {
            int     capacity = reader.BaseStream.CanSeek ? size : Math.Min(size, _MAX_UNTRUSTED_PREALLOCATE);
            List<T> list     = new List<T>(capacity);
            for (int x = 0; x < size; ++x) list.Add(readElement(reader));
            return list;
        }

        #endregion Collection Helpers

        #region Primitive Lists

        // BYTE
        public static void Write(this BinaryWriter writer, List<byte> values)
        {
            writer.Write(values.Count);
            foreach (byte v in values) writer.Write(v);
        }

        public static List<byte> ReadByteList(this BinaryReader reader)
        {
            return ReadList(reader, 1, r => r.ReadByte());
        }

        // SBYTE
        public static void Write(this BinaryWriter writer, List<sbyte> values)
        {
            writer.Write(values.Count);
            foreach (sbyte v in values) writer.Write(v);
        }

        public static List<sbyte> ReadSByteList(this BinaryReader reader)
        {
            return ReadList(reader, 1, r => r.ReadSByte());
        }

        // BOOL
        public static void Write(this BinaryWriter writer, List<bool> values)
        {
            writer.Write(values.Count);
            foreach (bool v in values) writer.Write(v);
        }

        public static List<bool> ReadBoolList(this BinaryReader reader)
        {
            return ReadList(reader, 1, r => r.ReadBoolean());
        }

        // SHORT
        public static void Write(this BinaryWriter writer, List<short> values)
        {
            writer.Write(values.Count);
            foreach (short v in values) writer.Write(v);
        }

        public static List<short> ReadShortList(this BinaryReader reader)
        {
            return ReadList(reader, 2, r => r.ReadInt16());
        }

        // USHORT
        public static void Write(this BinaryWriter writer, List<ushort> values)
        {
            writer.Write(values.Count);
            foreach (ushort v in values) writer.Write(v);
        }

        public static List<ushort> ReadUShortList(this BinaryReader reader)
        {
            return ReadList(reader, 2, r => r.ReadUInt16());
        }

        // INT
        public static void Write(this BinaryWriter writer, List<int> values)
        {
            writer.Write(values.Count);
            foreach (int v in values) writer.Write(v);
        }

        public static List<int> ReadIntList(this BinaryReader reader)
        {
            return ReadList(reader, 4, r => r.ReadInt32());
        }

        // UINT
        public static void Write(this BinaryWriter writer, List<uint> values)
        {
            writer.Write(values.Count);
            foreach (uint v in values) writer.Write(v);
        }

        public static List<uint> ReadUIntList(this BinaryReader reader)
        {
            return ReadList(reader, 4, r => r.ReadUInt32());
        }

        // LONG
        public static void Write(this BinaryWriter writer, List<long> values)
        {
            writer.Write(values.Count);
            foreach (long v in values) writer.Write(v);
        }

        public static List<long> ReadLongList(this BinaryReader reader)
        {
            return ReadList(reader, 8, r => r.ReadInt64());
        }

        // ULONG
        public static void Write(this BinaryWriter writer, List<ulong> values)
        {
            writer.Write(values.Count);
            foreach (ulong v in values) writer.Write(v);
        }

        public static List<ulong> ReadULongList(this BinaryReader reader)
        {
            return ReadList(reader, 8, r => r.ReadUInt64());
        }

        // FLOAT
        public static void Write(this BinaryWriter writer, List<float> values)
        {
            writer.Write(values.Count);
            foreach (float v in values) writer.Write(v);
        }

        public static List<float> ReadFloatList(this BinaryReader reader)
        {
            return ReadList(reader, 4, r => r.ReadSingle());
        }

        // DOUBLE
        public static void Write(this BinaryWriter writer, List<double> values)
        {
            writer.Write(values.Count);
            foreach (double v in values) writer.Write(v);
        }

        public static List<double> ReadDoubleList(this BinaryReader reader)
        {
            return ReadList(reader, 8, r => r.ReadDouble());
        }

        // CHAR
        public static void Write(this BinaryWriter writer, List<char> values)
        {
            writer.Write(values.Count);
            foreach (char v in values) writer.Write(v);
        }

        public static List<char> ReadCharList(this BinaryReader reader)
        {
            return ReadList(reader, 1, r => r.ReadChar());
        }

        // STRING
        public static void Write(this BinaryWriter writer, List<string> values)
        {
            writer.Write(values.Count);
            foreach (string v in values) writer.Write(v ?? string.Empty);
        }

        public static List<string> ReadStringList(this BinaryReader reader)
        {
            return ReadList(reader, 1, r => r.ReadString());
        }

        #endregion Primitive Lists

        #region Primitive Arrays

        // BYTE
        public static void WriteArray(this BinaryWriter writer, byte[] values)
        {
            writer.Write(values.Length);
            foreach (byte v in values) writer.Write(v);
        }

        public static byte[] ReadByteArray(this BinaryReader reader)
        {
            return ReadArray(reader, 1, r => r.ReadByte());
        }

        // SBYTE
        public static void WriteArray(this BinaryWriter writer, sbyte[] values)
        {
            writer.Write(values.Length);
            foreach (sbyte v in values) writer.Write(v);
        }

        public static sbyte[] ReadSByteArray(this BinaryReader reader)
        {
            return ReadArray(reader, 1, r => r.ReadSByte());
        }

        // BOOL
        public static void WriteArray(this BinaryWriter writer, bool[] values)
        {
            writer.Write(values.Length);
            foreach (bool v in values) writer.Write(v);
        }

        public static bool[] ReadBoolArray(this BinaryReader reader)
        {
            return ReadArray(reader, 1, r => r.ReadBoolean());
        }

        // SHORT
        public static void WriteArray(this BinaryWriter writer, short[] values)
        {
            writer.Write(values.Length);
            foreach (short v in values) writer.Write(v);
        }

        public static short[] ReadShortArray(this BinaryReader reader)
        {
            return ReadArray(reader, 2, r => r.ReadInt16());
        }

        // USHORT
        public static void WriteArray(this BinaryWriter writer, ushort[] values)
        {
            writer.Write(values.Length);
            foreach (ushort v in values) writer.Write(v);
        }

        public static ushort[] ReadUShortArray(this BinaryReader reader)
        {
            return ReadArray(reader, 2, r => r.ReadUInt16());
        }

        // INT
        public static void WriteArray(this BinaryWriter writer, int[] values)
        {
            writer.Write(values.Length);
            foreach (int v in values) writer.Write(v);
        }

        public static int[] ReadIntArray(this BinaryReader reader)
        {
            return ReadArray(reader, 4, r => r.ReadInt32());
        }

        // UINT
        public static void WriteArray(this BinaryWriter writer, uint[] values)
        {
            writer.Write(values.Length);
            foreach (uint v in values) writer.Write(v);
        }

        public static uint[] ReadUIntArray(this BinaryReader reader)
        {
            return ReadArray(reader, 4, r => r.ReadUInt32());
        }

        // LONG
        public static void WriteArray(this BinaryWriter writer, long[] values)
        {
            writer.Write(values.Length);
            foreach (long v in values) writer.Write(v);
        }

        public static long[] ReadLongArray(this BinaryReader reader)
        {
            return ReadArray(reader, 8, r => r.ReadInt64());
        }

        // ULONG
        public static void WriteArray(this BinaryWriter writer, ulong[] values)
        {
            writer.Write(values.Length);
            foreach (ulong v in values) writer.Write(v);
        }

        public static ulong[] ReadULongArray(this BinaryReader reader)
        {
            return ReadArray(reader, 8, r => r.ReadUInt64());
        }

        // FLOAT
        public static void WriteArray(this BinaryWriter writer, float[] values)
        {
            writer.Write(values.Length);
            foreach (float v in values) writer.Write(v);
        }

        public static float[] ReadFloatArray(this BinaryReader reader)
        {
            return ReadArray(reader, 4, r => r.ReadSingle());
        }

        // DOUBLE
        public static void WriteArray(this BinaryWriter writer, double[] values)
        {
            writer.Write(values.Length);
            foreach (double v in values) writer.Write(v);
        }

        public static double[] ReadDoubleArray(this BinaryReader reader)
        {
            return ReadArray(reader, 8, r => r.ReadDouble());
        }

        // CHAR
        public static void WriteArray(this BinaryWriter writer, char[] values)
        {
            writer.Write(values.Length);
            foreach (char v in values) writer.Write(v);
        }

        public static char[] ReadCharArray(this BinaryReader reader)
        {
            return ReadArray(reader, 1, r => r.ReadChar());
        }

        // STRING
        public static void WriteArray(this BinaryWriter writer, string[] values)
        {
            writer.Write(values.Length);
            foreach (string v in values) writer.Write(v ?? string.Empty);
        }

        public static string[] ReadStringArray(this BinaryReader reader)
        {
            return ReadArray(reader, 1, r => r.ReadString());
        }

        #endregion Primitive Arrays

        #region GUID

        public static void Write(this BinaryWriter binaryWriter, Guid guid)
        {
            binaryWriter.Write(guid.ToByteArray());
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(_GUID_SIZE);
            if (bytes.Length != _GUID_SIZE) throw new EndOfStreamException("Unable to read beyond the end of the stream while reading a Guid.");
            return new Guid(bytes);
        }

        public static void Write(this BinaryWriter writer, List<Guid> values)
        {
            writer.Write(values.Count);
            foreach (Guid v in values) writer.Write(v);
        }

        public static List<Guid> ReadGuidList(this BinaryReader reader)
        {
            return ReadList(reader, _GUID_SIZE, r => r.ReadGuid());
        }

        public static void Write(this BinaryWriter writer, Guid[] values)
        {
            writer.Write(values.Length);
            foreach (Guid v in values) writer.Write(v);
        }

        public static Guid[] ReadGuidArray(this BinaryReader reader)
        {
            return ReadArray(reader, _GUID_SIZE, r => r.ReadGuid());
        }

        #endregion GUID

        #region DateTime

        public static void Write(this BinaryWriter binaryWriter, DateTime dateTime)
        {
            binaryWriter.Write(dateTime.ToBinary());
        }

        public static DateTime ReadDateTime(this BinaryReader binaryReader)
        {
            long data = binaryReader.ReadInt64();
            return DateTime.FromBinary(data);
        }
        
        public static void Write(this BinaryWriter writer, List<DateTime> values)
        {
            writer.Write(values.Count);
            foreach (DateTime v in values) writer.Write(v);
        }

        public static List<DateTime> ReadDateTimeList(this BinaryReader reader)
        {
            return ReadList(reader, 8, r => r.ReadDateTime());
        }

        public static void Write(this BinaryWriter writer, DateTime[] values)
        {
            writer.Write(values.Length);
            foreach (DateTime v in values) writer.Write(v);
        }

        public static DateTime[] ReadDateTimeArray(this BinaryReader reader)
        {
            return ReadArray(reader, 8, r => r.ReadDateTime());
        }

        #endregion DateTime

        #region TimeSpan

        public static void Write(this BinaryWriter binaryWriter, TimeSpan timeSpan)
        {
            // Ticks are exact; a double of milliseconds is lossy and rounds on some runtimes.
            binaryWriter.Write(timeSpan.Ticks);
        }

        public static TimeSpan ReadTimeSpan(this BinaryReader binaryReader)
        {
            return TimeSpan.FromTicks(binaryReader.ReadInt64());
        }
        
        public static void Write(this BinaryWriter writer, List<TimeSpan> values)
        {
            writer.Write(values.Count);
            foreach (TimeSpan v in values) writer.Write(v);
        }

        public static List<TimeSpan> ReadTimeSpanList(this BinaryReader reader)
        {
            return ReadList(reader, 8, r => r.ReadTimeSpan());
        }

        public static void Write(this BinaryWriter writer, TimeSpan[] values)
        {
            writer.Write(values.Length);
            foreach (TimeSpan v in values) writer.Write(v);
        }

        public static TimeSpan[] ReadTimeSpanArray(this BinaryReader reader)
        {
            return ReadArray(reader, 8, r => r.ReadTimeSpan());
        }

        #endregion TimeSpan

        #region Version

        public static void Write(this BinaryWriter binaryWriter, Version version)
        {
            binaryWriter.Write(version.Major);
            binaryWriter.Write(version.Minor);
            binaryWriter.Write(version.Build);
            binaryWriter.Write(version.Revision);
        }

        public static Version ReadVersion(this BinaryReader binaryReader)
        {
            int major    = binaryReader.ReadInt32();
            int minor    = binaryReader.ReadInt32();
            int build    = binaryReader.ReadInt32();
            int revision = binaryReader.ReadInt32();

            // Undefined components are stored as -1 (e.g. Version(1, 2) has Build = -1 and Revision = -1).
            if (major < 0 || minor < 0 || build < -1 || revision < -1 || (build == -1 && revision != -1))
                throw new InvalidDataException($"Invalid serialized Version {major}.{minor}.{build}.{revision}.");

            if (build    == -1) return new Version(major, minor);
            if (revision == -1) return new Version(major, minor, build);
            return new Version(major, minor, build, revision);
        }
        
        public static void Write(this BinaryWriter writer, List<Version> values)
        {
            writer.Write(values.Count);
            foreach (Version v in values) writer.Write(v);
        }

        public static List<Version> ReadVersionList(this BinaryReader reader)
        {
            return ReadList(reader, _VERSION_SIZE, r => r.ReadVersion());
        }

        public static void Write(this BinaryWriter writer, Version[] values)
        {
            writer.Write(values.Length);
            foreach (Version v in values) writer.Write(v);
        }

        public static Version[] ReadVersionArray(this BinaryReader reader)
        {
            return ReadArray(reader, _VERSION_SIZE, r => r.ReadVersion());
        }

        #endregion Version

        #region SerializableEnum

        public static void WriteSerializableEnum<TEnum>(
            this BinaryWriter writer,
            TEnum value)
            where TEnum : unmanaged, Enum
        {
            SerializableEnumCache<TEnum>.Write(writer, value);
        }

        public static TEnum ReadSerializableEnum<TEnum>(
            this BinaryReader reader)
            where TEnum : unmanaged, Enum
        {
            return SerializableEnumCache<TEnum>.Read(reader);
        }
        
        public static void WriteSerializableEnumList<TEnum>(this BinaryWriter writer, List<TEnum> values)
            where TEnum : unmanaged, Enum
        {
            writer.Write(values.Count);
            foreach (TEnum v in values) writer.WriteSerializableEnum(v);
        }

        public static List<TEnum> ReadSerializableEnumList<TEnum>(this BinaryReader reader)
            where TEnum : unmanaged, Enum
        {
            return ReadList(reader, SerializableEnumCache<TEnum>.MinWireSize, r => r.ReadSerializableEnum<TEnum>());
        }

        public static void WriteSerializableEnumArray<TEnum>(this BinaryWriter writer, TEnum[] values)
            where TEnum : unmanaged, Enum
        {
            writer.Write(values.Length);
            foreach (TEnum v in values) writer.WriteSerializableEnum(v);
        }

        public static TEnum[] ReadSerializableEnumArray<TEnum>(this BinaryReader reader)
            where TEnum : unmanaged, Enum
        {
            return ReadArray(reader, SerializableEnumCache<TEnum>.MinWireSize, r => r.ReadSerializableEnum<TEnum>());
        }

        #endregion SerializableEnum

        #region HttpStatusCode

        public static void Write(this BinaryWriter binaryWriter, HttpStatusCode httpStatusCode)
        {
            binaryWriter.Write((int)httpStatusCode);
        }

        public static HttpStatusCode ReadHttpStatusCode(this BinaryReader binaryReader)
        {
            return (HttpStatusCode)binaryReader.ReadInt32();
        }
        
        public static void Write(this BinaryWriter writer, List<HttpStatusCode> values)
        {
            writer.Write(values.Count);
            foreach (HttpStatusCode v in values) writer.Write(v);
        }

        public static List<HttpStatusCode> ReadHttpStatusCodeList(this BinaryReader reader)
        {
            return ReadList(reader, 4, r => r.ReadHttpStatusCode());
        }

        public static void Write(this BinaryWriter writer, HttpStatusCode[] values)
        {
            writer.Write(values.Length);
            foreach (HttpStatusCode v in values) writer.Write(v);
        }

        public static HttpStatusCode[] ReadHttpStatusCodeArray(this BinaryReader reader)
        {
            return ReadArray(reader, 4, r => r.ReadHttpStatusCode());
        }

        #endregion HttpStatusCode
    }
}