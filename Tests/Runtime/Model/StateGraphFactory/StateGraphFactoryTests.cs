using System;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateGraphFactoryTests
    {
        private StateGraphFactory m_factory;

        [SetUp]
        public void SetUp()
        {
            m_factory = new StateGraphFactory();
        }

        [Test]
        public void Given_NullType_Then_ThrowsArgumentNullException()
        {
            Type type = null;

            Assert.Throws<ArgumentNullException>(() => m_factory.Build(type));
        }

        [Test]
        public void Given_AbstractType_Then_ThrowsArgumentException()
        {
            Type type = typeof(AbstractState);

            Assert.Throws<ArgumentException>(() => m_factory.Build(type));
        }

        [Test]
        public void Given_NonStateType_Then_ThrowsArgumentException()
        {
            Type type = typeof(NotAState);

            Assert.Throws<ArgumentException>(() => m_factory.Build(type));
        }

        [Test]
        public void Given_ValidConcreteState_Then_ReturnsStateGraph()
        {
            Type type = typeof(ConcreteState);

            var result = m_factory.Build(type);

            Assert.IsInstanceOf<IStateGraph>(result);
        }

        abstract class AbstractState : State { }
        class ConcreteState : State { }
        class NotAState { }
    }
}
