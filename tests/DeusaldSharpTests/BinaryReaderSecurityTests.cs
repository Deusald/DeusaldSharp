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
    public class BinaryReaderSecurityTests
    {
        private const long _MAX_ALLOWED_ALLOCATION = 4 * 1024 * 1024;

        [SerializableEnum(SerializableEnumType.Int)]
        private enum SecurityTestEnum
        {
            A = 1
        }

        /// <summary>Stream wrapper that hides seeking, like a network stream.</summary>
        private sealed class NonSeekableStream : Stream
        {
            private readonly Stream _Inner;

            public NonSeekableStream(Stream inner)
            {
                _Inner = inner;
            }

            public override bool CanRead  => true;
            public override bool CanSeek  => false;
            public override bool CanWrite => false;
            public override long Length   => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override int  Read(byte[] buffer, int offset, int count)  => _Inner.Read(buffer, offset, count);
            public override void Flush()                                     { }
            public override long Seek(long offset, SeekOrigin origin)        => throw new NotSupportedException();
            public override void SetLength(long value)                       => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        private static IEnumerable<TestCaseData> AllCollectionReaders()
        {
            yield return Case("ByteList",              r => r.ReadByteList());
            yield return Case("SByteList",             r => r.ReadSByteList());
            yield return Case("BoolList",              r => r.ReadBoolList());
            yield return Case("ShortList",             r => r.ReadShortList());
            yield return Case("UShortList",            r => r.ReadUShortList());
            yield return Case("IntList",               r => r.ReadIntList());
            yield return Case("UIntList",              r => r.ReadUIntList());
            yield return Case("LongList",              r => r.ReadLongList());
            yield return Case("ULongList",             r => r.ReadULongList());
            yield return Case("FloatList",             r => r.ReadFloatList());
            yield return Case("DoubleList",            r => r.ReadDoubleList());
            yield return Case("CharList",              r => r.ReadCharList());
            yield return Case("StringList",            r => r.ReadStringList());
            yield return Case("ByteArray",             r => r.ReadByteArray());
            yield return Case("SByteArray",            r => r.ReadSByteArray());
            yield return Case("BoolArray",             r => r.ReadBoolArray());
            yield return Case("ShortArray",            r => r.ReadShortArray());
            yield return Case("UShortArray",           r => r.ReadUShortArray());
            yield return Case("IntArray",              r => r.ReadIntArray());
            yield return Case("UIntArray",             r => r.ReadUIntArray());
            yield return Case("LongArray",             r => r.ReadLongArray());
            yield return Case("ULongArray",            r => r.ReadULongArray());
            yield return Case("FloatArray",            r => r.ReadFloatArray());
            yield return Case("DoubleArray",           r => r.ReadDoubleArray());
            yield return Case("CharArray",             r => r.ReadCharArray());
            yield return Case("StringArray",           r => r.ReadStringArray());
            yield return Case("GuidList",              r => r.ReadGuidList());
            yield return Case("GuidArray",             r => r.ReadGuidArray());
            yield return Case("DateTimeList",          r => r.ReadDateTimeList());
            yield return Case("DateTimeArray",         r => r.ReadDateTimeArray());
            yield return Case("TimeSpanList",          r => r.ReadTimeSpanList());
            yield return Case("TimeSpanArray",         r => r.ReadTimeSpanArray());
            yield return Case("VersionList",           r => r.ReadVersionList());
            yield return Case("VersionArray",          r => r.ReadVersionArray());
            yield return Case("SerializableEnumList",  r => r.ReadSerializableEnumList<SecurityTestEnum>());
            yield return Case("SerializableEnumArray", r => r.ReadSerializableEnumArray<SecurityTestEnum>());
            yield return Case("HttpStatusCodeList",    r => r.ReadHttpStatusCodeList());
            yield return Case("HttpStatusCodeArray",   r => r.ReadHttpStatusCodeArray());
        }

        private static TestCaseData Case(string name, Func<BinaryReader, object> read)
        {
            return new TestCaseData(read).SetArgDisplayNames(name);
        }

        private static byte[] Payload(int count, int extraBytes)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(count);
            bw.Write(new byte[extraBytes]);
            bw.Flush();
            return ms.ToArray();
        }

        [TestCaseSource(nameof(AllCollectionReaders))]
        public void NegativeCount_ThrowsInvalidData(Func<BinaryReader, object> read)
        {
            BinaryReader seekable    = new BinaryReader(new MemoryStream(Payload(-1, 8)));
            BinaryReader nonSeekable = new BinaryReader(new NonSeekableStream(new MemoryStream(Payload(-1, 8))));

            Assert.Throws<InvalidDataException>(() => read(seekable));
            Assert.Throws<InvalidDataException>(() => read(nonSeekable));
        }

        [TestCaseSource(nameof(AllCollectionReaders))]
        public void HugeCountWithTinyPayload_Seekable_ThrowsInvalidDataWithoutAllocating(Func<BinaryReader, object> read)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(Payload(int.MaxValue, 8)));

            long before = GC.GetAllocatedBytesForCurrentThread();
            Assert.Throws<InvalidDataException>(() => read(reader));
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.That(allocated, Is.LessThan(_MAX_ALLOWED_ALLOCATION));
        }

        [TestCaseSource(nameof(AllCollectionReaders))]
        public void HugeCountWithTinyPayload_NonSeekable_ThrowsWithoutAllocating(Func<BinaryReader, object> read)
        {
            BinaryReader reader = new BinaryReader(new NonSeekableStream(new MemoryStream(Payload(int.MaxValue, 8))));

            long      before    = GC.GetAllocatedBytesForCurrentThread();
            Exception exception = Assert.Catch(() => read(reader))!;
            long      allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.That(exception, Is.InstanceOf<EndOfStreamException>().Or.InstanceOf<InvalidDataException>());
            Assert.That(allocated, Is.LessThan(_MAX_ALLOWED_ALLOCATION));
        }

        [Test]
        public void CountMatchingPayload_Seekable_ReadsExactly()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.WriteArray(new[] { 1, 2, 3 });
            bw.Write(new List<long> { 4, 5 });
            bw.Write(new List<Guid> { Guid.NewGuid() });
            bw.WriteArray(Array.Empty<string>());
            bw.Flush();

            ms.Position = 0;
            BinaryReader br = new BinaryReader(ms);
            Assert.That(br.ReadIntArray(),        Is.EqualTo(new[] { 1, 2, 3 }));
            Assert.That(br.ReadLongList(),        Is.EqualTo(new List<long> { 4, 5 }));
            Assert.That(br.ReadGuidList().Count,  Is.EqualTo(1));
            Assert.That(br.ReadStringArray(),     Is.Empty);
            Assert.That(ms.Position,              Is.EqualTo(ms.Length));
        }

        [Test]
        public void ValidPayload_NonSeekable_RoundTrips()
        {
            int[] big = new int[5000];
            for (int x = 0; x < big.Length; ++x) big[x] = x * 7;

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.WriteArray(big);
            bw.Write(new List<string> { "a", "bc" });
            bw.Flush();

            BinaryReader br = new BinaryReader(new NonSeekableStream(new MemoryStream(ms.ToArray())));
            Assert.That(br.ReadIntArray(),   Is.EqualTo(big));
            Assert.That(br.ReadStringList(), Is.EqualTo(new List<string> { "a", "bc" }));
        }
    }
}
