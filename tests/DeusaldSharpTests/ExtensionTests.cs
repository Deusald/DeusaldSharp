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

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Local

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DeusaldSharp;
using NUnit.Framework;


namespace DeusaldSharpTests
{
    public class ExtensionTests
    {
        [Flags]
        private enum TestFlags
        {
            None = 0,
            A    = 1 << 0,
            B    = 1 << 1,
            C    = 1 << 2
        }

        /// <summary> Testing IsSingleFlagOn method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.IsSingleFlagOn))]
        public void IsSingleFlagOn_Base()
        {
            // Arrange
            TestFlags flags  = TestFlags.A;
            TestFlags flags2 = TestFlags.A | TestFlags.C;

            // Act & Assert
            Assert.That(flags.IsSingleFlagOn(), Is.True);
            Assert.That(flags2.IsSingleFlagOn(), Is.False);
        }

        /// <summary> Testing GetRandomFlag method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.GetRandomFlag))]
        public void GetRandomFlag_Base()
        {
            // Arrange
            TestFlags flags = TestFlags.A | TestFlags.B;

            // Act
            TestFlags result = flags.GetRandomFlag((min, max) => new Random().Next(min, max));

            // Assert
            Assert.That(result.IsSingleFlagOn(), Is.True);
            Assert.That(result == TestFlags.A || result == TestFlags.B, Is.True);
        }

        /// <summary> Testing HasAnyFlag method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.HasAnyFlag))]
        public void HasAnyFlag_Base()
        {
            // Arrange
            TestFlags flags = TestFlags.A | TestFlags.B;

            // Act & Assert
            Assert.That(flags.HasAnyFlag(TestFlags.A | TestFlags.C), Is.True);
        }
        
        /// <summary> Testing HasAllFlags method. </summary>
        [Test]
        [TestOf(nameof(EnumExtensions.HasAllFlags))]
        public void HasAllFlags_Base()
        {
            // Arrange
            TestFlags flags = TestFlags.A | TestFlags.B;

            // Act & Assert
            Assert.That(flags.HasAllFlags(TestFlags.A | TestFlags.B), Is.True);
            Assert.That(flags.HasAllFlags(TestFlags.A | TestFlags.C), Is.False);
        }

        [Test]
        [TestOf(nameof(ListExtensions.Shuffle))]
        public void Shuffle_KeepsAllElements()
        {
            List<int> list = Enumerable.Range(0, 1000).ToList();

            for (int x = 0; x < 20; ++x)
            {
                list.Shuffle();
                Assert.That(list, Is.EquivalentTo(Enumerable.Range(0, 1000)));
            }
        }

        [Test]
        [TestOf(nameof(ListExtensions.Shuffle))]
        public void Shuffle_EmptyAndSingleElementLists()
        {
            List<int> empty  = new List<int>();
            List<int> single = new List<int> { 42 };

            Assert.That(empty.Shuffle(),  Is.Empty);
            Assert.That(single.Shuffle(), Is.EqualTo(new[] { 42 }));
        }

        [Test]
        [TestOf(nameof(ListExtensions.Shuffle))]
        public void Shuffle_TwoElements_ProducesBothOrders()
        {
            bool seenOriginal = false;
            bool seenSwapped  = false;

            for (int x = 0; x < 200 && !(seenOriginal && seenSwapped); ++x)
            {
                IList<int> list = new List<int> { 1, 2 }.Shuffle();
                if (list[0] == 1) seenOriginal = true;
                else seenSwapped               = true;
            }

            Assert.That(seenOriginal && seenSwapped, Is.True);
        }

        [Test]
        [TestOf(nameof(ByteArrayExtensions.HexStringToByteArray))]
        public void HexStringToByteArray_ValidInput_RoundTrips()
        {
            byte[] bytes = { 0x00, 0x0F, 0xAB, 0xFF };

            Assert.That(bytes.ByteArrayToHexString().HexStringToByteArray(), Is.EqualTo(bytes));
            Assert.That("ABcd".HexStringToByteArray(),                         Is.EqualTo(new byte[] { 0xAB, 0xCD }));
            Assert.That("".HexStringToByteArray(),                             Is.Empty);
        }

        [TestCase("abc")]
        [TestCase("0")]
        [TestCase("zz")]
        [TestCase("0x")]
        [TestCase("-1")]
        [TestCase(" 1")]
        [TestOf(nameof(ByteArrayExtensions.HexStringToByteArray))]
        public void HexStringToByteArray_InvalidInput_ThrowsFormatException(string hex)
        {
            Assert.Throws<FormatException>(() => hex.HexStringToByteArray());
        }

        [Test]
        [TestOf(nameof(ZipExtensions.CreateEntryFromDirectory))]
        public void ZipExtensions_UsesForwardSlashesAndKeepsEmptyDirectories()
        {
            string root = Path.Combine(Path.GetTempPath(), "DeusaldSharpZipTest_" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(Path.Combine(root, "src", "sub"));
                Directory.CreateDirectory(Path.Combine(root, "src", "empty"));
                File.WriteAllText(Path.Combine(root, "src", "sub", "a.txt"), "hello");
                File.WriteAllText(Path.Combine(root, "src", "b.txt"),        "world");

                string zipPath = Path.Combine(root, "out.zip");
                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromDirectory(Path.Combine(root, "src"));
                    archive.CreateEntryFromAny(Path.Combine(root, "src"), "nested\\prefix");
                }

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    List<string> names = archive.Entries.Select(e => e.FullName).ToList();

                    Assert.That(names.Any(n => n.Contains('\\')), Is.False);
                    Assert.That(names, Is.EquivalentTo(new[]
                    {
                        "b.txt",
                        "empty/",
                        "sub/a.txt",
                        "nested/prefix/src/b.txt",
                        "nested/prefix/src/empty/",
                        "nested/prefix/src/sub/a.txt"
                    }));

                    using StreamReader reader = new StreamReader(archive.GetEntry("sub/a.txt")!.Open());
                    Assert.That(reader.ReadToEnd(), Is.EqualTo("hello"));
                }
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }
    }
}