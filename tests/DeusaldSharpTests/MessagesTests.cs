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

using System;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class MessagesTests
    {
        private class ExampleMsg
        {
            public int    Int    { get; set; }
            public float  Float  { get; set; }
            public string String { get; set; }
            public object Object { get; set; }
        }

        private class AttributeTest
        {
            public int ReceivedMessages { get; private set; }

            public AttributeTest()
            {
                MsgCtrl.Register(this);
            }

            public void Unregister()
            {
                MsgCtrl.Unregister(this);
            }
            
            [MessageSlot]
            public void Receive(ExampleMsg message)
            {
                ++ReceivedMessages;
            }
        }

        private class PoolMsg
        {
            public int Value { get; set; }
        }

        private class ThrowMsg { }

        private class ClearMsg { }

        private class ConsumeMsg { }

        private class BaseSlotMsg { }

        private class MembersMsg
        {
            public int    Value;
            public string Text;

            public int    Doubled         => Value * 2;
            public string ReadOnly        { get; } = "ReadOnly";
            public int    this[int index] => index;
            public float  Property        { get; set; }
        }

        private class StructSlotReceiver
        {
            [MessageSlot]
            public void Receive(int value) { }
        }

        private class BaseReceiver
        {
            public int ReceivedMessages { get; private set; }

            [MessageSlot]
            private void ReceiveInBase(BaseSlotMsg message)
            {
                ++ReceivedMessages;
            }
        }

        private class DerivedReceiver : BaseReceiver { }

        [SetUp]
        public void Setup()
        {
            MsgCtrl.Clear();
        }

        /// <summary> Binding and message send test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Bind))]
        public void MsgCtrl_Bind()
        {
            // Arrange
            int    receivedInt    = 0;
            float  receivedFloat  = 0;
            string receivedString = null;
            object receivedObject = null;

            void Receive(ExampleMsg message)
            {
                receivedInt    = message.Int;
                receivedFloat  = message.Float;
                receivedString = message.String;
                receivedObject = message.Object;
            }

            object objectToReceive = new object();

            // Act
            MsgCtrl.Bind<ExampleMsg>(Receive);

            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            msg.Int    = 10;
            msg.Float  = 10.5f;
            msg.String = "Test";
            msg.Object = objectToReceive;
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInt, Is.EqualTo(10));
                Assert.That(receivedFloat, Is.EqualTo(10.5f));
                Assert.That(receivedString, Is.EqualTo("Test"));
                Assert.That(receivedObject, Is.EqualTo(objectToReceive));
            });

            // Act
            msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInt, Is.EqualTo(0));
                Assert.That(receivedFloat, Is.EqualTo(0f));
                Assert.That(receivedString, Is.Null);
                Assert.That(receivedObject, Is.Null);
            });
        }

        /// <summary> Binding, unbinding and message send test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Unbind))]
        public void MsgCtrl_Unbind()
        {
            // Arrange
            int    receivedInt    = 0;
            float  receivedFloat  = 0;
            string receivedString = null;
            object receivedObject = null;

            void Receive(ExampleMsg message)
            {
                receivedInt    = message.Int;
                receivedFloat  = message.Float;
                receivedString = message.String;
                receivedObject = message.Object;
            }

            object objectToReceive = new object();

            // Act
            MsgCtrl.Bind<ExampleMsg>(Receive);
            MsgCtrl.Unbind<ExampleMsg>(Receive);

            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            msg.Int    = 10;
            msg.Float  = 10.5f;
            msg.String = "Test";
            msg.Object = objectToReceive;
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInt, Is.EqualTo(0));
                Assert.That(receivedFloat, Is.EqualTo(0f));
                Assert.That(receivedString, Is.Null);
                Assert.That(receivedObject, Is.Null);
            });
        }

        /// <summary> Binding inside receiving message. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Bind))]
        public void MsgCtrl_Bind_Inside()
        {
            // Arrange
            int    receivedInt    = 0;
            float  receivedFloat  = 0;
            string receivedString = null;
            object receivedObject = null;

            void Receive(ExampleMsg message)
            {
                MsgCtrl.Bind<ExampleMsg>(ReceiveTwo);
            }

            void ReceiveTwo(ExampleMsg message)
            {
                receivedInt    = message.Int;
                receivedFloat  = message.Float;
                receivedString = message.String;
                receivedObject = message.Object;
            }

            object objectToReceive = new object();

            // Act
            MsgCtrl.Bind<ExampleMsg>(Receive);

            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            msg.Int    = 10;
            msg.Float  = 10.5f;
            msg.String = "Test";
            msg.Object = objectToReceive;
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInt, Is.EqualTo(0));
                Assert.That(receivedFloat, Is.EqualTo(0f));
                Assert.That(receivedString, Is.Null);
                Assert.That(receivedObject, Is.Null);
            });

            // Act
            msg        = MsgCtrl.Allocate<ExampleMsg>();
            msg.Int    = 10;
            msg.Float  = 10.5f;
            msg.String = "Test";
            msg.Object = objectToReceive;
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedInt, Is.EqualTo(10));
                Assert.That(receivedFloat, Is.EqualTo(10.5f));
                Assert.That(receivedString, Is.EqualTo("Test"));
                Assert.That(receivedObject, Is.EqualTo(objectToReceive));
            });
        }

        /// <summary> Priority test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void MsgCtrl_Send_Priority()
        {
            // Arrange
            string order = "";

            void Receive(ExampleMsg message)
            {
                order += "a";
            }

            void ReceiveTwo(ExampleMsg message)
            {
                order += "b";
            }

            // Act
            MsgCtrl.Bind<ExampleMsg>(Receive,    10);
            MsgCtrl.Bind<ExampleMsg>(ReceiveTwo, -10);

            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.That(order, Is.EqualTo("ba"));
        }

        /// <summary> Deallocate test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void MsgCtrl_Send_Deallocate()
        {
            // Arrange
            void Receive(ExampleMsg message) { }
            MsgCtrl.Bind<ExampleMsg>(Receive);
            
            // Act
            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            
            // Assert
            Assert.That(MessagesPool<ExampleMsg>.NumberOfFreeMessages, Is.EqualTo(0));
            
            // Act
            MsgCtrl.Deallocate(msg);
            
            // Assert
            Assert.That(MessagesPool<ExampleMsg>.NumberOfFreeMessages, Is.EqualTo(1));
        }
        
        /// <summary> Consume test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void MsgCtrl_Send_Consume()
        {
            // Arrange
            string order = "";

            void Receive(ExampleMsg message)
            {
                order += "a";
            }

            void ReceiveTwo(ExampleMsg message)
            {
                order += "b";
                MsgCtrl.Consume();
            }

            // Act
            MsgCtrl.Bind<ExampleMsg>(Receive,    10);
            MsgCtrl.Bind<ExampleMsg>(ReceiveTwo, -10);

            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.That(order, Is.EqualTo("b"));
        }
        
        /// <summary> Custom attribute register test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Register))]
        public void MsgCtrl_Register()
        {
            // Arrange
            AttributeTest attributeTest = new AttributeTest();

            // Act
            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.That(attributeTest.ReceivedMessages, Is.EqualTo(1));
        }
        
        /// <summary> Custom attribute unregister test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Unregister))]
        public void MsgCtrl_Unregister()
        {
            // Arrange
            AttributeTest attributeTest = new AttributeTest();

            // Act
            attributeTest.Unregister();
            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.That(attributeTest.ReceivedMessages, Is.EqualTo(0));
        }

        /// <summary> Sent message is returned to the pool of its own type. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void MsgCtrl_Send_ReturnsMessageToTypedPool()
        {
            // Arrange
            void Receive(PoolMsg message) { }
            MsgCtrl.Bind<PoolMsg>(Receive);

            PoolMsg msg              = MsgCtrl.Allocate<PoolMsg>();
            int     freeTyped        = MessagesPool<PoolMsg>.NumberOfFreeMessages;
            int     freeObjectBefore = MessagesPool<object>.NumberOfFreeMessages;

            // Act
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(MessagesPool<PoolMsg>.NumberOfFreeMessages, Is.EqualTo(freeTyped + 1));
                Assert.That(MessagesPool<object>.NumberOfFreeMessages,  Is.EqualTo(freeObjectBefore));
                Assert.That(MsgCtrl.Allocate<PoolMsg>(),                Is.SameAs(msg));
            });
        }

        /// <summary> Exception in a receiver propagates to the caller and doesn't break the bus. Queued messages are not lost. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void MsgCtrl_Send_ReceiverThrows_BusStillWorks()
        {
            // Arrange
            int calls = 0;

            void Receive(ThrowMsg message)
            {
                ++calls;
                if (calls != 1) return;

                MsgCtrl.Send(MsgCtrl.Allocate<ThrowMsg>()); // queued, delivered later
                throw new InvalidOperationException("Test");
            }

            MsgCtrl.Bind<ThrowMsg>(Receive);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => MsgCtrl.Send(MsgCtrl.Allocate<ThrowMsg>()));
            Assert.That(calls, Is.EqualTo(1));

            MsgCtrl.Send(MsgCtrl.Allocate<ThrowMsg>());
            Assert.That(calls, Is.EqualTo(3));
        }

        /// <summary> Message with computed properties and indexers can be sent, writable properties and fields are reset. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void MsgCtrl_Send_ResetsWritableMembersOnly()
        {
            // Arrange
            int receivedDoubled = 0;

            void Receive(MembersMsg message)
            {
                receivedDoubled = message.Doubled;
            }

            MsgCtrl.Bind<MembersMsg>(Receive);

            MembersMsg msg = MsgCtrl.Allocate<MembersMsg>();
            msg.Value    = 5;
            msg.Text     = "Text";
            msg.Property = 1.5f;

            // Act
            Assert.DoesNotThrow(() => MsgCtrl.Send(msg));
            MembersMsg reused = MsgCtrl.Allocate<MembersMsg>();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(receivedDoubled, Is.EqualTo(10));
                Assert.That(reused,          Is.SameAs(msg));
                Assert.That(reused.Value,    Is.EqualTo(0));
                Assert.That(reused.Text,     Is.Null);
                Assert.That(reused.Property, Is.EqualTo(0f));
                Assert.That(reused.ReadOnly, Is.EqualTo("ReadOnly"));
            });
        }

        /// <summary> Clear called inside a receiver stops delivery and leaves the bus usable. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Clear))]
        public void MsgCtrl_Clear_InsideReceiver()
        {
            // Arrange
            int secondCalls = 0;

            void ReceiveAndClear(ClearMsg message)
            {
                MsgCtrl.Clear();
            }

            void ReceiveSecond(ClearMsg message)
            {
                ++secondCalls;
            }

            MsgCtrl.Bind<ClearMsg>(ReceiveAndClear, -10);
            MsgCtrl.Bind<ClearMsg>(ReceiveSecond,   10);

            // Act & Assert
            Assert.DoesNotThrow(() => MsgCtrl.Send(MsgCtrl.Allocate<ClearMsg>()));
            Assert.That(secondCalls, Is.EqualTo(0));

            MsgCtrl.Bind<ClearMsg>(ReceiveSecond);
            MsgCtrl.Send(MsgCtrl.Allocate<ClearMsg>());
            Assert.That(secondCalls, Is.EqualTo(1));
        }

        /// <summary> Consume called outside of receiver has no effect on next message. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Consume))]
        public void MsgCtrl_Consume_OutsideReceiver_Ignored()
        {
            // Arrange
            string order = "";

            void Receive(ConsumeMsg message)
            {
                order += "a";
            }

            void ReceiveTwo(ConsumeMsg message)
            {
                order += "b";
            }

            MsgCtrl.Bind<ConsumeMsg>(Receive,    -10);
            MsgCtrl.Bind<ConsumeMsg>(ReceiveTwo, 10);

            // Act
            MsgCtrl.Consume();
            MsgCtrl.Send(MsgCtrl.Allocate<ConsumeMsg>());

            // Assert
            Assert.That(order, Is.EqualTo("ab"));
        }

        /// <summary> Registering slot with struct parameter throws. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Register))]
        public void MsgCtrl_Register_StructParameter_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => MsgCtrl.Register(new StructSlotReceiver()));
        }

        /// <summary> Private [MessageSlot] methods declared in base classes are registered. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Register))]
        public void MsgCtrl_Register_BaseClassPrivateSlot()
        {
            // Arrange
            DerivedReceiver receiver = new DerivedReceiver();

            // Act
            MsgCtrl.Register(receiver);
            MsgCtrl.Send(MsgCtrl.Allocate<BaseSlotMsg>());

            // Assert
            Assert.That(receiver.ReceivedMessages, Is.EqualTo(1));
        }
    }
}