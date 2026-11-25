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
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Global

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeusaldSharp;
using JetBrains.Annotations;
using NUnit.Framework;

namespace DeusaldSharpTests.ProtoNamespaceA
{
    [PublicAPI]
    public partial class SameNameMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int A;
    }
}

namespace DeusaldSharpTests.ProtoNamespaceB
{
    [PublicAPI]
    public partial class SameNameMsg : ProtoMsgBase
    {
        [ProtoField(1)] public string B = "";
    }
}

namespace DeusaldSharpTests
{
    // ------------------------------------------------------------
    // Messages for regression tests
    // ------------------------------------------------------------

    [PublicAPI]
    public partial class RecursiveMsg : ProtoMsgBase
    {
        [ProtoField(1)] public RecursiveMsg? Next;
    }

    [PublicAPI]
    public partial class ChildListMsg : ProtoMsgBase
    {
        [ProtoField(1)] public List<ChildMsg> Items = new();
        [ProtoField(2)] public ChildMsg[]     Array = System.Array.Empty<ChildMsg>();
    }

    // Split across two partial declarations (and a third one in ProtoSplitMsgPart.cs).
    [PublicAPI]
    public partial class SplitMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int A;
    }

    public partial class SplitMsg
    {
        [ProtoField(2)] public int B;
    }

    [PublicAPI]
    public abstract partial class AbstractBaseMsg : ProtoMsgBase
    {
        [ProtoField(1)] protected int  BaseValue;
        [ProtoField(2)] public    string BaseName = "";

        public int GetBaseValue()           => BaseValue;
        public void SetBaseValue(int value) => BaseValue = value;
    }

    [PublicAPI]
    public partial class ConcreteBaseMsg : AbstractBaseMsg
    {
        [ProtoField(3)] public int Middle;
    }

    [PublicAPI]
    public partial class DerivedMsg : ConcreteBaseMsg
    {
        [ProtoField(4)] public int Leaf;
    }

    [PublicAPI]
    public partial class OuterHolder
    {
        public partial class InnerMsg : ProtoMsgBase
        {
            [ProtoField(1)] public int Value;
        }
    }

    [PublicAPI]
    public partial class GenericOuter<T>
    {
        public partial class InnerMsg : ProtoMsgBase
        {
            [ProtoField(1)] public int Value;
        }
    }

    internal partial class InternalMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int Value;
    }

    [PublicAPI]
    public partial class KeywordMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int    @class;
        [ProtoField(2)] public string @string = "";
    }

    [PublicAPI]
    public partial class NullableSpecialMsg : ProtoMsgBase
    {
        [ProtoField(1)] public string?  Name;
        [ProtoField(2)] public Version? Version;
        [ProtoField(3)] public int      After;
    }

    [PublicAPI]
    public partial class ResetMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int       A;
        [ProtoField(2)] public string?   Name;
        [ProtoField(3)] public ChildMsg? Child;
        [ProtoField(4)] public int       Hp   = 100;
        [ProtoField(5)] public List<int> Ints = new();
    }

    [PublicAPI]
    public partial class PropertyMsg : ProtoMsgBase
    {
        [field: ProtoField(1)] public int Value { get; set; }
    }

    // ------------------------------------------------------------
    // Tests
    // ------------------------------------------------------------

    public class ProtoSecurityTests
    {
        private static byte[] Bytes(Action<BinaryWriter> write)
        {
            using MemoryStream ms = new MemoryStream();
            using BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
            write(bw);
            bw.Flush();
            return ms.ToArray();
        }

        /// <summary> RecursiveMsg chain with <paramref name="levels"/> nested messages below the root. </summary>
        private static byte[] RecursiveChain(int levels)
            => Bytes(w =>
            {
                for (int x = 0; x < levels; ++x)
                {
                    int remainingLevels = levels - x;
                    w.Write((ushort)1);
                    w.Write(remainingLevels * 7 - 6);
                    w.Write(true);
                }
            });

        private static void AssertCompletesQuickly(Action action)
        {
            Task task = Task.Run(action);
            Assert.That(task.Wait(TimeSpan.FromSeconds(5)), Is.True, "Deserialization did not finish (infinite loop?).");
        }

        // ---------------- malformed input ----------------

        [Test]
        public void Proto_NegativeFieldLength_Throws_DoesNotLoop()
        {
            byte[] data = Bytes(w =>
            {
                w.Write((ushort)99);
                w.Write(-6);
            });

            AssertCompletesQuickly(() => Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<ChildMsg>(data)));
            AssertCompletesQuickly(() => Assert.Throws<InvalidDataException>(() => new ChildMsg().Deserialize(data)));
        }

        [Test]
        public void Proto_FieldLengthPastEnd_Throws()
        {
            byte[] data = Bytes(w =>
            {
                w.Write((ushort)99);
                w.Write(int.MaxValue);
                w.Write((byte)1);
            });

            Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<ChildMsg>(data));
        }

        [Test]
        public void Proto_TruncatedPayload_Throws()
        {
            byte[] full = new RootMsg { S = "hello", I = 5 }.Serialize();

            for (int cut = 1; cut < 7; ++cut) // the last field (NArrGuids = null) is 7 bytes long
            {
                byte[] truncated = full.Take(full.Length - cut).ToArray();
                Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<RootMsg>(truncated), $"cut {cut}");
            }
        }

        [Test]
        public void Proto_TruncatedFieldHeader_Throws()
        {
            byte[] data = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write((byte)0);
            });

            Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<ChildMsg>(data));
        }

        [Test]
        public void Proto_KnownFieldReadingPastItsLength_Throws()
        {
            // Field 1 (int X) declares 2 bytes but int needs 4: the read must not consume the next field.
            byte[] data = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(2);
                w.Write((short)7);
                w.Write((ushort)2);
                w.Write(1);
                w.Write(true);
            });

            Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<ChildMsg>(data));
        }

        [Test]
        public void Proto_DeepNesting_Throws_WithoutStackOverflow()
        {
            byte[] data = RecursiveChain(10000);

            InvalidDataException exception = Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<RecursiveMsg>(data))!;
            Assert.That(exception.Message, Does.Contain("depth"));
        }

        [Test]
        public void Proto_Nesting_UpToMaxDepth_Works()
        {
            // Root counts as level 1: MaxDepth - 1 nested messages is the deepest accepted chain.
            RecursiveMsg msg = ProtoMsgBase.Deserialize<RecursiveMsg>(RecursiveChain(ProtoMsgBase.MaxDepth - 1));

            int levels = 0;
            for (RecursiveMsg? current = msg.Next; current is not null; current = current.Next) ++levels;
            Assert.That(levels, Is.EqualTo(ProtoMsgBase.MaxDepth - 1));

            Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<RecursiveMsg>(RecursiveChain(ProtoMsgBase.MaxDepth)));
        }

        [Test]
        public void Proto_MaxDepth_IsConfigurable_AndRoundtripUsesSameLimit()
        {
            int previous = ProtoMsgBase.MaxDepth;

            try
            {
                ProtoMsgBase.MaxDepth = 4;

                RecursiveMsg ok = new RecursiveMsg { Next = new RecursiveMsg { Next = new RecursiveMsg { Next = new RecursiveMsg() } } };
                Assert.DoesNotThrow(() => ProtoMsgBase.Deserialize<RecursiveMsg>(ok.Serialize()));

                RecursiveMsg tooDeep = new RecursiveMsg { Next = ok };
                Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<RecursiveMsg>(tooDeep.Serialize()));
                Assert.Throws<ArgumentOutOfRangeException>(() => ProtoMsgBase.MaxDepth = 0);
            }
            finally
            {
                ProtoMsgBase.MaxDepth = previous;
            }
        }

        [Test]
        public void Proto_HugeCollectionCount_Throws_WithoutLargeAllocation()
        {
            byte[] listCount = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(4);
                w.Write(int.MaxValue);
            });

            byte[] arrayCount = Bytes(w =>
            {
                w.Write((ushort)2);
                w.Write(4);
                w.Write(100_000_000);
            });

            byte[] negativeCount = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(4);
                w.Write(-1);
            });

            byte[] itemLength = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(9);
                w.Write(1);
                w.Write(true);
                w.Write(int.MaxValue);
            });

            byte[] negativeItemLength = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(9);
                w.Write(1);
                w.Write(true);
                w.Write(-5);
            });

            foreach (byte[] data in new[] { listCount, arrayCount, negativeCount, itemLength, negativeItemLength })
            {
                long before = GC.GetAllocatedBytesForCurrentThread();
                Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<ChildListMsg>(data));
                long allocated = GC.GetAllocatedBytesForCurrentThread() - before;
                Assert.That(allocated, Is.LessThan(256 * 1024));
            }
        }

        [Test]
        public void Proto_HugeNestedLength_Throws_WithoutLargeAllocation()
        {
            byte[] data = Bytes(w =>
            {
                w.Write((ushort)22); // RootMsg.NChild
                w.Write(int.MaxValue - 16);
                w.Write(true);
            });

            long before = GC.GetAllocatedBytesForCurrentThread();
            Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<RootMsg>(data));
            Assert.That(GC.GetAllocatedBytesForCurrentThread() - before, Is.LessThan(256 * 1024));
        }

        [Test]
        public void Proto_DecodingFailure_IsWrapped_WithInnerException()
        {
            // Field 3 (string S) with an invalid 7-bit length prefix.
            byte[] data = Bytes(w =>
            {
                w.Write((ushort)3);
                w.Write(5);
                w.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            });

            InvalidDataException exception = Assert.Throws<InvalidDataException>(() => ProtoMsgBase.Deserialize<RootMsg>(data))!;
            Assert.That(exception.InnerException, Is.Not.Null);
        }

        // ---------------- generator correctness ----------------

        [Test]
        public void Proto_WireFormat_IsUnchanged()
        {
            byte[] expected = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(4);
                w.Write(5);
                w.Write((ushort)2);
                w.Write(1);
                w.Write(true);
            });

            Assert.That(new ChildMsg { X = 5, Flag = true }.Serialize(), Is.EqualTo(expected));
        }

        [Test]
        public void Proto_SameNameInDifferentNamespaces_BothWork()
        {
            ProtoNamespaceA.SameNameMsg a = ProtoMsgBase.Deserialize<ProtoNamespaceA.SameNameMsg>(new ProtoNamespaceA.SameNameMsg { A = 7 }.Serialize());
            ProtoNamespaceB.SameNameMsg b = ProtoMsgBase.Deserialize<ProtoNamespaceB.SameNameMsg>(new ProtoNamespaceB.SameNameMsg { B = "b" }.Serialize());

            Assert.That(a.A, Is.EqualTo(7));
            Assert.That(b.B, Is.EqualTo("b"));
        }

        [Test]
        public void Proto_PartialSplitMessage_Works()
        {
            SplitMsg read = ProtoMsgBase.Deserialize<SplitMsg>(new SplitMsg { A = 1, B = 2, C = 3 }.Serialize());

            Assert.That(read.A, Is.EqualTo(1));
            Assert.That(read.B, Is.EqualTo(2));
            Assert.That(read.C, Is.EqualTo(3));
        }

        [Test]
        public void Proto_Inheritance_BaseFieldsRoundtrip()
        {
            DerivedMsg msg = new DerivedMsg { BaseName = "base", Middle = 2, Leaf = 3 };
            msg.SetBaseValue(1);

            DerivedMsg read = ProtoMsgBase.Deserialize<DerivedMsg>(msg.Serialize());

            Assert.That(read.GetBaseValue(), Is.EqualTo(1));
            Assert.That(read.BaseName,       Is.EqualTo("base"));
            Assert.That(read.Middle,         Is.EqualTo(2));
            Assert.That(read.Leaf,           Is.EqualTo(3));

            // The concrete base reads the shared fields and skips the derived one.
            ConcreteBaseMsg asBase = ProtoMsgBase.Deserialize<ConcreteBaseMsg>(msg.Serialize());
            Assert.That(asBase.GetBaseValue(), Is.EqualTo(1));
            Assert.That(asBase.Middle,         Is.EqualTo(2));
        }

        [Test]
        public void Proto_NestedTypeMessages_Work()
        {
            OuterHolder.InnerMsg      a = ProtoMsgBase.Deserialize<OuterHolder.InnerMsg>(new OuterHolder.InnerMsg { Value = 4 }.Serialize());
            GenericOuter<int>.InnerMsg b = ProtoMsgBase.Deserialize<GenericOuter<int>.InnerMsg>(new GenericOuter<int>.InnerMsg { Value = 5 }.Serialize());

            Assert.That(a.Value, Is.EqualTo(4));
            Assert.That(b.Value, Is.EqualTo(5));
        }

        [Test]
        public void Proto_InternalMessage_Works()
        {
            Assert.That(ProtoMsgBase.Deserialize<InternalMsg>(new InternalMsg { Value = 6 }.Serialize()).Value, Is.EqualTo(6));
        }

        [Test]
        public void Proto_KeywordFieldNames_Work()
        {
            KeywordMsg read = ProtoMsgBase.Deserialize<KeywordMsg>(new KeywordMsg { @class = 8, @string = "s" }.Serialize());

            Assert.That(read.@class,  Is.EqualTo(8));
            Assert.That(read.@string, Is.EqualTo("s"));
        }

        [Test]
        public void Proto_PropertyBackingField_Works()
        {
            Assert.That(ProtoMsgBase.Deserialize<PropertyMsg>(new PropertyMsg { Value = 9 }.Serialize()).Value, Is.EqualTo(9));
        }

        [Test]
        public void Proto_NullableStringAndVersion_NullAndValue()
        {
            NullableSpecialMsg nulls = ProtoMsgBase.Deserialize<NullableSpecialMsg>(new NullableSpecialMsg { After = 1 }.Serialize());

            Assert.That(nulls.Name,    Is.Null);
            Assert.That(nulls.Version, Is.Null);
            Assert.That(nulls.After,   Is.EqualTo(1));

            NullableSpecialMsg values = ProtoMsgBase.Deserialize<NullableSpecialMsg>(
                new NullableSpecialMsg { Name = "n", Version = new Version(1, 2, 3), After = 2 }.Serialize());

            Assert.That(values.Name,    Is.EqualTo("n"));
            Assert.That(values.Version, Is.EqualTo(new Version(1, 2, 3)));
            Assert.That(values.After,   Is.EqualTo(2));
        }

        [Test]
        public void Proto_NonNullableStringSetToNull_SerializesWithoutThrowing()
        {
            StringOnlyMsg read = ProtoMsgBase.Deserialize<StringOnlyMsg>(new StringOnlyMsg { S = null! }.Serialize());
            Assert.That(read.S, Is.EqualTo(""));
        }

        [Test]
        public void Proto_ReusedInstance_ResetsAbsentFields()
        {
            ResetMsg full = new ResetMsg { A = 1, Name = "name", Child = new ChildMsg { X = 1 }, Hp = 5, Ints = new List<int> { 1, 2 } };
            byte[]   onlyA = Bytes(w =>
            {
                w.Write((ushort)1);
                w.Write(4);
                w.Write(42);
            });

            ResetMsg msg = new ResetMsg();
            msg.Deserialize(full.Serialize());
            Assert.That(msg.Name, Is.EqualTo("name"));

            msg.Deserialize(onlyA);

            Assert.That(msg.A,     Is.EqualTo(42));
            Assert.That(msg.Name,  Is.Null);
            Assert.That(msg.Child, Is.Null);
            Assert.That(msg.Hp,    Is.EqualTo(100));
            Assert.That(msg.Ints,  Is.Empty);
        }
    }
}
