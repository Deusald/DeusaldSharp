// MIT License
// DeusaldSharp:
// Copyright (c) 2020 Adam "Deusald" Orliński

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

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
                CollectionAssert.AreEqual(bytes, rBytes);
                CollectionAssert.AreEqual(sbytes, rSBytes);
                CollectionAssert.AreEqual(bools, rBools);
                CollectionAssert.AreEqual(shorts, rShorts);
                CollectionAssert.AreEqual(ushorts, rUShorts);
                CollectionAssert.AreEqual(ints, rInts);
                CollectionAssert.AreEqual(uints, rUInts);
                CollectionAssert.AreEqual(longs, rLongs);
                CollectionAssert.AreEqual(ulongs, rULongs);

                // floats/doubles: exact roundtrip through BinaryWriter/Reader
                CollectionAssert.AreEqual(floats, rFloats);
                CollectionAssert.AreEqual(doubles, rDoubles);

                CollectionAssert.AreEqual(chars, rChars);

                // note: Write(List<string>) writes (v ?? string.Empty)
                List<string> expectedStrings = new() { "", "hello", "", "żółw", "line\nbreak" };
                CollectionAssert.AreEqual(expectedStrings, rStrings);
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
            Assert.AreEqual(g, rg);
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
            CollectionAssert.AreEqual(list, r);
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
            Assert.AreEqual(dt, rdt);
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
            CollectionAssert.AreEqual(list, r);
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
            Assert.AreEqual(ts, rts);
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
            CollectionAssert.AreEqual(list, r);
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
            Assert.AreEqual(v, rv);
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
            CollectionAssert.AreEqual(list, r);
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
                Assert.AreEqual(TestByteEnum.Max, r1);
                Assert.AreEqual(TestSByteEnum.Neg, r2);
                Assert.AreEqual(TestIntEnum.Min, r3);
                Assert.AreEqual(TestIntEnum.Max, r4);
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
            CollectionAssert.AreEqual(list, r);
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
