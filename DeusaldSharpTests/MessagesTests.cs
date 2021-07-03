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

using DeusaldSharp.Messages;
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
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
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
        public void XWTMN()
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
                Assert.AreEqual(10,              receivedInt);
                Assert.AreEqual(10.5f,           receivedFloat);
                Assert.AreEqual("Test",          receivedString);
                Assert.AreEqual(objectToReceive, receivedObject);
            });

            // Act
            msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(0,  receivedInt);
                Assert.AreEqual(0f, receivedFloat);
                Assert.IsNull(receivedString);
                Assert.IsNull(receivedObject);
            });
        }

        /// <summary> Binding, unbinding and message send test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Unbind))]
        public void KBFHM()
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
                Assert.AreEqual(0,  receivedInt);
                Assert.AreEqual(0f, receivedFloat);
                Assert.IsNull(receivedString);
                Assert.IsNull(receivedObject);
            });
        }

        /// <summary> Binding inside receiving message. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Bind))]
        public void ZSKML()
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
                Assert.AreEqual(0,  receivedInt);
                Assert.AreEqual(0f, receivedFloat);
                Assert.IsNull(receivedString);
                Assert.IsNull(receivedObject);
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
                Assert.AreEqual(10,              receivedInt);
                Assert.AreEqual(10.5f,           receivedFloat);
                Assert.AreEqual("Test",          receivedString);
                Assert.AreEqual(objectToReceive, receivedObject);
            });
        }

        /// <summary> Priority test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void XHMZP()
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
            Assert.AreEqual("ba", order);
        }

        /// <summary> Deallocate test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void PDRDZ()
        {
            // Arrange
            void Receive(ExampleMsg message) { }
            MsgCtrl.Bind<ExampleMsg>(Receive);
            
            // Act
            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            
            // Assert
            Assert.AreEqual(0, MessagesPool<ExampleMsg>.NumberOfFreeMessages);
            
            // Act
            MsgCtrl.Deallocate(msg);
            
            // Assert
            Assert.AreEqual(1, MessagesPool<ExampleMsg>.NumberOfFreeMessages);
        }
        
        /// <summary> Consume test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Send))]
        public void RBYAX()
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
            Assert.AreEqual("b", order);
        }
        
        /// <summary> Custom attribute register test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Register))]
        public void YZDMS()
        {
            // Arrange
            AttributeTest attributeTest = new AttributeTest();

            // Act
            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.AreEqual(1, attributeTest.ReceivedMessages);
        }
        
        /// <summary> Custom attribute unregister test. </summary>
        [Test]
        [TestOf(nameof(MsgCtrl.Unregister))]
        public void DEZSX()
        {
            // Arrange
            AttributeTest attributeTest = new AttributeTest();

            // Act
            attributeTest.Unregister();
            ExampleMsg msg = MsgCtrl.Allocate<ExampleMsg>();
            MsgCtrl.Send(msg);

            // Assert
            Assert.AreEqual(0, attributeTest.ReceivedMessages);
        }
    }
}