using NSubstitute;
using NUnit.Framework;
using System;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateDataFactoryTests
    {
        StateDataFactory m_sut;


        [SetUp]
        public void SetUp()
        {
            m_sut = new StateDataFactory();
        }


        [Test]
        public void Given_ValidArguments_Then_ReturnsStateDataInstance()
        {
            var meta = Substitute.For<IStateMeta>();
            var blackboard = Substitute.For<IBlackboard>();
            var factory = Substitute.For<IStateFactory>();

            var result = m_sut.Build(meta, blackboard, factory);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IStateData>(result);
        }

        [Test]
        public void Given_NullMeta_Then_ThrowsArgumentNullException()
        {
            IStateMeta meta = null;
            var blackboard = Substitute.For<IBlackboard>();
            var factory = Substitute.For<IStateFactory>();

            Assert.Throws<ArgumentNullException>(() => m_sut.Build(meta, blackboard, factory));
        }

        [Test]
        public void Given_NullBlackboard_Then_ThrowsArgumentNullException()
        {
            var meta = Substitute.For<IStateMeta>();
            IBlackboard blackboard = null;
            var factory = Substitute.For<IStateFactory>();

            Assert.Throws<ArgumentNullException>(() => m_sut.Build(meta, blackboard, factory));
        }

        [Test]
        public void Given_NullStateFactory_Then_ThrowsArgumentNullException()
        {
            var meta = Substitute.For<IStateMeta>();
            var blackboard = Substitute.For<IBlackboard>();
            IStateFactory factory = null;

            Assert.Throws<ArgumentNullException>(() => m_sut.Build(meta, blackboard, factory));
        }
    }
}
