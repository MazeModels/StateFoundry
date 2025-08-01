using System;
using NSubstitute;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateDataTests
    {
        Type m_stateType;
        StateMeta m_meta;
        IBlackboard m_blackboard;
        IStateFactory m_factory;
        IInternalState m_mockState;

        [SetUp]
        public void SetUp()
        {
            m_stateType = typeof(DummyState);
            m_meta = new StateMeta(m_stateType);
            m_blackboard = Substitute.For<IBlackboard>();
            m_mockState = Substitute.For<IInternalState>();

            m_factory = Substitute.For<IStateFactory>();
            m_factory.Build(m_stateType).Returns(m_mockState);
        }

        [Test]
        public void Given_StateDataCreated_Then_TypeIsSetCorrectly()
        {
            var data = new StateData(m_meta, m_blackboard, m_factory);
            Assert.AreEqual(m_stateType, data.Type);
        }

        [Test]
        public void Given_StateDataCreated_Then_BlackboardIsInjected()
        {
            _ = new StateData(m_meta, m_blackboard, m_factory);
            m_mockState.Received(1).Blackboard = m_blackboard;
        }

        [Test]
        public void Given_StateDataCreated_Then_StateIsSetFromFactory()
        {
            var data = new StateData(m_meta, m_blackboard, m_factory);
            Assert.AreSame(m_mockState, data.State);
        }

        [Test]
        public void Given_StateDataCreated_Then_ChildrenIsInitiallyEmpty()
        {
            var data = new StateData(m_meta, m_blackboard, m_factory);
            Assert.IsEmpty(data.Children);
        }

        [Test]
        public void Given_StateDataCreated_Then_TransitionsIsInitiallyEmpty()
        {
            var data = new StateData(m_meta, m_blackboard, m_factory);
            Assert.IsEmpty(data.Transitions);
        }

        [Test]
        public void Given_StateData_When_SetParent_Then_ParentIsAssigned()
        {
            var parent = new StateData(m_meta, m_blackboard, m_factory);
            var child = new StateData(m_meta, m_blackboard, m_factory);

            child.SetParent(parent);

            Assert.AreSame(parent, child.Parent);
        }

        [Test]
        public void Given_StateData_When_AddChild_Then_ChildAppearsInChildren()
        {
            var parent = new StateData(m_meta, m_blackboard, m_factory);
            var child = new StateData(m_meta, m_blackboard, m_factory);

            parent.AddChild(child);

            CollectionAssert.Contains(parent.Children, child);
        }

        [Test]
        public void Given_StateData_When_AddTransition_Then_TransitionIsStored()
        {
            var source = new StateData(m_meta, m_blackboard, m_factory);
            var destination = new StateData(m_meta, m_blackboard, m_factory);
            Type trigger = typeof(DummyTrigger);

            source.AddTransition(trigger, destination);

            Assert.AreSame(destination, source.Transitions[trigger]);
        }

        [Test]
        public void Given_StateData_When_Disposed_Then_StateIsDisposed()
        {
            var data = new StateData(m_meta, m_blackboard, m_factory);
            data.Dispose();
            m_mockState.Received(1).Dispose();
        }

        [Test]
        public void Given_StateData_When_ToString_Then_ReturnsStateToString()
        {
            m_factory.Build(m_stateType).Returns(new FakeState());
            var data = new StateData(m_meta, m_blackboard, m_factory);

            string result = data.ToString();

            Assert.AreEqual(nameof(FakeState), result);
        }

        class DummyState : State { }
        class DummyTrigger { }
        class FakeState : State
        {
            public override string ToString()
            {
                return nameof(FakeState);
            }
        }
    }
}