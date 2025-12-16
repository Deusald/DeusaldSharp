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

namespace DeusaldSharp
{
    public static class BinaryWriterReaderExtensions
    {
        #region Primitive Lists

        // BYTE
        public static void Write(this BinaryWriter writer, List<byte> values)
        {
            writer.Write(values.Count);
            foreach (byte v in values) writer.Write(v);
        }

        public static List<byte> ReadByteList(this BinaryReader reader)
        {
            int        size = reader.ReadInt32();
            List<byte> list = new List<byte>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadByte());
            return list;
        }

        // SBYTE
        public static void Write(this BinaryWriter writer, List<sbyte> values)
        {
            writer.Write(values.Count);
            foreach (sbyte v in values) writer.Write(v);
        }

        public static List<sbyte> ReadSByteList(this BinaryReader reader)
        {
            int         size = reader.ReadInt32();
            List<sbyte> list = new List<sbyte>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadSByte());
            return list;
        }

        // BOOL
        public static void Write(this BinaryWriter writer, List<bool> values)
        {
            writer.Write(values.Count);
            foreach (bool v in values) writer.Write(v);
        }

        public static List<bool> ReadBoolList(this BinaryReader reader)
        {
            int        size = reader.ReadInt32();
            List<bool> list = new List<bool>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadBoolean());
            return list;
        }

        // SHORT
        public static void Write(this BinaryWriter writer, List<short> values)
        {
            writer.Write(values.Count);
            foreach (short v in values) writer.Write(v);
        }

        public static List<short> ReadShortList(this BinaryReader reader)
        {
            int         size = reader.ReadInt32();
            List<short> list = new List<short>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadInt16());
            return list;
        }

        // USHORT
        public static void Write(this BinaryWriter writer, List<ushort> values)
        {
            writer.Write(values.Count);
            foreach (ushort v in values) writer.Write(v);
        }

        public static List<ushort> ReadUShortList(this BinaryReader reader)
        {
            int          size = reader.ReadInt32();
            List<ushort> list = new List<ushort>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadUInt16());
            return list;
        }

        // INT
        public static void Write(this BinaryWriter writer, List<int> values)
        {
            writer.Write(values.Count);
            foreach (int v in values) writer.Write(v);
        }

        public static List<int> ReadIntList(this BinaryReader reader)
        {
            int       size = reader.ReadInt32();
            List<int> list = new List<int>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadInt32());
            return list;
        }

        // UINT
        public static void Write(this BinaryWriter writer, List<uint> values)
        {
            writer.Write(values.Count);
            foreach (uint v in values) writer.Write(v);
        }

        public static List<uint> ReadUIntList(this BinaryReader reader)
        {
            int        size = reader.ReadInt32();
            List<uint> list = new List<uint>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadUInt32());
            return list;
        }

        // LONG
        public static void Write(this BinaryWriter writer, List<long> values)
        {
            writer.Write(values.Count);
            foreach (long v in values) writer.Write(v);
        }

        public static List<long> ReadLongList(this BinaryReader reader)
        {
            int        size = reader.ReadInt32();
            List<long> list = new List<long>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadInt64());
            return list;
        }

        // ULONG
        public static void Write(this BinaryWriter writer, List<ulong> values)
        {
            writer.Write(values.Count);
            foreach (ulong v in values) writer.Write(v);
        }

        public static List<ulong> ReadULongList(this BinaryReader reader)
        {
            int         size = reader.ReadInt32();
            List<ulong> list = new List<ulong>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadUInt64());
            return list;
        }

        // FLOAT
        public static void Write(this BinaryWriter writer, List<float> values)
        {
            writer.Write(values.Count);
            foreach (float v in values) writer.Write(v);
        }

        public static List<float> ReadFloatList(this BinaryReader reader)
        {
            int         size = reader.ReadInt32();
            List<float> list = new List<float>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadSingle());
            return list;
        }

        // DOUBLE
        public static void Write(this BinaryWriter writer, List<double> values)
        {
            writer.Write(values.Count);
            foreach (double v in values) writer.Write(v);
        }

        public static List<double> ReadDoubleList(this BinaryReader reader)
        {
            int          size = reader.ReadInt32();
            List<double> list = new List<double>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadDouble());
            return list;
        }

        // CHAR
        public static void Write(this BinaryWriter writer, List<char> values)
        {
            writer.Write(values.Count);
            foreach (char v in values) writer.Write(v);
        }

        public static List<char> ReadCharList(this BinaryReader reader)
        {
            int        size = reader.ReadInt32();
            List<char> list = new List<char>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadChar());
            return list;
        }

        // STRING
        public static void Write(this BinaryWriter writer, List<string> values)
        {
            writer.Write(values.Count);
            foreach (string v in values) writer.Write(v ?? string.Empty);
        }

        public static List<string> ReadStringList(this BinaryReader reader)
        {
            int          size = reader.ReadInt32();
            List<string> list = new List<string>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadString());
            return list;
        }

        #endregion Primitive Lists

        #region GUID

        public static void Write(this BinaryWriter binaryWriter, Guid guid)
        {
            binaryWriter.Write(guid.ToByteArray());
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        public static void Write(this BinaryWriter writer, List<Guid> values)
        {
            writer.Write(values.Count);
            foreach (Guid v in values) writer.Write(v);
        }

        public static List<Guid> ReadGuidList(this BinaryReader reader)
        {
            int        size = reader.ReadInt32();
            List<Guid> list = new List<Guid>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadGuid());
            return list;
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
            int            size = reader.ReadInt32();
            List<DateTime> list = new List<DateTime>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadDateTime());
            return list;
        }

        #endregion DateTime

        #region TimeSpan

        public static void Write(this BinaryWriter binaryWriter, TimeSpan timeSpan)
        {
            binaryWriter.Write(timeSpan.TotalMilliseconds);
        }

        public static TimeSpan ReadTimeSpan(this BinaryReader binaryReader)
        {
            return TimeSpan.FromMilliseconds(binaryReader.ReadDouble());
        }
        
        public static void Write(this BinaryWriter writer, List<TimeSpan> values)
        {
            writer.Write(values.Count);
            foreach (TimeSpan v in values) writer.Write(v);
        }

        public static List<TimeSpan> ReadTimeSpanList(this BinaryReader reader)
        {
            int            size = reader.ReadInt32();
            List<TimeSpan> list = new List<TimeSpan>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadTimeSpan());
            return list;
        }

        #endregion TimeSpan

        #region Version

        public static void Write(this BinaryWriter binaryWriter, Version version)
        {
            binaryWriter.Write(version.Major);
            binaryWriter.Write(version.Minor);
            binaryWriter.Write(version.Build);
        }

        public static Version ReadVersion(this BinaryReader binaryReader)
        {
            return new Version(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
        }
        
        public static void Write(this BinaryWriter writer, List<Version> values)
        {
            writer.Write(values.Count);
            foreach (Version v in values) writer.Write(v);
        }

        public static List<Version> ReadVersionList(this BinaryReader reader)
        {
            int           size = reader.ReadInt32();
            List<Version> list = new List<Version>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadVersion());
            return list;
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
            int         size = reader.ReadInt32();
            List<TEnum> list = new List<TEnum>(size);
            for (int i = 0; i < size; i++) list.Add(reader.ReadSerializableEnum<TEnum>());
            return list;
        }

        #endregion SerializableEnum
    }
}