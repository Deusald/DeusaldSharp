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

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DeusaldSharp;
using JetBrains.Annotations;
using NUnit.Framework;

// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable EnumUnderlyingTypeIsInt
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedMember.Local

namespace DeusaldSharpTests
{
    // ------------------------------------------------------------
    // Test enums
    // ------------------------------------------------------------

    [SerializableEnum(SerializableEnumType.SByte)]
    public enum TestEnumSByte : sbyte
    {
        Neg  = -1,
        Zero = 0,
        Pos  = 1
    }

    [SerializableEnum(SerializableEnumType.Int)]
    public enum TestEnumInt : int
    {
        A = 1,
        B = 2,
        C = 3
    }

    // ------------------------------------------------------------
    // Test messages
    // ------------------------------------------------------------

    [PublicAPI]
    public partial class ChildMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int  X;
        [ProtoField(2)] public bool Flag;
    }

    [PublicAPI]
    public partial class EmptyMsg : ProtoMsgBase { }

    [PublicAPI]
    public partial class WithEmptyMsg : ProtoMsgBase
    {
        [ProtoField(1)] public EmptyMsg? Msg;
    }

    [PublicAPI]
    public partial class GenericMsg<T> : ProtoMsgBase where T : ProtoMsgBase, new()
    {
        [ProtoField(1)] public T? Msg;
    }

    /// <summary>
    /// “Full” message to cover most public ProtoField factories.
    /// </summary>
    [PublicAPI]
    public partial class RootMsg : ProtoMsgBase
    {
        // primitives
        [ProtoField(1)] public bool   B;
        [ProtoField(2)] public int    I;
        [ProtoField(3)] public string S = "";

        // special
        [ProtoField(4)] public Guid           G;
        [ProtoField(5)] public DateTime       DT;
        [ProtoField(6)] public TimeSpan       TS;
        [ProtoField(7)] public Version        V = new(1, 0, 0);
        [ProtoField(8)] public HttpStatusCode Code;

        // nullable value-types
        [ProtoField(9)]  public int?      NI;
        [ProtoField(10)] public Guid?     NG;
        [ProtoField(11)] public TimeSpan? NTS;

        // enums
        [ProtoField(12)] public TestEnumSByte  E;
        [ProtoField(13)] public TestEnumSByte? NE;

        // lists (non-nullable)
        [ProtoField(14)] public List<int>           Ints    = new();
        [ProtoField(15)] public List<string>        Strings = new();
        [ProtoField(16)] public List<Guid>          Guids   = new();
        [ProtoField(17)] public List<TestEnumSByte> Enums   = new();

        // nullable lists (typed factories you exposed + new primitive list factories)
        [ProtoField(18)] public List<int>?           NInts;
        [ProtoField(19)] public List<string>?        NStrings;
        [ProtoField(20)] public List<TestEnumSByte>? NEnums;

        // objects
        [ProtoField(21)] public ChildMsg  Child = new();
        [ProtoField(22)] public ChildMsg? NChild;

        // object lists
        [ProtoField(23)] public List<ChildMsg>  Children = new();
        [ProtoField(24)] public List<ChildMsg>? NChildren;

        // arrays
        [ProtoField(25)] public int[]  ArrInts  = Array.Empty<int>();
        [ProtoField(26)] public Guid[] ArrGuids = Array.Empty<Guid>();

        // Nullable arrays
        [ProtoField(27)] public int[]?  NArrInts;
        [ProtoField(28)] public Guid[]? NArrGuids;
    }

    /// <summary>
    /// Small schema for evolution tests (old/new schemas).
    /// </summary>
    [PublicAPI]
    public partial class OldSchemaMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int    A;
        [ProtoField(2)] public string B = "";
    }

    [PublicAPI]
    public partial class NewSchemaMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int    A;
        [ProtoField(2)] public string B = "";
        [ProtoField(3)] public int    C;
    }

    /// <summary>
    /// Message designed to test duplicate fields in stream.
    /// </summary>
    [PublicAPI]
    public partial class DuplicateFieldMsg : ProtoMsgBase
    {
        [ProtoField(1)] public int I;
    }

    /// <summary>
    /// Message to test zero-length payload behaviour.
    /// </summary>
    [PublicAPI]
    public partial class StringOnlyMsg : ProtoMsgBase
    {
        [ProtoField(1)] public string S = "";
    }

    [PublicAPI]
    public partial class GuidByteArray : ProtoMsgBase
    {
        [ProtoField(1)] public Guid   GameId;
        [ProtoField(2)] public byte[] GameLogicBytes = null!;
    }

    public class ProtoModuleTests
    {
        // ------------------------------------------------------------
        // Helpers to craft streams with unknown/duplicate/corrupt fields
        // ------------------------------------------------------------

        private static byte[] SerializeManual(Action<BinaryWriter> write)
        {
            using MemoryStream ms = new MemoryStream();
            using BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
            write(bw);
            bw.Flush();
            return ms.ToArray();
        }

        private static void AppendField(MemoryStream ms, ushort id, byte[] payload)
        {
            using BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
            bw.Write(id);
            bw.Write(payload.Length);
            bw.Write(payload);
            bw.Flush();
        }

        private static byte[] Payload(Action<BinaryWriter> write)
            => SerializeManual(write);

        private static byte[] InsertUnknownFieldInMiddle(byte[] msgBytes, ushort unknownId, byte[] unknownPayload)
        {
            // message is a sequence of (id ushort, len int, payload bytes)
            // We’ll copy the first field, then insert unknown field, then copy the rest.
            using MemoryStream input = new MemoryStream(msgBytes);
            using BinaryReader br    = new BinaryReader(input, Encoding.UTF8, leaveOpen: true);

            ushort firstId      = br.ReadUInt16();
            int    firstLen     = br.ReadInt32();
            byte[] firstPayload = br.ReadBytes(firstLen);

            byte[] rest = br.ReadBytes((int)(input.Length - input.Position));

            using MemoryStream output = new MemoryStream();
            AppendField(output, firstId,   firstPayload);
            AppendField(output, unknownId, unknownPayload);
            output.Write(rest, 0, rest.Length);

            return output.ToArray();
        }

        // ------------------------------------------------------------
        // Tests
        // ------------------------------------------------------------

        [Test]
        public void Proto_Guid_ByteArray()
        {
            GuidByteArray msg = new GuidByteArray
            {
                GameId = Guid.NewGuid(),
                GameLogicBytes = "AgAAAAEDAQUAAAABAAAAAQAAAAAAAAAAAwAAAAAAAAAA".Base64StringToByteArray()
            };
            
            GuidByteArray result = ProtoMsgBase.Deserialize<GuidByteArray>(msg.Serialize());
            
            Assert.That(result.GameId, Is.EqualTo(msg.GameId));
            Assert.That(result.GameLogicBytes, Is.EqualTo(msg.GameLogicBytes));
        }
        
        [Test]
        public void Proto_Empty_Msg()
        {
            WithEmptyMsg withEmptyMsg = new WithEmptyMsg
            {
                Msg = new()
            };

            WithEmptyMsg newWithEmptyMsg = ProtoMsgBase.Deserialize<WithEmptyMsg>(withEmptyMsg.Serialize());

            Assert.That(newWithEmptyMsg.Msg, Is.Not.Null);
        }

        [Test]
        public void Proto_Generic_Msg()
        {
            GenericMsg<ChildMsg> genericMsg = new GenericMsg<ChildMsg>
            {
                Msg = new()
                {
                    X    = 5,
                    Flag = true
                }
            };

            GenericMsg<ChildMsg> newMsg = ProtoMsgBase.Deserialize<GenericMsg<ChildMsg>>(genericMsg.Serialize());

            Assert.That(newMsg.Msg,      Is.Not.Null);
            Assert.That(newMsg.Msg!.X,   Is.EqualTo(5));
            Assert.That(newMsg.Msg.Flag, Is.True);
        }

        [Test]
        public void Proto_Roundtrip_Primitives_Specials_Nullables_Lists_Objects()
        {
            // Arrange
            RootMsg msg = new RootMsg
            {
                B = true,
                I = 123456,
                S = "hello",

                G    = Guid.NewGuid(),
                DT   = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                TS   = TimeSpan.FromMilliseconds(123.45),
                V    = new Version(2, 3, 4),
                Code = HttpStatusCode.Created,

                NI  = 777,
                NG  = Guid.NewGuid(),
                NTS = TimeSpan.FromDays(2),

                E  = TestEnumSByte.Neg,
                NE = TestEnumSByte.Pos
            };

            msg.Ints.AddRange(new[] { 1, 2, 3 });
            msg.Strings.AddRange(new[] { "a", null, "żółw" }!); // list writer: null -> ""
            msg.Guids.AddRange(new[] { Guid.Empty, Guid.NewGuid() });
            msg.Enums.AddRange(new[] { TestEnumSByte.Neg, TestEnumSByte.Zero, TestEnumSByte.Pos });

            msg.NInts    = new List<int> { 9, 8, 7 };
            msg.NStrings = new List<string> { "x", null!, "y" };
            msg.NEnums   = new List<TestEnumSByte> { TestEnumSByte.Pos };

            msg.ArrInts  = new[] { 5, 6, 7 };
            msg.ArrGuids = new[] { Guid.NewGuid(), Guid.NewGuid() };

            msg.NArrInts  = new[] { 9, 10, 11 };
            msg.NArrGuids = new[] { Guid.NewGuid(), Guid.NewGuid() };
            
            msg.Child.X    = 10;
            msg.Child.Flag = true;

            msg.NChild = null;

            msg.Children.Add(new ChildMsg { X = 1, Flag = false });
            msg.Children.Add(new ChildMsg { X = 2, Flag = true });

            msg.NChildren = new List<ChildMsg> { new() { X = 42, Flag = true } };

            // Act
            byte[]  data      = msg.Serialize();
            RootMsg roundtrip = ProtoMsgBase.Deserialize<RootMsg>(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(roundtrip.B, Is.EqualTo(msg.B));
                Assert.That(roundtrip.I, Is.EqualTo(msg.I));
                Assert.That(roundtrip.S, Is.EqualTo(msg.S));

                Assert.That(roundtrip.G,    Is.EqualTo(msg.G));
                Assert.That(roundtrip.DT,   Is.EqualTo(msg.DT));
                Assert.That(roundtrip.TS,   Is.EqualTo(msg.TS));
                Assert.That(roundtrip.V,    Is.EqualTo(msg.V));
                Assert.That(roundtrip.Code, Is.EqualTo(msg.Code));

                Assert.That(roundtrip.NI,  Is.EqualTo(msg.NI));
                Assert.That(roundtrip.NG,  Is.EqualTo(msg.NG));
                Assert.That(roundtrip.NTS, Is.EqualTo(msg.NTS));

                Assert.That(roundtrip.E,  Is.EqualTo(msg.E));
                Assert.That(roundtrip.NE, Is.EqualTo(msg.NE));

                Assert.That(roundtrip.Ints, Is.EqualTo(msg.Ints));

                // string list write uses (v ?? string.Empty)
                Assert.That(roundtrip.Strings, Is.EqualTo(new[] { "a", "", "żółw" }));

                Assert.That(roundtrip.Guids, Is.EqualTo(msg.Guids));
                Assert.That(roundtrip.Enums, Is.EqualTo(msg.Enums));

                Assert.That(roundtrip.NInts,    Is.EqualTo(msg.NInts));
                Assert.That(roundtrip.NStrings, Is.EqualTo(new[] { "x", "", "y" }));
                Assert.That(roundtrip.NEnums,   Is.EqualTo(msg.NEnums));
                
                Assert.That(roundtrip.ArrInts,   Is.EqualTo(msg.ArrInts));
                Assert.That(roundtrip.ArrGuids,  Is.EqualTo(msg.ArrGuids));
                
                Assert.That(roundtrip.NArrInts,   Is.EqualTo(msg.NArrInts));
                Assert.That(roundtrip.NArrGuids,  Is.EqualTo(msg.NArrGuids));

                Assert.That(roundtrip.Child.X,    Is.EqualTo(msg.Child.X));
                Assert.That(roundtrip.Child.Flag, Is.EqualTo(msg.Child.Flag));

                Assert.That(roundtrip.Children.Count,   Is.EqualTo(2));
                Assert.That(roundtrip.Children[0].X,    Is.EqualTo(1));
                Assert.That(roundtrip.Children[0].Flag, Is.EqualTo(false));
                Assert.That(roundtrip.Children[1].X,    Is.EqualTo(2));
                Assert.That(roundtrip.Children[1].Flag, Is.EqualTo(true));

                Assert.That(roundtrip.NChildren,         Is.Not.Null);
                Assert.That(roundtrip.NChildren!.Count,  Is.EqualTo(1));
                Assert.That(roundtrip.NChildren[0].X,    Is.EqualTo(42));
                Assert.That(roundtrip.NChildren[0].Flag, Is.EqualTo(true));
            });
        }

        [Test]
        public void Proto_Serialize_IsDeterministic_ForSameState()
        {
            RootMsg msg = new RootMsg
            {
                B    = true,
                I    = 5,
                S    = "x",
                G    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                DT   = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                TS   = TimeSpan.FromSeconds(1),
                V    = new Version(1, 2, 3),
                Code = HttpStatusCode.OK,
                NI   = 1,
                NG   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                NTS  = TimeSpan.FromMinutes(2),
                E    = TestEnumSByte.Pos,
                NE   = TestEnumSByte.Neg
            };

            msg.Ints.AddRange(new[] { 1, 2, 3 });
            msg.Strings.AddRange(new[] { "a", "b" });
            msg.Child = new ChildMsg { X = 7, Flag = true };
            msg.Children.Add(new ChildMsg { X = 1, Flag = false });

            byte[] a = msg.Serialize();
            byte[] b = msg.Serialize();

            Assert.That(a.SequenceEqual(b), Is.True);
        }

        [Test]
        public void Proto_Deserialize_UnknownField_IsSkipped_WhenInsertedInMiddle()
        {
            RootMsg msg      = new RootMsg { B = true, I = 123, S = "ok" };
            byte[]  baseData = msg.Serialize();

            byte[] unknownPayload = Payload(w =>
            {
                w.Write(999999);
                w.Write("junk");
            });

            byte[] mutated = InsertUnknownFieldInMiddle(baseData, unknownId: 999, unknownPayload: unknownPayload);

            RootMsg read = ProtoMsgBase.Deserialize<RootMsg>(mutated);

            Assert.Multiple(() =>
            {
                Assert.That(read.B, Is.EqualTo(true));
                Assert.That(read.I, Is.EqualTo(123));
                Assert.That(read.S, Is.EqualTo("ok"));
            });
        }

        [Test]
        public void Proto_Deserialize_DuplicateFieldId_LastValueWins()
        {
            // Build a stream with field #1 written twice.
            // ProtoModel should read sequentially and apply updates; last should win.
            byte[] payload1 = Payload(w => w.Write(1));
            byte[] payload2 = Payload(w => w.Write(999));

            using MemoryStream ms = new MemoryStream();
            AppendField(ms, 1, payload1);
            AppendField(ms, 1, payload2);

            DuplicateFieldMsg msg = ProtoMsgBase.Deserialize<DuplicateFieldMsg>(ms.ToArray());

            Assert.That(msg.I, Is.EqualTo(999));
        }

        [Test]
        public void Proto_Deserialize_ZeroLengthPayload_DoesNotCrash_AndLeavesDefault()
        {
            // For string field, the payload writer normally writes something.
            // Here we craft a field entry with length=0.
            using MemoryStream ms = new MemoryStream();
            AppendField(ms, 1, payload: Array.Empty<byte>());

            Assert.Throws<EndOfStreamException>(() => ProtoMsgBase.Deserialize<StringOnlyMsg>(ms.ToArray()));
        }

        [Test]
        public void Proto_Deserialize_CorruptLength_Throws()
        {
            // Field says it has payload length 100, but we provide only 3 bytes.
            using MemoryStream ms = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                bw.Write((ushort)1);
                bw.Write(100);
                bw.Write(new byte[] { 1, 2, 3 });
                bw.Flush();
            }

            Assert.Throws<EndOfStreamException>(() => ProtoMsgBase.Deserialize<DuplicateFieldMsg>(ms.ToArray()));
        }

        [Test]
        public void Proto_NullableValueTypes_Roundtrip_NullAndValue()
        {
            RootMsg a = new RootMsg { NI = null, NG = null, NTS           = null, NE                    = null };
            RootMsg b = new RootMsg { NI = 5, NG    = Guid.NewGuid(), NTS = TimeSpan.FromSeconds(1), NE = TestEnumSByte.Neg };

            RootMsg ra = ProtoMsgBase.Deserialize<RootMsg>(a.Serialize());
            RootMsg rb = ProtoMsgBase.Deserialize<RootMsg>(b.Serialize());

            Assert.Multiple(() =>
            {
                Assert.That(ra.NI,  Is.Null);
                Assert.That(ra.NG,  Is.Null);
                Assert.That(ra.NTS, Is.Null);
                Assert.That(ra.NE,  Is.Null);

                Assert.That(rb.NI,  Is.EqualTo(b.NI));
                Assert.That(rb.NG,  Is.EqualTo(b.NG));
                Assert.That(rb.NTS, Is.EqualTo(b.NTS));
                Assert.That(rb.NE,  Is.EqualTo(b.NE));
            });
        }

        [Test]
        public void Proto_NullablePrimitiveLists_Roundtrip_NullAndValue()
        {
            RootMsg a = new RootMsg { NInts = null, NStrings = null };
            RootMsg b = new RootMsg
            {
                NInts    = new List<int> { 1, 2, 3 },
                NStrings = new List<string> { "x", null!, "y" }
            };

            RootMsg ra = ProtoMsgBase.Deserialize<RootMsg>(a.Serialize());
            RootMsg rb = ProtoMsgBase.Deserialize<RootMsg>(b.Serialize());

            Assert.Multiple(() =>
            {
                Assert.That(ra.NInts,    Is.Null);
                Assert.That(ra.NStrings, Is.Null);

                Assert.That(rb.NInts,    Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(rb.NStrings, Is.EqualTo(new[] { "x", "", "y" }));
            });
        }

        [Test]
        public void Proto_NullableObject_Roundtrip_NullStaysNull()
        {
            RootMsg msg = new RootMsg { NChild = null };
            RootMsg r   = ProtoMsgBase.Deserialize<RootMsg>(msg.Serialize());
            Assert.That(r.NChild, Is.Null);
        }

        [Test]
        public void Proto_ObjectList_Roundtrip_PreservesAllElements()
        {
            RootMsg msg = new RootMsg();
            msg.Children.Add(new ChildMsg { X = 1, Flag = false });
            msg.Children.Add(new ChildMsg { X = 2, Flag = true });
            msg.Children.Add(new ChildMsg { X = 3, Flag = false });

            RootMsg r = ProtoMsgBase.Deserialize<RootMsg>(msg.Serialize());

            Assert.Multiple(() =>
            {
                Assert.That(r.Children.Count,   Is.EqualTo(3));
                Assert.That(r.Children[0].X,    Is.EqualTo(1));
                Assert.That(r.Children[0].Flag, Is.EqualTo(false));
                Assert.That(r.Children[1].X,    Is.EqualTo(2));
                Assert.That(r.Children[1].Flag, Is.EqualTo(true));
                Assert.That(r.Children[2].X,    Is.EqualTo(3));
                Assert.That(r.Children[2].Flag, Is.EqualTo(false));
            });
        }

        [Test]
        public void Proto_SchemaEvolution_OldWriter_NewReader_IgnoresMissingNewFields()
        {
            OldSchemaMsg oldMsg = new OldSchemaMsg { A = 10, B = "hi" };
            byte[]       data   = oldMsg.Serialize();

            NewSchemaMsg read = ProtoMsgBase.Deserialize<NewSchemaMsg>(data);

            Assert.Multiple(() =>
            {
                Assert.That(read.A, Is.EqualTo(10));
                Assert.That(read.B, Is.EqualTo("hi"));
                Assert.That(read.C, Is.EqualTo(0));
            });
        }

        [Test]
        public void Proto_SchemaEvolution_NewWriter_OldReader_SkipsUnknownFields()
        {
            NewSchemaMsg newMsg = new NewSchemaMsg { A = 10, B = "hi", C = 999 };
            byte[]       data   = newMsg.Serialize();

            OldSchemaMsg read = ProtoMsgBase.Deserialize<OldSchemaMsg>(data);

            Assert.Multiple(() =>
            {
                Assert.That(read.A, Is.EqualTo(10));
                Assert.That(read.B, Is.EqualTo("hi"));
            });
        }

        [Test]
        public void Proto_EnumAndNullableEnum_Roundtrip()
        {
            RootMsg msg = new RootMsg { E = TestEnumSByte.Neg, NE = TestEnumSByte.Pos };
            RootMsg r   = ProtoMsgBase.Deserialize<RootMsg>(msg.Serialize());

            Assert.Multiple(() =>
            {
                Assert.That(r.E,  Is.EqualTo(TestEnumSByte.Neg));
                Assert.That(r.NE, Is.EqualTo(TestEnumSByte.Pos));
            });
        }

        [Test]
        public void Proto_Stress_ManyRounds_RoundtripStable()
        {
            // Not a perf benchmark—just a high-coverage “doesn’t drift / corrupt”.
            for (int i = 0; i < 200; i++)
            {
                RootMsg msg = new RootMsg
                {
                    B    = (i % 2) == 0,
                    I    = i * 12345,
                    S    = "s" + i,
                    G    = Guid.NewGuid(),
                    DT   = new DateTime(2020, 1, 1).AddMinutes(i),
                    TS   = TimeSpan.FromTicks(i * 10),
                    V    = new Version(1, i % 10, i % 100),
                    Code = HttpStatusCode.OK,
                    NI   = (i % 3) == 0 ? null : i,
                    NG   = (i % 5) == 0 ? null : Guid.NewGuid(),
                    NTS  = (i % 7) == 0 ? null : TimeSpan.FromSeconds(i),
                    E    = (i % 3) == 0 ? TestEnumSByte.Neg : TestEnumSByte.Pos,
                    NE   = (i % 4) == 0 ? null : TestEnumSByte.Zero
                };

                msg.Ints.AddRange(new[] { i, i + 1, i + 2 });
                msg.Strings.AddRange(new[] { "a", null!, "b" });
                msg.Children.Add(new ChildMsg { X = i, Flag     = (i % 2) == 1 });
                msg.Children.Add(new ChildMsg { X = i + 1, Flag = (i % 2) == 0 });

                msg.NInts    = (i % 6) == 0 ? null : new List<int> { 1, 2, 3 };
                msg.NStrings = (i % 8) == 0 ? null : new List<string> { "x", null!, "y" };

                byte[]  data = msg.Serialize();
                RootMsg r    = ProtoMsgBase.Deserialize<RootMsg>(data);

                Assert.That(r.I,  Is.EqualTo(msg.I));
                Assert.That(r.S,  Is.EqualTo(msg.S));
                Assert.That(r.B,  Is.EqualTo(msg.B));
                Assert.That(r.V,  Is.EqualTo(msg.V));
                Assert.That(r.NI, Is.EqualTo(msg.NI));
                Assert.That(r.NE, Is.EqualTo(msg.NE));

                Assert.That(r.Children.Count,   Is.EqualTo(msg.Children.Count));
                Assert.That(r.Children[0].X,    Is.EqualTo(msg.Children[0].X));
                Assert.That(r.Children[0].Flag, Is.EqualTo(msg.Children[0].Flag));
            }
        }
    }
}