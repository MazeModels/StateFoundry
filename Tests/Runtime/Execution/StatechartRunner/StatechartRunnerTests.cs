using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatechartRunnerTests
    {
        IBlackboard m_blackboard;
        IStatePool<InitialState> m_pool;
        IStatechartEvents<InitialState> m_events;
        IStateData m_stateData;

        DummyState m_state;
        StatechartRunner<InitialState> m_sut;

        [SetUp]
        public void SetUp()
        {
            m_blackboard = Substitute.For<IBlackboard>();
            m_pool = Substitute.For<IStatePool<InitialState>>();
            m_events = Substitute.For<IStatechartEvents<InitialState>>();

            m_stateData = Substitute.For<IStateData>();
            m_state = new DummyState();
            m_stateData.State.Returns(m_state);

            m_pool.GetStates().Returns(new Dictionary<Type, IStateData> { { typeof(InitialState), m_stateData } });
            m_pool.GetCurrentState().Returns(m_stateData);

            m_sut = new StatechartRunner<InitialState>(typeof(ValidStatechart), m_blackboard, m_pool, m_events);
            m_sut.Start();
        }

        [Test]
        public void Given_NullStatechartType_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StatechartRunner<InitialState>(null, m_blackboard, m_pool, m_events));
        }

        [Test]
        public void Given_NullBlackboard_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StatechartRunner<InitialState>(typeof(ValidStatechart), null, m_pool, m_events));
        }

        [Test]
        public void Given_NullPool_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StatechartRunner<InitialState>(typeof(ValidStatechart), m_blackboard, null, m_events));
        }

        [Test]
        public void Given_NullEvents_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StatechartRunner<InitialState>(typeof(ValidStatechart), m_blackboard, m_pool, null));
        }

        [Test]
        public void Given_AbstractStatechartType_Then_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StatechartRunner<InitialState>(typeof(AbstractStatechart), m_blackboard, m_pool, m_events));
        }

        [Test]
        public void Given_NonStatechartType_Then_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StatechartRunner<InitialState>(typeof(string), m_blackboard, m_pool, m_events));
        }

        [Test]
        public void Given_SendCalledBeforeStart_Then_ThrowsInvalidOperationException()
        {
            using var sut = new StatechartRunner<InitialState>(typeof(ValidStatechart), m_blackboard, m_pool, m_events);
            var trigger = new DummyTrigger();

            Assert.Throws<InvalidOperationException>(() => sut.Send(trigger));
        }

        [Test]
        public void Given_StartIsCalled_Then_CallsInternalOnCreateOnAllStates()
        {
            Assert.IsTrue(m_state.OnCreateCalled);
        }

        [Test]
        public void Given_SendAndNewDataIsNull_Then_DoesNotSetNewState()
        {
            var trigger = new DummyTrigger();
            m_events.Send(trigger).Returns((IStateData)null);
            m_sut.Send(trigger);
            m_pool.DidNotReceive().SetCurrentState(Arg.Any<IStateData>());
        }

        [Test]
        public void Given_SendAndNewDataIsValid_Then_SetsCurrentState()
        {
            var trigger = new DummyTrigger();
            var newState = Substitute.For<IStateData>();
            m_events.Send(trigger).Returns(newState);
            m_sut.Send(trigger);
            m_pool.Received(1).SetCurrentState(newState);
        }

        [Test]
        public void Given_ListenCallbackRegistered_And_TriggerIsRaised_Then_CallbackIsInvoked()
        {
            bool invoked = false;
            Action<DummyTrigger> captured = null;
            m_events.When(x => x.Listen(Arg.Any<Action<DummyTrigger>>()))
                    .Do(call => captured = call.Arg<Action<DummyTrigger>>());

            m_sut.Listen<DummyTrigger>(_ => invoked = true);
            captured?.Invoke(new DummyTrigger());

            Assert.IsTrue(invoked);
        }

        [Test]
        public void Given_OnEnterCallback_Then_DelegatesToEvents()
        {
            Action<InitialState> callback = _ => { };
            m_sut.OnEnter(callback);
            m_events.Received(1).OnLifecycleEvent(When.OnEnter, callback);
        }

        [Test]
        public void Given_OnExitCallback_Then_DelegatesToEvents()
        {
            Action<InitialState> callback = _ => { };
            m_sut.OnExit(callback);
            m_events.Received(1).OnLifecycleEvent(When.OnExit, callback);
        }

        [Test]
        public void Given_OnCreateCallback_Then_DelegatesToEvents()
        {
            Action<InitialState> callback = _ => { };
            m_sut.OnCreate(callback);
            m_events.Received(1).OnLifecycleEvent(When.OnCreate, callback);
        }

        [Test]
        public void Given_OnDisposeCallback_Then_DelegatesToEvents()
        {
            Action<InitialState> callback = _ => { };
            m_sut.OnDispose(callback);
            m_events.Received(1).OnLifecycleEvent(When.OnDispose, callback);
        }

        [Test]
        public void Given_AddCalled_Then_DelegatesToBlackboard()
        {
            var obj = new object();
            m_sut.Add(obj);
            m_blackboard.Received(1).Add(obj);
        }

        [Test]
        public void Given_GetCalled_Then_ReturnsFromBlackboard()
        {
            var obj = new object();
            m_blackboard.Get<object>().Returns(obj);
            var result = m_sut.Get<object>();
            Assert.AreSame(obj, result);
        }

        [Test]
        public void Given_InternalSendIsRaised_Then_OnTriggerIsInvoked()
        {
            bool invoked = false;
            m_sut.OnTrigger += _ => invoked = true;
            m_state.Send(new DummyTrigger());
            Assert.IsTrue(invoked);
        }

        [Test]
        public void Given_DisposeIsCalled_Then_UnsubscribesAndDisposesPool()
        {
            m_sut.Dispose();
            m_pool.Received(1).Dispose();
            bool triggered = false;
            m_sut.OnTrigger += _ => triggered = true;
            m_state.Send(new DummyTrigger());
            Assert.IsFalse(triggered);
        }

        public class InitialState : State { }
        sealed class ValidStatechart : Statechart<InitialState> { }
        abstract class AbstractStatechart : Statechart<InitialState> { }
        struct DummyTrigger : ITrigger { }
        sealed class DummyState : State 
        {
            public bool OnCreateCalled { get; set; }

            public override void OnCreate()
            {
                OnCreateCalled = true;
            }
        }
    }
}
