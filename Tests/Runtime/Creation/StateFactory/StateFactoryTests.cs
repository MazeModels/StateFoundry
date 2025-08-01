using System;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateFactoryTests
    {
        IStateFactory m_factory;

        [SetUp]
        public void SetUp()
        {
            m_factory = new StateFactory();
        }

        [Test]
        public void Given_ValidStateType_When_GetInstance_Then_ReturnsInstance()
        {
            IInternalState instance = m_factory.Build(typeof(DummyState));
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<DummyState>(instance);
        }

        [Test]
        public void Given_NullType_When_GetInstance_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => m_factory.Build(null));
        }

        [Test]
        public void Given_TypeNotAssignableToState_When_GetInstance_Then_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => m_factory.Build(typeof(string)));
        }

        [Test]
        public void Given_AbstractStateType_When_GetInstance_Then_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => m_factory.Build(typeof(AbstractDummyState)));
        }

        class DummyState : State { }
        abstract class AbstractDummyState : State { }
    }
}