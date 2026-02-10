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
    }
}