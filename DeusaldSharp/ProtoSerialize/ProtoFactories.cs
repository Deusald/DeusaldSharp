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

namespace DeusaldSharp
{
    public static class ProtoField
    {
        /* -----------------------------------------------------------
         * PRIMITIVES
         * ----------------------------------------------------------- */

        private static ProtoField<T> Primitive<T, TValue>(
            ushort id,
            RefAccessor<T, TValue> accessor,
            Action<BinaryWriter, TValue> write,
            Func<BinaryReader, TValue> read)
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) => write(w, accessor(ref o)),
                (BinaryReader r, ref T o) => accessor(ref o) = read(r)
            );
        }

        public static ProtoField<T> Bool<T>(ushort id, RefAccessor<T, bool> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadBoolean());

        public static ProtoField<T> Byte<T>(ushort id, RefAccessor<T, byte> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadByte());

        public static ProtoField<T> SByte<T>(ushort id, RefAccessor<T, sbyte> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadSByte());

        public static ProtoField<T> Short<T>(ushort id, RefAccessor<T, short> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadInt16());

        public static ProtoField<T> UShort<T>(ushort id, RefAccessor<T, ushort> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadUInt16());

        public static ProtoField<T> Int<T>(ushort id, RefAccessor<T, int> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadInt32());

        public static ProtoField<T> UInt<T>(ushort id, RefAccessor<T, uint> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadUInt32());

        public static ProtoField<T> Long<T>(ushort id, RefAccessor<T, long> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadInt64());

        public static ProtoField<T> ULong<T>(ushort id, RefAccessor<T, ulong> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadUInt64());

        public static ProtoField<T> Float<T>(ushort id, RefAccessor<T, float> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadSingle());

        public static ProtoField<T> Double<T>(ushort id, RefAccessor<T, double> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadDouble());

        public static ProtoField<T> Char<T>(ushort id, RefAccessor<T, char> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadChar());

        public static ProtoField<T> String<T>(ushort id, RefAccessor<T, string> a)
            => Primitive(id, a, (w, v) => w.Write(v ?? string.Empty), r => r.ReadString());


        /* -----------------------------------------------------------
         * SPECIAL TYPES
         * ----------------------------------------------------------- */

        public static ProtoField<T> Guid<T>(ushort id, RefAccessor<T, Guid> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadGuid());

        public static ProtoField<T> DateTime<T>(ushort id, RefAccessor<T, DateTime> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadDateTime());

        public static ProtoField<T> TimeSpan<T>(ushort id, RefAccessor<T, TimeSpan> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadTimeSpan());

        public static ProtoField<T> Version<T>(ushort id, RefAccessor<T, Version> a)
            => Primitive(id, a, (w, v) => w.Write(v), r => r.ReadVersion());

        public static ProtoField<T> HttpStatusCode<T>(ushort id, RefAccessor<T, HttpStatusCode> a)
            => Primitive(id, a, (w, v) => w.Write((int)v), r => (HttpStatusCode)r.ReadInt32());

        /* -----------------------------------------------------------
         * NULLABLE VALUE TYPES (bool + value)
         * ----------------------------------------------------------- */

        private static ProtoField<T> NullableValue<T, TValue>(
            ushort id,
            RefAccessor<T, TValue?> accessor,
            Action<BinaryWriter, TValue> write,
            Func<BinaryReader, TValue> read)
            where TValue : struct
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) =>
                {
                    var v = accessor(ref o);
                    w.Write(v.HasValue);
                    if (v.HasValue)
                        write(w, v.Value);
                },
                (BinaryReader r, ref T o) =>
                {
                    if (!r.ReadBoolean())
                    {
                        accessor(ref o) = null;
                        return;
                    }

                    accessor(ref o) = read(r);
                }
            );
        }

        public static ProtoField<T> NullableBool<T>(ushort id, RefAccessor<T, bool?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadBoolean());

        public static ProtoField<T> NullableByte<T>(ushort id, RefAccessor<T, byte?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadByte());

        public static ProtoField<T> NullableSByte<T>(ushort id, RefAccessor<T, sbyte?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadSByte());

        public static ProtoField<T> NullableShort<T>(ushort id, RefAccessor<T, short?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadInt16());

        public static ProtoField<T> NullableUShort<T>(ushort id, RefAccessor<T, ushort?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadUInt16());

        public static ProtoField<T> NullableInt<T>(ushort id, RefAccessor<T, int?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadInt32());

        public static ProtoField<T> NullableUInt<T>(ushort id, RefAccessor<T, uint?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadUInt32());

        public static ProtoField<T> NullableLong<T>(ushort id, RefAccessor<T, long?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadInt64());

        public static ProtoField<T> NullableULong<T>(ushort id, RefAccessor<T, ulong?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadUInt64());

        public static ProtoField<T> NullableFloat<T>(ushort id, RefAccessor<T, float?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadSingle());

        public static ProtoField<T> NullableDouble<T>(ushort id, RefAccessor<T, double?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadDouble());

        public static ProtoField<T> NullableChar<T>(ushort id, RefAccessor<T, char?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadChar());

        public static ProtoField<T> NullableGuid<T>(ushort id, RefAccessor<T, Guid?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadGuid());

        public static ProtoField<T> NullableDateTime<T>(ushort id, RefAccessor<T, DateTime?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadDateTime());

        public static ProtoField<T> NullableTimeSpan<T>(ushort id, RefAccessor<T, TimeSpan?> a)
            => NullableValue(id, a, (w, v) => w.Write(v), r => r.ReadTimeSpan());


        /* -----------------------------------------------------------
         * SERIALIZABLE ENUM
         * ----------------------------------------------------------- */

        public static ProtoField<T> SerializableEnum<T, TEnum>(
            ushort id,
            RefAccessor<T, TEnum> a)
            where TEnum : unmanaged, Enum
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) => w.WriteSerializableEnum(a(ref o)),
                (BinaryReader r, ref T o) => a(ref o) = r.ReadSerializableEnum<TEnum>()
            );
        }

        public static ProtoField<T> NullableSerializableEnum<T, TEnum>(
            ushort id,
            RefAccessor<T, TEnum?> a)
            where TEnum : unmanaged, Enum
        {
            return NullableValue(
                id,
                a,
                (w, v) => w.WriteSerializableEnum(v),
                r => r.ReadSerializableEnum<TEnum>()
            );
        }


        /* -----------------------------------------------------------
         * OBJECTS (ProtoMsg)
         * ----------------------------------------------------------- */

        public static ProtoField<T> Object<T, TObject>(
            ushort id,
            RefAccessor<T, TObject> a)
            where TObject : ProtoMsg<TObject>, new()
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) =>
                {
                    byte[] data = a(ref o).Serialize();
                    w.Write(data.Length);
                    w.Write(data);
                },
                (BinaryReader r, ref T o) =>
                {
                    int    len  = r.ReadInt32();
                    byte[] data = r.ReadBytes(len);
                    a(ref o) = ProtoMsg<TObject>.Deserialize(data);
                }
            );
        }

        public static ProtoField<T> NullableObject<T, TObject>(
            ushort id,
            RefAccessor<T, TObject?> a)
            where TObject : ProtoMsg<TObject>, new()
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) =>
                {
                    var obj = a(ref o);
                    if (obj == null)
                    {
                        w.Write(0);
                        return;
                    }

                    byte[] data = obj.Serialize();
                    w.Write(data.Length);
                    w.Write(data);
                },
                (BinaryReader r, ref T o) =>
                {
                    int len = r.ReadInt32();
                    if (len <= 0)
                    {
                        a(ref o) = null;
                        return;
                    }

                    byte[] data = r.ReadBytes(len);
                    a(ref o) = ProtoMsg<TObject>.Deserialize(data);
                }
            );
        }


        /* -----------------------------------------------------------
         * LISTS – PRIMITIVES & SPECIAL TYPES
         * ----------------------------------------------------------- */

        public static ProtoField<T> List<T, TValue>(
            ushort id,
            RefAccessor<T, List<TValue>> a,
            Action<BinaryWriter, List<TValue>> write,
            Func<BinaryReader, List<TValue>> read)
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) => write(w, a(ref o)),
                (BinaryReader r, ref T o) => a(ref o) = read(r)
            );
        }

        public static ProtoField<T> ByteList<T>(ushort id, RefAccessor<T, List<byte>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadByteList());

        public static ProtoField<T> SByteList<T>(ushort id, RefAccessor<T, List<sbyte>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadSByteList());

        public static ProtoField<T> BoolList<T>(ushort id, RefAccessor<T, List<bool>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadBoolList());

        public static ProtoField<T> ShortList<T>(ushort id, RefAccessor<T, List<short>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadShortList());

        public static ProtoField<T> UShortList<T>(ushort id, RefAccessor<T, List<ushort>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadUShortList());

        public static ProtoField<T> IntList<T>(ushort id, RefAccessor<T, List<int>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadIntList());

        public static ProtoField<T> UIntList<T>(ushort id, RefAccessor<T, List<uint>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadUIntList());

        public static ProtoField<T> LongList<T>(ushort id, RefAccessor<T, List<long>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadLongList());

        public static ProtoField<T> ULongList<T>(ushort id, RefAccessor<T, List<ulong>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadULongList());

        public static ProtoField<T> FloatList<T>(ushort id, RefAccessor<T, List<float>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadFloatList());

        public static ProtoField<T> DoubleList<T>(ushort id, RefAccessor<T, List<double>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadDoubleList());

        public static ProtoField<T> CharList<T>(ushort id, RefAccessor<T, List<char>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadCharList());

        public static ProtoField<T> StringList<T>(ushort id, RefAccessor<T, List<string>> a)
            => List(id, a, static (w, v) => w.Write(v), static r => r.ReadStringList());

        public static ProtoField<T> GuidList<T>(ushort id, RefAccessor<T, List<Guid>> a)
            => List(id, a, (w, v) => w.Write(v), r => r.ReadGuidList());

        public static ProtoField<T> DateTimeList<T>(ushort id, RefAccessor<T, List<DateTime>> a)
            => List(id, a, (w, v) => w.Write(v), r => r.ReadDateTimeList());

        public static ProtoField<T> TimeSpanList<T>(ushort id, RefAccessor<T, List<TimeSpan>> a)
            => List(id, a, (w, v) => w.Write(v), r => r.ReadTimeSpanList());

        public static ProtoField<T> VersionList<T>(ushort id, RefAccessor<T, List<Version>> a)
            => List(id, a, (w, v) => w.Write(v), r => r.ReadVersionList());


        /* -----------------------------------------------------------
         * NULLABLE LISTS – PRIMITIVES & SPECIAL TYPES
         * (bool hasList + listPayload)
         * ----------------------------------------------------------- */

        private static ProtoField<T> NullableList<T, TValue>(
            ushort id,
            RefAccessor<T, List<TValue>?> a,
            Action<BinaryWriter, List<TValue>> write,
            Func<BinaryReader, List<TValue>> read)
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) =>
                {
                    var list = a(ref o);
                    w.Write(list != null);
                    if (list != null)
                        write(w, list);
                },
                (BinaryReader r, ref T o) =>
                {
                    if (!r.ReadBoolean())
                    {
                        a(ref o) = null;
                        return;
                    }

                    a(ref o) = read(r);
                }
            );
        }

        public static ProtoField<T> NullableByteList<T>(ushort id, RefAccessor<T, List<byte>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadByteList());

        public static ProtoField<T> NullableSByteList<T>(ushort id, RefAccessor<T, List<sbyte>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadSByteList());

        public static ProtoField<T> NullableBoolList<T>(ushort id, RefAccessor<T, List<bool>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadBoolList());

        public static ProtoField<T> NullableShortList<T>(ushort id, RefAccessor<T, List<short>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadShortList());

        public static ProtoField<T> NullableUShortList<T>(ushort id, RefAccessor<T, List<ushort>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadUShortList());

        public static ProtoField<T> NullableIntList<T>(ushort id, RefAccessor<T, List<int>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadIntList());

        public static ProtoField<T> NullableUIntList<T>(ushort id, RefAccessor<T, List<uint>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadUIntList());

        public static ProtoField<T> NullableLongList<T>(ushort id, RefAccessor<T, List<long>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadLongList());

        public static ProtoField<T> NullableULongList<T>(ushort id, RefAccessor<T, List<ulong>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadULongList());

        public static ProtoField<T> NullableFloatList<T>(ushort id, RefAccessor<T, List<float>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadFloatList());

        public static ProtoField<T> NullableDoubleList<T>(ushort id, RefAccessor<T, List<double>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadDoubleList());

        public static ProtoField<T> NullableCharList<T>(ushort id, RefAccessor<T, List<char>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadCharList());

        public static ProtoField<T> NullableStringList<T>(ushort id, RefAccessor<T, List<string>?> a)
            => NullableList(id, a, static (w, v) => w.Write(v), static r => r.ReadStringList());

        public static ProtoField<T> NullableGuidList<T>(ushort id, RefAccessor<T, List<Guid>?> a)
            => NullableList(id, a, (w, v) => w.Write(v), r => r.ReadGuidList());

        public static ProtoField<T> NullableDateTimeList<T>(ushort id, RefAccessor<T, List<DateTime>?> a)
            => NullableList(id, a, (w, v) => w.Write(v), r => r.ReadDateTimeList());

        public static ProtoField<T> NullableTimeSpanList<T>(ushort id, RefAccessor<T, List<TimeSpan>?> a)
            => NullableList(id, a, (w, v) => w.Write(v), r => r.ReadTimeSpanList());

        public static ProtoField<T> NullableVersionList<T>(ushort id, RefAccessor<T, List<Version>?> a)
            => NullableList(id, a, (w, v) => w.Write(v), r => r.ReadVersionList());


        /* -----------------------------------------------------------
         * LISTS – SERIALIZABLE ENUM
         * ----------------------------------------------------------- */

        public static ProtoField<T> SerializableEnumList<T, TEnum>(
            ushort id,
            RefAccessor<T, List<TEnum>> a)
            where TEnum : unmanaged, Enum
        {
            return List(
                id,
                a,
                (w, v) => w.WriteSerializableEnumList(v),
                r => r.ReadSerializableEnumList<TEnum>()
            );
        }

        public static ProtoField<T> NullableSerializableEnumList<T, TEnum>(
            ushort id,
            RefAccessor<T, List<TEnum>?> a)
            where TEnum : unmanaged, Enum
        {
            return NullableList(
                id,
                a,
                (w, v) => w.WriteSerializableEnumList(v),
                r => r.ReadSerializableEnumList<TEnum>()
            );
        }


        /* -----------------------------------------------------------
         * LISTS – OBJECTS
         * ----------------------------------------------------------- */

        public static ProtoField<T> ObjectList<T, TObject>(
            ushort id,
            RefAccessor<T, List<TObject>> a)
            where TObject : ProtoMsg<TObject>, new()
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) =>
                {
                    var list = a(ref o);
                    w.Write(list.Count);

                    foreach (var item in list)
                    {
                        byte[] data = item.Serialize();
                        w.Write(data.Length);
                        w.Write(data);
                    }
                },
                (BinaryReader r, ref T o) =>
                {
                    int count = r.ReadInt32();
                    var list  = a(ref o);
                    list.Clear();

                    for (int i = 0; i < count; i++)
                    {
                        int    len  = r.ReadInt32();
                        byte[] data = r.ReadBytes(len);
                        list.Add(ProtoMsg<TObject>.Deserialize(data));
                    }
                }
            );
        }

        public static ProtoField<T> NullableObjectList<T, TObject>(
            ushort id,
            RefAccessor<T, List<TObject>?> a)
            where TObject : ProtoMsg<TObject>, new()
        {
            return new ProtoField<T>(
                id,
                (BinaryWriter w, ref T o) =>
                {
                    var list = a(ref o);
                    w.Write(list != null);
                    if (list == null)
                        return;

                    w.Write(list.Count);

                    foreach (var item in list)
                    {
                        byte[] data = item.Serialize();
                        w.Write(data.Length);
                        w.Write(data);
                    }
                },
                (BinaryReader r, ref T o) =>
                {
                    if (!r.ReadBoolean())
                    {
                        a(ref o) = null;
                        return;
                    }

                    int count = r.ReadInt32();
                    var list  = new List<TObject>(count);

                    for (int i = 0; i < count; i++)
                    {
                        int    len  = r.ReadInt32();
                        byte[] data = r.ReadBytes(len);
                        list.Add(ProtoMsg<TObject>.Deserialize(data));
                    }

                    a(ref o) = list;
                }
            );
        }
    }
}