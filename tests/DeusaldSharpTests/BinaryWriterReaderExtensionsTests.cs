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
using DeusaldSharp;
using NUnit.Framework;


// ReSharper disable UnusedMember.Local

namespace DeusaldSharpTests
{
    public class BinaryWriterReaderExtensionsTests
    {
        private static (BinaryWriter writer, MemoryStream ms) CreateWriter()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            return (bw, ms);
        }

        private static BinaryReader CreateReader(MemoryStream ms)
        {
            ms.Position = 0;
            return new BinaryReader(ms);
        }

        #region Primitive Lists

        [Test]
        public void BinaryWriterReaderExtensions_PrimitiveLists_Roundtrip_All()
        {
            // Arrange
            List<byte>   bytes   = new() { 0, 1, 2, 255 };
            List<sbyte>  sbytes  = new() { -128, -1, 0, 1, 127 };
            List<bool>   bools   = new() { true, false, true };
            List<short>  shorts  = new() { short.MinValue, -1, 0, 1, short.MaxValue };
            List<ushort> ushorts = new() { 0, 1, ushort.MaxValue };
            List<int>    ints    = new() { int.MinValue, -1, 0, 1, int.MaxValue };
            List<uint>   uints   = new() { 0U, 1U, uint.MaxValue };
            List<long>   longs   = new() { long.MinValue, -1L, 0L, 1L, long.MaxValue };
            List<ulong>  ulongs  = new() { 0UL, 1UL, ulong.MaxValue };
            List<float>  floats  = new() { -1.5f, 0f, 1.5f, float.Epsilon, float.MaxValue };
            List<double> doubles = new() { -1.5, 0.0, 1.5, double.Epsilon, double.MaxValue };
            List<char>   chars   = new() { 'a', 'Ż', '\0' };
            List<string> strings = new() { "", "hello", null, "żółw", "line\nbreak" };

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(bytes);
            bw.Write(sbytes);
            bw.Write(bools);
            bw.Write(shorts);
            bw.Write(ushorts);
            bw.Write(ints);
            bw.Write(uints);
            bw.Write(longs);
            bw.Write(ulongs);
            bw.Write(floats);
            bw.Write(doubles);
            bw.Write(chars);
            bw.Write(strings);
            bw.Flush();

            BinaryReader br = CreateReader(ms);

            List<byte>   rBytes   = br.ReadByteList();
            List<sbyte>  rSBytes  = br.ReadSByteList();
            List<bool>   rBools   = br.ReadBoolList();
            List<short>  rShorts  = br.ReadShortList();
            List<ushort> rUShorts = br.ReadUShortList();
            List<int>    rInts    = br.ReadIntList();
            List<uint>   rUInts   = br.ReadUIntList();
            List<long>   rLongs   = br.ReadLongList();
            List<ulong>  rULongs  = br.ReadULongList();
            List<float>  rFloats  = br.ReadFloatList();
            List<double> rDoubles = br.ReadDoubleList();
            List<char>   rChars   = br.ReadCharList();
            List<string> rStrings = br.ReadStringList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(rBytes, Is.EqualTo(bytes));
                Assert.That(rSBytes, Is.EqualTo(sbytes));
                Assert.That(rBools, Is.EqualTo(bools));
                Assert.That(rShorts, Is.EqualTo(shorts));
                Assert.That(rUShorts, Is.EqualTo(ushorts));
                Assert.That(rInts, Is.EqualTo(ints));
                Assert.That(rUInts, Is.EqualTo(uints));
                Assert.That(rLongs, Is.EqualTo(longs));
                Assert.That(rULongs, Is.EqualTo(ulongs));

                // floats/doubles: exact roundtrip through BinaryWriter/Reader
                Assert.That(rFloats, Is.EqualTo(floats));
                Assert.That(rDoubles, Is.EqualTo(doubles));

                Assert.That(rChars, Is.EqualTo(chars));

                // note: Write(List<string>) writes (v ?? string.Empty)
                List<string> expectedStrings = new() { "", "hello", "", "żółw", "line\nbreak" };
                Assert.That(rStrings, Is.EqualTo(expectedStrings));
            });
        }

        #endregion Primitive Lists

        #region Guid / DateTime / TimeSpan / Version

        [Test]
        public void BinaryWriterReaderExtensions_Guid_Roundtrip()
        {
            // Arrange
            Guid g = Guid.NewGuid();

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(g);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            Guid rg = br.ReadGuid();

            // Assert
            Assert.That(g, Is.EqualTo(rg));
        }

        [Test]
        public void BinaryWriterReaderExtensions_GuidList_Roundtrip()
        {
            // Arrange
            List<Guid> list = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.Empty };

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(list);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            List<Guid> r = br.ReadGuidList();

            // Assert
            Assert.That(r, Is.EqualTo(list));
        }

        [Test]
        public void BinaryWriterReaderExtensions_DateTime_Roundtrip()
        {
            // Arrange
            DateTime dt = DateTime.UtcNow;

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(dt);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            DateTime rdt = br.ReadDateTime();

            // Assert
            Assert.That(rdt, Is.EqualTo(dt));
        }

        [Test]
        public void BinaryWriterReaderExtensions_DateTimeList_Roundtrip()
        {
            // Arrange
            List<DateTime> list = new()
            {
                new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                DateTime.SpecifyKind(new DateTime(1999, 12, 31, 23, 59, 59), DateTimeKind.Local),
                DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Unspecified)
            };

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(list);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            List<DateTime> r = br.ReadDateTimeList();

            // Assert
            Assert.That(r, Is.EqualTo(list));
        }

        [Test]
        public void BinaryWriterReaderExtensions_TimeSpan_Roundtrip()
        {
            // Arrange
            TimeSpan ts = TimeSpan.FromDays(1.25) + TimeSpan.FromMilliseconds(123);

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(ts);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            TimeSpan rts = br.ReadTimeSpan();

            // Assert
            Assert.That(rts, Is.EqualTo(ts));
        }

        [Test]
        public void BinaryWriterReaderExtensions_TimeSpanList_Roundtrip()
        {
            // Arrange
            List<TimeSpan> list = new()
            {
                TimeSpan.Zero,
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromHours(123.456)
            };

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(list);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            List<TimeSpan> r = br.ReadTimeSpanList();

            // Assert
            Assert.That(r, Is.EqualTo(list));
        }

        [Test]
        public void BinaryWriterReaderExtensions_Version_Roundtrip()
        {
            // Arrange
            Version v = new Version(1, 2, 3);

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(v);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            Version rv = br.ReadVersion();

            // Assert
            Assert.That(rv, Is.EqualTo(v));
        }

        [Test]
        public void BinaryWriterReaderExtensions_VersionList_Roundtrip()
        {
            // Arrange
            List<Version> list = new() { new Version(1, 0, 0), new Version(2, 3, 4) };

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(list);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            List<Version> r = br.ReadVersionList();

            // Assert
            Assert.That(r, Is.EqualTo(list));
        }

        #endregion Guid / DateTime / TimeSpan / Version

        #region SerializableEnum

        [SerializableEnum(SerializableEnumType.Byte)]
        private enum TestByteEnum : byte
        {
            A = 1,
            Max = 255
        }

        [SerializableEnum(SerializableEnumType.SByte)]
        private enum TestSByteEnum : sbyte
        {
            Neg = -1,
            Min = sbyte.MinValue,
            Max = sbyte.MaxValue
        }

        [SerializableEnum(SerializableEnumType.Int)]
        private enum TestIntEnum
        {
            Min = int.MinValue,
            Neg = -123,
            Zero = 0,
            Pos = 123,
            Max = int.MaxValue
        }

        private enum NoAttributeEnum
        {
            A = 1
        }

        // note: wire type can be larger than enum storage size; reading may throw if the wire value doesn't fit.
        [SerializableEnum(SerializableEnumType.UInt)]
        private enum SmallStorageButUIntWire : byte
        {
            A = 1
        }
        
        [SerializableEnum(SerializableEnumType.Byte)]
        private enum IntBackedButByteWire
        {
            Ok  = 200,
            Bad = 300
        }

        [Test]
        public void BinaryWriterReaderExtensions_SerializableEnum_Roundtrip_SignedAndUnsigned()
        {
            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.WriteSerializableEnum(TestByteEnum.Max);
            bw.WriteSerializableEnum(TestSByteEnum.Neg);
            bw.WriteSerializableEnum(TestIntEnum.Min);
            bw.WriteSerializableEnum(TestIntEnum.Max);
            bw.Flush();

            BinaryReader br = CreateReader(ms);
            TestByteEnum r1 = br.ReadSerializableEnum<TestByteEnum>();
            TestSByteEnum r2 = br.ReadSerializableEnum<TestSByteEnum>();
            TestIntEnum  r3 = br.ReadSerializableEnum<TestIntEnum>();
            TestIntEnum  r4 = br.ReadSerializableEnum<TestIntEnum>();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(r1, Is.EqualTo(TestByteEnum.Max));
                Assert.That(r2, Is.EqualTo(TestSByteEnum.Neg));
                Assert.That(r3, Is.EqualTo(TestIntEnum.Min));
                Assert.That(r4, Is.EqualTo(TestIntEnum.Max));
            });
        }

        [Test]
        public void BinaryWriterReaderExtensions_SerializableEnumList_Roundtrip()
        {
            // Arrange
            List<TestIntEnum> list = new() { TestIntEnum.Neg, TestIntEnum.Zero, TestIntEnum.Pos };

            // Act
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.WriteSerializableEnumList(list);
            bw.Flush();

            BinaryReader br = CreateReader(ms);

            // NOTE: method name in extensions is ReadVersionList<TEnum>, but it actually reads a serializable-enum list.
            List<TestIntEnum> r = br.ReadSerializableEnumList<TestIntEnum>();

            // Assert
            Assert.That(r, Is.EqualTo(list));
        }

        [Test]
        public void BinaryWriterReaderExtensions_SerializableEnum_MissingAttribute_Throws()
        {
            // Arrange
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(1); // placeholder so stream isn't empty
            bw.Flush();

            BinaryReader br = CreateReader(ms);

            // Assert
            Assert.Throws<InvalidOperationException>(() => br.ReadSerializableEnum<NoAttributeEnum>());
        }

        [Test]
        public void SerializableEnumCache_Write_Overflow_ThrowsInvalidOperationException()
        {
            // Arrange: value 300 won't fit into Byte wire type
            (BinaryWriter bw, MemoryStream _) = CreateWriter();

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => bw.WriteSerializableEnum(IntBackedButByteWire.Bad));
        }

        [Test]
        public void SerializableEnumCache_Read_WireValueDoesNotFitEnumStorage_ThrowsInvalidOperationException()
        {
            // Arrange: enum storage is 1 byte but wire type is UInt; write uint=300 to the stream
            (BinaryWriter bw, MemoryStream ms) = CreateWriter();
            bw.Write(300U);
            bw.Flush();

            BinaryReader br = CreateReader(ms);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => br.ReadSerializableEnum<SmallStorageButUIntWire>());
        }

        #endregion SerializableEnum
    }
}
