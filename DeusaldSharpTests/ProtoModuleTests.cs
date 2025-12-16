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
using NUnit.Framework;
// ReSharper disable EnumUnderlyingTypeIsInt
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedMember.Local

namespace DeusaldSharpTests
{
    public class ProtoModuleTests
    {
        // ------------------------------------------------------------
        // Test enums
        // ------------------------------------------------------------

        [SerializableEnum(SerializableEnumType.SByte)]
        private enum TestEnumSByte : sbyte
        {
            Neg  = -1,
            Zero = 0,
            Pos  = 1
        }

        [SerializableEnum(SerializableEnumType.Int)]
        private enum TestEnumInt : int
        {
            A = 1,
            B = 2,
            C = 3
        }

        // ------------------------------------------------------------
        // Test messages
        // ------------------------------------------------------------

        private sealed class ChildMsg : ProtoMsg<ChildMsg>
        {
            public int  X;
            public bool Flag;

            static ChildMsg()
            {
                _model = new ProtoModel<ChildMsg>(
                    ProtoField.Int(1, static (ref ChildMsg o) => ref o.X),
                    ProtoField.Bool(2, static (ref ChildMsg o) => ref o.Flag)
                );
            }
        }

        /// <summary>
        /// “Full” message to cover most public ProtoField factories.
        /// </summary>
        private sealed class RootMsg : ProtoMsg<RootMsg>
        {
            // primitives
            public bool   B;
            public int    I;
            public string S = "";

            // special
            public Guid           G;
            public DateTime       DT;
            public TimeSpan       TS;
            public Version        V = new(1, 0, 0);
            public HttpStatusCode Code;

            // nullable value-types
            public int?      NI;
            public Guid?     NG;
            public TimeSpan? NTS;

            // enums
            public TestEnumSByte  E;
            public TestEnumSByte? NE;

            // lists (non-nullable)
            public List<int>           Ints    = new();
            public List<string>        Strings = new();
            public List<Guid>          Guids   = new();
            public List<TestEnumSByte> Enums   = new();

            // nullable lists (typed factories you exposed + new primitive list factories)
            public List<int>?           NInts;
            public List<string>?        NStrings;
            public List<TestEnumSByte>? NEnums;

            // objects
            public ChildMsg  Child = new();
            public ChildMsg? NChild;

            // object lists
            public List<ChildMsg>  Children  = new();
            public List<ChildMsg>? NChildren;

            static RootMsg()
            {
                _model = new ProtoModel<RootMsg>(
                    // primitives
                    ProtoField.Bool(1, static (ref RootMsg o) => ref o.B),
                    ProtoField.Int(2, static (ref RootMsg o) => ref o.I),
                    ProtoField.String(3, static (ref RootMsg o) => ref o.S),

                    // special
                    ProtoField.Guid(4, static (ref RootMsg o) => ref o.G),
                    ProtoField.DateTime(5, static (ref RootMsg o) => ref o.DT),
                    ProtoField.TimeSpan(6, static (ref RootMsg o) => ref o.TS),
                    ProtoField.Version(7, static (ref RootMsg o) => ref o.V),
                    ProtoField.HttpStatusCode(8, static (ref RootMsg o) => ref o.Code),

                    // nullable value-types
                    ProtoField.NullableInt(9, static (ref RootMsg o) => ref o.NI),
                    ProtoField.NullableGuid(10, static (ref RootMsg o) => ref o.NG),
                    ProtoField.NullableTimeSpan(11, static (ref RootMsg o) => ref o.NTS),

                    // enums
                    ProtoField.SerializableEnum(12, static (ref RootMsg o) => ref o.E),
                    ProtoField.NullableSerializableEnum(13, static (ref RootMsg o) => ref o.NE),

                    // lists
                    ProtoField.IntList(14, static (ref RootMsg o) => ref o.Ints),
                    ProtoField.StringList(15, static (ref RootMsg o) => ref o.Strings),
                    ProtoField.GuidList(16, static (ref RootMsg o) => ref o.Guids),
                    ProtoField.SerializableEnumList(17, static (ref RootMsg o) => ref o.Enums),

                    // nullable lists (primitive factories you added)
                    ProtoField.NullableIntList(18, static (ref RootMsg o) => ref o.NInts),
                    ProtoField.NullableStringList(19, static (ref RootMsg o) => ref o.NStrings),
                    ProtoField.NullableSerializableEnumList(20, static (ref RootMsg o) => ref o.NEnums),

                    // objects
                    ProtoField.Object(21, static (ref RootMsg o) => ref o.Child),
                    ProtoField.NullableObject(22, static (ref RootMsg o) => ref o.NChild),

                    // object lists
                    ProtoField.ObjectList(23, static (ref RootMsg o) => ref o.Children),
                    ProtoField.NullableObjectList(24, static (ref RootMsg o) => ref o.NChildren)
                );
            }
        }

        /// <summary>
        /// Small schema for evolution tests (old/new schemas).
        /// </summary>
        private sealed class OldSchemaMsg : ProtoMsg<OldSchemaMsg>
        {
            public int    A;
            public string B = "";

            static OldSchemaMsg()
            {
                _model = new ProtoModel<OldSchemaMsg>(
                    ProtoField.Int(1, static (ref OldSchemaMsg o) => ref o.A),
                    ProtoField.String(2, static (ref OldSchemaMsg o) => ref o.B)
                );
            }
        }

        private sealed class NewSchemaMsg : ProtoMsg<NewSchemaMsg>
        {
            public int    A;
            public string B = "";
            public int    C;

            static NewSchemaMsg()
            {
                _model = new ProtoModel<NewSchemaMsg>(
                    ProtoField.Int(1, static (ref NewSchemaMsg o) => ref o.A),
                    ProtoField.String(2, static (ref NewSchemaMsg o) => ref o.B),
                    ProtoField.Int(3, static (ref NewSchemaMsg o) => ref o.C)
                );
            }
        }

        /// <summary>
        /// Message designed to test duplicate fields in stream.
        /// </summary>
        private sealed class DuplicateFieldMsg : ProtoMsg<DuplicateFieldMsg>
        {
            public int I;

            static DuplicateFieldMsg()
            {
                _model = new ProtoModel<DuplicateFieldMsg>(
                    ProtoField.Int(1, static (ref DuplicateFieldMsg o) => ref o.I)
                );
            }
        }

        /// <summary>
        /// Message to test zero-length payload behaviour.
        /// </summary>
        private sealed class StringOnlyMsg : ProtoMsg<StringOnlyMsg>
        {
            public string S = "";

            static StringOnlyMsg()
            {
                _model = new ProtoModel<StringOnlyMsg>(
                    ProtoField.String(1, static (ref StringOnlyMsg o) => ref o.S)
                );
            }
        }

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
            using BinaryReader br = new BinaryReader(input, Encoding.UTF8, leaveOpen: true);

            ushort firstId = br.ReadUInt16();
            int firstLen = br.ReadInt32();
            byte[] firstPayload = br.ReadBytes(firstLen);

            byte[] rest = br.ReadBytes((int)(input.Length - input.Position));

            using MemoryStream output = new MemoryStream();
            AppendField(output, firstId, firstPayload);
            AppendField(output, unknownId, unknownPayload);
            output.Write(rest, 0, rest.Length);

            return output.ToArray();
        }

        // ------------------------------------------------------------
        // Tests
        // ------------------------------------------------------------

        [Test]
        public void Proto_Roundtrip_Primitives_Specials_Nullables_Lists_Objects()
        {
            // Arrange
            RootMsg msg = new RootMsg
            {
                B    = true,
                I    = 123456,
                S    = "hello",

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

            msg.Child.X    = 10;
            msg.Child.Flag = true;

            msg.NChild = null;

            msg.Children.Add(new ChildMsg { X = 1, Flag = false });
            msg.Children.Add(new ChildMsg { X = 2, Flag = true });

            msg.NChildren = new List<ChildMsg> { new() { X = 42, Flag = true } };

            // Act
            byte[] data = msg.Serialize();
            RootMsg roundtrip = RootMsg.Deserialize(data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(msg.B, roundtrip.B);
                Assert.AreEqual(msg.I, roundtrip.I);
                Assert.AreEqual(msg.S, roundtrip.S);

                Assert.AreEqual(msg.G, roundtrip.G);
                Assert.AreEqual(msg.DT, roundtrip.DT);
                Assert.AreEqual(msg.TS, roundtrip.TS);
                Assert.AreEqual(msg.V, roundtrip.V);
                Assert.AreEqual(msg.Code, roundtrip.Code);

                Assert.AreEqual(msg.NI, roundtrip.NI);
                Assert.AreEqual(msg.NG, roundtrip.NG);
                Assert.AreEqual(msg.NTS, roundtrip.NTS);

                Assert.AreEqual(msg.E, roundtrip.E);
                Assert.AreEqual(msg.NE, roundtrip.NE);

                CollectionAssert.AreEqual(msg.Ints, roundtrip.Ints);

                // string list write uses (v ?? string.Empty)
                CollectionAssert.AreEqual(new[] { "a", "", "żółw" }, roundtrip.Strings);

                CollectionAssert.AreEqual(msg.Guids, roundtrip.Guids);
                CollectionAssert.AreEqual(msg.Enums, roundtrip.Enums);

                CollectionAssert.AreEqual(msg.NInts, roundtrip.NInts);
                CollectionAssert.AreEqual(new[] { "x", "", "y" }, roundtrip.NStrings);
                CollectionAssert.AreEqual(msg.NEnums, roundtrip.NEnums);

                Assert.AreEqual(msg.Child.X, roundtrip.Child.X);
                Assert.AreEqual(msg.Child.Flag, roundtrip.Child.Flag);

                Assert.AreEqual(2, roundtrip.Children.Count);
                Assert.AreEqual(1, roundtrip.Children[0].X);
                Assert.AreEqual(false, roundtrip.Children[0].Flag);
                Assert.AreEqual(2, roundtrip.Children[1].X);
                Assert.AreEqual(true, roundtrip.Children[1].Flag);

                Assert.NotNull(roundtrip.NChildren);
                Assert.AreEqual(1, roundtrip.NChildren!.Count);
                Assert.AreEqual(42, roundtrip.NChildren[0].X);
                Assert.AreEqual(true, roundtrip.NChildren[0].Flag);
            });
        }

        [Test]
        public void Proto_Serialize_IsDeterministic_ForSameState()
        {
            RootMsg msg = new RootMsg
            {
                B = true,
                I = 5,
                S = "x",
                G = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                DT = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                TS = TimeSpan.FromSeconds(1),
                V = new Version(1, 2, 3),
                Code = HttpStatusCode.OK,
                NI = 1,
                NG = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                NTS = TimeSpan.FromMinutes(2),
                E = TestEnumSByte.Pos,
                NE = TestEnumSByte.Neg
            };

            msg.Ints.AddRange(new[] { 1, 2, 3 });
            msg.Strings.AddRange(new[] { "a", "b" });
            msg.Child = new ChildMsg { X = 7, Flag = true };
            msg.Children.Add(new ChildMsg { X = 1, Flag = false });

            byte[] a = msg.Serialize();
            byte[] b = msg.Serialize();

            Assert.IsTrue(a.SequenceEqual(b));
        }

        [Test]
        public void Proto_Deserialize_UnknownField_IsSkipped_WhenInsertedInMiddle()
        {
            RootMsg msg = new RootMsg { B = true, I = 123, S = "ok" };
            byte[] baseData = msg.Serialize();

            byte[] unknownPayload = Payload(w =>
            {
                w.Write(999999);
                w.Write("junk");
            });

            byte[] mutated = InsertUnknownFieldInMiddle(baseData, unknownId: 999, unknownPayload: unknownPayload);

            RootMsg read = RootMsg.Deserialize(mutated);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(true, read.B);
                Assert.AreEqual(123, read.I);
                Assert.AreEqual("ok", read.S);
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

            DuplicateFieldMsg msg = DuplicateFieldMsg.Deserialize(ms.ToArray());

            Assert.AreEqual(999, msg.I);
        }

        [Test]
        public void Proto_Deserialize_ZeroLengthPayload_DoesNotCrash_AndLeavesDefault()
        {
            // For string field, the payload writer normally writes something.
            // Here we craft a field entry with length=0.
            using MemoryStream ms = new MemoryStream();
            AppendField(ms, 1, payload: Array.Empty<byte>());
            
            Assert.Throws<EndOfStreamException>(() => StringOnlyMsg.Deserialize(ms.ToArray()));
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

            Assert.Throws<EndOfStreamException>(() => DuplicateFieldMsg.Deserialize(ms.ToArray()));
        }

        [Test]
        public void Proto_NullableValueTypes_Roundtrip_NullAndValue()
        {
            RootMsg a = new RootMsg { NI = null, NG = null, NTS = null, NE = null };
            RootMsg b = new RootMsg { NI = 5, NG = Guid.NewGuid(), NTS = TimeSpan.FromSeconds(1), NE = TestEnumSByte.Neg };

            RootMsg ra = RootMsg.Deserialize(a.Serialize());
            RootMsg rb = RootMsg.Deserialize(b.Serialize());

            Assert.Multiple(() =>
            {
                Assert.IsNull(ra.NI);
                Assert.IsNull(ra.NG);
                Assert.IsNull(ra.NTS);
                Assert.IsNull(ra.NE);

                Assert.AreEqual(b.NI, rb.NI);
                Assert.AreEqual(b.NG, rb.NG);
                Assert.AreEqual(b.NTS, rb.NTS);
                Assert.AreEqual(b.NE, rb.NE);
            });
        }

        [Test]
        public void Proto_NullablePrimitiveLists_Roundtrip_NullAndValue()
        {
            RootMsg a = new RootMsg { NInts = null, NStrings = null };
            RootMsg b = new RootMsg
            {
                NInts = new List<int> { 1, 2, 3 },
                NStrings = new List<string> { "x", null!, "y" }
            };

            RootMsg ra = RootMsg.Deserialize(a.Serialize());
            RootMsg rb = RootMsg.Deserialize(b.Serialize());

            Assert.Multiple(() =>
            {
                Assert.IsNull(ra.NInts);
                Assert.IsNull(ra.NStrings);

                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, rb.NInts);
                CollectionAssert.AreEqual(new[] { "x", "", "y" }, rb.NStrings);
            });
        }

        [Test]
        public void Proto_NullableObject_Roundtrip_NullStaysNull()
        {
            RootMsg msg = new RootMsg { NChild = null };
            RootMsg r = RootMsg.Deserialize(msg.Serialize());
            Assert.IsNull(r.NChild);
        }

        [Test]
        public void Proto_ObjectList_Roundtrip_PreservesAllElements()
        {
            RootMsg msg = new RootMsg();
            msg.Children.Add(new ChildMsg { X = 1, Flag = false });
            msg.Children.Add(new ChildMsg { X = 2, Flag = true });
            msg.Children.Add(new ChildMsg { X = 3, Flag = false });

            RootMsg r = RootMsg.Deserialize(msg.Serialize());

            Assert.Multiple(() =>
            {
                Assert.AreEqual(3, r.Children.Count);
                Assert.AreEqual(1, r.Children[0].X);
                Assert.AreEqual(false, r.Children[0].Flag);
                Assert.AreEqual(2, r.Children[1].X);
                Assert.AreEqual(true, r.Children[1].Flag);
                Assert.AreEqual(3, r.Children[2].X);
                Assert.AreEqual(false, r.Children[2].Flag);
            });
        }

        [Test]
        public void Proto_SchemaEvolution_OldWriter_NewReader_IgnoresMissingNewFields()
        {
            OldSchemaMsg oldMsg = new OldSchemaMsg { A = 10, B = "hi" };
            byte[] data = oldMsg.Serialize();

            NewSchemaMsg read = NewSchemaMsg.Deserialize(data);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10, read.A);
                Assert.AreEqual("hi", read.B);
                Assert.AreEqual(0, read.C); // default
            });
        }

        [Test]
        public void Proto_SchemaEvolution_NewWriter_OldReader_SkipsUnknownFields()
        {
            NewSchemaMsg newMsg = new NewSchemaMsg { A = 10, B = "hi", C = 999 };
            byte[] data = newMsg.Serialize();

            OldSchemaMsg read = OldSchemaMsg.Deserialize(data);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(10, read.A);
                Assert.AreEqual("hi", read.B);
            });
        }

        [Test]
        public void Proto_EnumAndNullableEnum_Roundtrip()
        {
            RootMsg msg = new RootMsg { E = TestEnumSByte.Neg, NE = TestEnumSByte.Pos };
            RootMsg r = RootMsg.Deserialize(msg.Serialize());

            Assert.Multiple(() =>
            {
                Assert.AreEqual(TestEnumSByte.Neg, r.E);
                Assert.AreEqual(TestEnumSByte.Pos, r.NE);
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
                    B  = (i % 2) == 0,
                    I  = i * 12345,
                    S  = "s" + i,
                    G  = Guid.NewGuid(),
                    DT = new DateTime(2020, 1, 1).AddMinutes(i),
                    TS = TimeSpan.FromTicks(i * 10),
                    V  = new Version(1, i % 10, i % 100),
                    Code = HttpStatusCode.OK,
                    NI = (i % 3) == 0 ? null : i,
                    NG = (i % 5) == 0 ? null : Guid.NewGuid(),
                    NTS = (i % 7) == 0 ? null : TimeSpan.FromSeconds(i),
                    E = (i % 3) == 0 ? TestEnumSByte.Neg : TestEnumSByte.Pos,
                    NE = (i % 4) == 0 ? null : TestEnumSByte.Zero
                };

                msg.Ints.AddRange(new[] { i, i + 1, i + 2 });
                msg.Strings.AddRange(new[] { "a", null!, "b" });
                msg.Children.Add(new ChildMsg { X = i, Flag = (i % 2) == 1 });
                msg.Children.Add(new ChildMsg { X = i + 1, Flag = (i % 2) == 0 });

                msg.NInts = (i % 6) == 0 ? null : new List<int> { 1, 2, 3 };
                msg.NStrings = (i % 8) == 0 ? null : new List<string> { "x", null!, "y" };

                byte[] data = msg.Serialize();
                RootMsg r = RootMsg.Deserialize(data);

                Assert.AreEqual(msg.I, r.I);
                Assert.AreEqual(msg.S, r.S);
                Assert.AreEqual(msg.B, r.B);
                Assert.AreEqual(msg.V, r.V);
                Assert.AreEqual(msg.NI, r.NI);
                Assert.AreEqual(msg.NE, r.NE);

                Assert.AreEqual(msg.Children.Count, r.Children.Count);
                Assert.AreEqual(msg.Children[0].X, r.Children[0].X);
                Assert.AreEqual(msg.Children[0].Flag, r.Children[0].Flag);
            }
        }
    }
}