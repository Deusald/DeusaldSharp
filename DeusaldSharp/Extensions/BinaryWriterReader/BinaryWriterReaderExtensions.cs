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
            for (int x = 0; x < size; x++) list.Add(reader.ReadByte());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadSByte());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadBoolean());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadInt16());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadUInt16());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadInt32());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadUInt32());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadInt64());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadUInt64());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadSingle());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadDouble());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadChar());
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadString());
            return list;
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
            int    size  = reader.ReadInt32();
            byte[] array = new byte[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadByte();
            return array;
        }

        // SBYTE
        public static void WriteArray(this BinaryWriter writer, sbyte[] values)
        {
            writer.Write(values.Length);
            foreach (sbyte v in values) writer.Write(v);
        }

        public static sbyte[] ReadSByteArray(this BinaryReader reader)
        {
            int     size  = reader.ReadInt32();
            sbyte[] array = new sbyte[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadSByte();
            return array;
        }

        // BOOL
        public static void WriteArray(this BinaryWriter writer, bool[] values)
        {
            writer.Write(values.Length);
            foreach (bool v in values) writer.Write(v);
        }

        public static bool[] ReadBoolArray(this BinaryReader reader)
        {
            int    size  = reader.ReadInt32();
            bool[] array = new bool[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadBoolean();
            return array;
        }

        // SHORT
        public static void WriteArray(this BinaryWriter writer, short[] values)
        {
            writer.Write(values.Length);
            foreach (short v in values) writer.Write(v);
        }

        public static short[] ReadShortArray(this BinaryReader reader)
        {
            int     size  = reader.ReadInt32();
            short[] array = new short[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadInt16();
            return array;
        }

        // USHORT
        public static void WriteArray(this BinaryWriter writer, ushort[] values)
        {
            writer.Write(values.Length);
            foreach (ushort v in values) writer.Write(v);
        }

        public static ushort[] ReadUShortArray(this BinaryReader reader)
        {
            int      size  = reader.ReadInt32();
            ushort[] array = new ushort[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadUInt16();
            return array;
        }

        // INT
        public static void WriteArray(this BinaryWriter writer, int[] values)
        {
            writer.Write(values.Length);
            foreach (int v in values) writer.Write(v);
        }

        public static int[] ReadIntArray(this BinaryReader reader)
        {
            int   size  = reader.ReadInt32();
            int[] array = new int[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadInt32();
            return array;
        }

        // UINT
        public static void WriteArray(this BinaryWriter writer, uint[] values)
        {
            writer.Write(values.Length);
            foreach (uint v in values) writer.Write(v);
        }

        public static uint[] ReadUIntArray(this BinaryReader reader)
        {
            int    size  = reader.ReadInt32();
            uint[] array = new uint[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadUInt32();
            return array;
        }

        // LONG
        public static void WriteArray(this BinaryWriter writer, long[] values)
        {
            writer.Write(values.Length);
            foreach (long v in values) writer.Write(v);
        }

        public static long[] ReadLongArray(this BinaryReader reader)
        {
            int    size  = reader.ReadInt32();
            long[] array = new long[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadInt64();
            return array;
        }

        // ULONG
        public static void WriteArray(this BinaryWriter writer, ulong[] values)
        {
            writer.Write(values.Length);
            foreach (ulong v in values) writer.Write(v);
        }

        public static ulong[] ReadULongArray(this BinaryReader reader)
        {
            int     size  = reader.ReadInt32();
            ulong[] array = new ulong[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadUInt64();
            return array;
        }

        // FLOAT
        public static void WriteArray(this BinaryWriter writer, float[] values)
        {
            writer.Write(values.Length);
            foreach (float v in values) writer.Write(v);
        }

        public static float[] ReadFloatArray(this BinaryReader reader)
        {
            int     size  = reader.ReadInt32();
            float[] array = new float[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadSingle();
            return array;
        }

        // DOUBLE
        public static void WriteArray(this BinaryWriter writer, double[] values)
        {
            writer.Write(values.Length);
            foreach (double v in values) writer.Write(v);
        }

        public static double[] ReadDoubleArray(this BinaryReader reader)
        {
            int      size  = reader.ReadInt32();
            double[] array = new double[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadDouble();
            return array;
        }

        // CHAR
        public static void WriteArray(this BinaryWriter writer, char[] values)
        {
            writer.Write(values.Length);
            foreach (char v in values) writer.Write(v);
        }

        public static char[] ReadCharArray(this BinaryReader reader)
        {
            int    size  = reader.ReadInt32();
            char[] array = new char[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadChar();
            return array;
        }

        // STRING
        public static void WriteArray(this BinaryWriter writer, string[] values)
        {
            writer.Write(values.Length);
            foreach (string v in values) writer.Write(v);
        }

        public static string[] ReadStringArray(this BinaryReader reader)
        {
            int      size  = reader.ReadInt32();
            string[] array = new string[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadString();
            return array;
        }

        #endregion Primitive Arrays

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
            for (int x = 0; x < size; x++) list.Add(reader.ReadGuid());
            return list;
        }

        public static void Write(this BinaryWriter writer, Guid[] values)
        {
            writer.Write(values.Length);
            foreach (Guid v in values) writer.Write(v);
        }

        public static Guid[] ReadGuidArray(this BinaryReader reader)
        {
            int    size  = reader.ReadInt32();
            Guid[] array = new Guid[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadGuid();
            return array;
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadDateTime());
            return list;
        }

        public static void Write(this BinaryWriter writer, DateTime[] values)
        {
            writer.Write(values.Length);
            foreach (DateTime v in values) writer.Write(v);
        }

        public static DateTime[] ReadDateTimeArray(this BinaryReader reader)
        {
            int        size  = reader.ReadInt32();
            DateTime[] array = new DateTime[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadDateTime();
            return array;
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadTimeSpan());
            return list;
        }

        public static void Write(this BinaryWriter writer, TimeSpan[] values)
        {
            writer.Write(values.Length);
            foreach (TimeSpan v in values) writer.Write(v);
        }

        public static TimeSpan[] ReadTimeSpanArray(this BinaryReader reader)
        {
            int        size  = reader.ReadInt32();
            TimeSpan[] array = new TimeSpan[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadTimeSpan();
            return array;
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadVersion());
            return list;
        }

        public static void Write(this BinaryWriter writer, Version[] values)
        {
            writer.Write(values.Length);
            foreach (Version v in values) writer.Write(v);
        }

        public static Version[] ReadVersionArray(this BinaryReader reader)
        {
            int       size  = reader.ReadInt32();
            Version[] array = new Version[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadVersion();
            return array;
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
            for (int x = 0; x < size; x++) list.Add(reader.ReadSerializableEnum<TEnum>());
            return list;
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
            int     size  = reader.ReadInt32();
            TEnum[] array = new TEnum[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadSerializableEnum<TEnum>();
            return array;
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
            int                  size = reader.ReadInt32();
            List<HttpStatusCode> list = new List<HttpStatusCode>(size);
            for (int x = 0; x < size; x++) list.Add(reader.ReadHttpStatusCode());
            return list;
        }

        public static void Write(this BinaryWriter writer, HttpStatusCode[] values)
        {
            writer.Write(values.Length);
            foreach (HttpStatusCode v in values) writer.Write(v);
        }

        public static HttpStatusCode[] ReadHttpStatusCodeArray(this BinaryReader reader)
        {
            int              size  = reader.ReadInt32();
            HttpStatusCode[] array = new HttpStatusCode[size];
            for (int x = 0; x < size; x++) array[x] = reader.ReadHttpStatusCode();
            return array;
        }

        #endregion HttpStatusCode
    }
}