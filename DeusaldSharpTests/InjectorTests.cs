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

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local

using System.Collections.Generic;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class InjectorTests
    {
        // Fields below are assigned through reflection by the Injector.
#pragma warning disable CS0649

        #region Types

        private interface IService
        {
            string Name { get; }
        }

        private interface IOtherService { }

        private class Service : IService, IOtherService
        {
            public string Name => "Service";
        }

        private class Logger { }

        private class PropertyTarget
        {
            [Inject] public  IService Service         { get; set; }
            [Inject] private Logger   _InjectedLogger { get; set; }

            public IService NotInjected { get; set; }

            public Logger GetLogger() => _InjectedLogger;
        }

        private class FieldTarget
        {
            [Inject] public  IService service;
            [Inject] private Logger   _Logger;

            public Logger NotInjected;

            public Logger GetLogger() => _Logger;
        }

        private class MissingDependencyTarget
        {
            [Inject] public IService Service { get; set; }
        }

        private class CountingTarget
        {
            private IService _Service;

            [Inject]
            public IService Service
            {
                get => _Service;
                set
                {
                    _Service = value;
                    ++InjectCount;
                }
            }

            public int InjectCount { get; private set; }
        }

        private class SelfInjectingTarget
        {
            [Inject] public Injector Injector { get; set; }
        }

        #endregion Types

#pragma warning restore CS0649

        #region Tests

        /// <summary> Injector binds itself so it can be resolved and injected like any other dependency. </summary>
        [Test]
        [TestOf(nameof(Injector))]
        public void Injector_BindsItself()
        {
            // Arrange
            Injector            injector = new Injector();
            SelfInjectingTarget target   = new SelfInjectingTarget();

            // Act
            injector.Inject(target);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(injector.Get<Injector>(), Is.SameAs(injector));
                Assert.That(target.Injector, Is.SameAs(injector));
            });
        }

        /// <summary> Bound object is returned under the type it was bound with. </summary>
        [Test]
        [TestOf(nameof(Injector.Bind))]
        public void Injector_Bind_Get()
        {
            // Arrange
            Injector injector = new Injector();
            Service  service  = new Service();

            // Act
            injector.Bind<IService>(service);

            // Assert
            Assert.That(injector.Get<IService>(), Is.SameAs(service));
        }

        /// <summary> Binding uses the generic type argument, not the concrete runtime type. </summary>
        [Test]
        [TestOf(nameof(Injector.Bind))]
        public void Injector_Bind_UsesGenericTypeArgument()
        {
            // Arrange
            Injector injector = new Injector();
            Service  service  = new Service();

            // Act
            injector.Bind<IService>(service);

            // Assert
            Assert.Throws<KeyNotFoundException>(() => injector.Get<Service>());
        }

        /// <summary> Binding the same type twice replaces the previous binding. </summary>
        [Test]
        [TestOf(nameof(Injector.Bind))]
        public void Injector_Bind_Overwrites()
        {
            // Arrange
            Injector injector = new Injector();
            Service  first    = new Service();
            Service  second   = new Service();

            // Act
            injector.Bind<IService>(first);
            injector.Bind<IService>(second);

            // Assert
            Assert.That(injector.Get<IService>(), Is.SameAs(second));
        }

        /// <summary> Asking for a type that was never bound throws. </summary>
        [Test]
        [TestOf(nameof(Injector.Get))]
        public void Injector_Get_NotBound_Throws()
        {
            // Arrange
            Injector injector = new Injector();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => injector.Get<IService>());
        }

        /// <summary> All interfaces implemented by the object are bound to that single instance. </summary>
        [Test]
        [TestOf(nameof(Injector.BindAllInterfaces))]
        public void Injector_BindAllInterfaces()
        {
            // Arrange
            Injector injector = new Injector();
            Service  service  = new Service();

            // Act
            injector.BindAllInterfaces(service);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(injector.Get<IService>(), Is.SameAs(service));
                Assert.That(injector.Get<IOtherService>(), Is.SameAs(service));
                Assert.Throws<KeyNotFoundException>(() => injector.Get<Service>());
            });
        }

        /// <summary> An object without interfaces binds nothing. </summary>
        [Test]
        [TestOf(nameof(Injector.BindAllInterfaces))]
        public void Injector_BindAllInterfaces_NoInterfaces_BindsNothing()
        {
            // Arrange
            Injector injector = new Injector();
            Logger   logger   = new Logger();

            // Act
            injector.BindAllInterfaces(logger);

            // Assert
            Assert.Throws<KeyNotFoundException>(() => injector.Get<Logger>());
        }

        /// <summary> Public and private properties marked with the attribute are filled, others are left alone. </summary>
        [Test]
        [TestOf(nameof(Injector.Inject))]
        public void Injector_Inject_Properties()
        {
            // Arrange
            Injector       injector = new Injector();
            Service        service  = new Service();
            Logger         logger   = new Logger();
            PropertyTarget target   = new PropertyTarget();

            injector.Bind<IService>(service);
            injector.Bind(logger);

            // Act
            injector.Inject(target);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(target.Service, Is.SameAs(service));
                Assert.That(target.GetLogger(), Is.SameAs(logger));
                Assert.That(target.NotInjected, Is.Null);
            });
        }

        /// <summary> Public and private fields marked with the attribute are filled, others are left alone. </summary>
        [Test]
        [TestOf(nameof(Injector.Inject))]
        public void Injector_Inject_Fields()
        {
            // Arrange
            Injector    injector = new Injector();
            Service     service  = new Service();
            Logger      logger   = new Logger();
            FieldTarget target   = new FieldTarget();

            injector.Bind<IService>(service);
            injector.Bind(logger);

            // Act
            injector.Inject(target);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(target.service, Is.SameAs(service));
                Assert.That(target.GetLogger(), Is.SameAs(logger));
                Assert.That(target.NotInjected, Is.Null);
            });
        }

        /// <summary> Objects without any marked member are accepted and left untouched. </summary>
        [Test]
        [TestOf(nameof(Injector.Inject))]
        public void Injector_Inject_NoAttributes_DoesNothing()
        {
            // Arrange
            Injector injector = new Injector();
            Logger   logger   = new Logger();

            // Act & Assert
            Assert.DoesNotThrow(() => injector.Inject(logger));
        }

        /// <summary> Injecting a member whose type was never bound throws. </summary>
        [Test]
        [TestOf(nameof(Injector.Inject))]
        public void Injector_Inject_MissingDependency_Throws()
        {
            // Arrange
            Injector                injector = new Injector();
            MissingDependencyTarget target   = new MissingDependencyTarget();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => injector.Inject(target));
        }

        /// <summary> Every bound object gets its dependencies filled. </summary>
        [Test]
        [TestOf(nameof(Injector.TriggerInjectOnBinders))]
        public void Injector_TriggerInjectOnBinders()
        {
            // Arrange
            Injector       injector       = new Injector();
            Service        service        = new Service();
            Logger         logger         = new Logger();
            PropertyTarget propertyTarget = new PropertyTarget();
            FieldTarget    fieldTarget    = new FieldTarget();

            injector.Bind<IService>(service);
            injector.Bind(logger);
            injector.Bind(propertyTarget);
            injector.Bind(fieldTarget);

            // Act
            injector.TriggerInjectOnBinders();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(propertyTarget.Service, Is.SameAs(service));
                Assert.That(propertyTarget.GetLogger(), Is.SameAs(logger));
                Assert.That(fieldTarget.service, Is.SameAs(service));
                Assert.That(fieldTarget.GetLogger(), Is.SameAs(logger));
            });
        }

        /// <summary> An object bound under several types is injected only once. </summary>
        [Test]
        [TestOf(nameof(Injector.TriggerInjectOnBinders))]
        public void Injector_TriggerInjectOnBinders_InjectsEachObjectOnce()
        {
            // Arrange
            Injector       injector = new Injector();
            Service        service  = new Service();
            CountingTarget target   = new CountingTarget();

            injector.Bind<IService>(service);
            injector.Bind(target);
            injector.Bind<object>(target);

            // Act
            injector.TriggerInjectOnBinders();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(target.Service, Is.SameAs(service));
                Assert.That(target.InjectCount, Is.EqualTo(1));
            });
        }

        #endregion Tests
    }
}
