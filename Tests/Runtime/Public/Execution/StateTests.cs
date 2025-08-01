using NSubstitute;
using NUnit.Framework;
using System;


namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateTests
    {
        const string TEST_STRING = "abc";
        
        ConcreteState m_sut;
        IInternalState m_internal;
        IBlackboard m_blackboard;


        [SetUp]
        public void Setup()
        {
            m_sut = new ConcreteState();
            m_internal = m_sut;

            m_blackboard = Substitute.For<IBlackboard>();
            m_internal.Blackboard = m_blackboard;
        }

        [TearDown]
        public void TearDown()
        {
            m_sut?.Dispose();
        }


        [Test]
        public void Given_NewInstance_Then_OnCreate_IsCalled()
        {
            Assert.IsTrue(m_sut.CreateCalled);
        }

        [Test]
        public void Given_DisposeCalled_Then_OnDispose_IsCalled()
        {
            m_sut.Dispose();
            Assert.IsTrue(m_sut.DisposeCalled);
        }

        [Test]
        public void Given_ToStringCalled_Then_ReturnsClassName()
        {
            var result = m_sut.ToString();
            Assert.AreEqual(nameof(ConcreteState), result);
        }

        [Test]
        public void Given_TriggerSent_Then_EventIsRaised()
        {
            var received = false;
            m_internal.OnEventSent += _ => received = true;

            m_sut.Send(new DummyTrigger());

            Assert.IsTrue(received);
        }

        [Test]
        public void Given_InternalOnEnterCalled_Then_OnEnterIsCalled_And_EventRaised()
        {
            var received = false;
            m_internal.OnLifecycleEvent += when => received = when == When.OnEnter;

            m_internal.InternalOnEnter();

            Assert.IsTrue(m_sut.EnterCalled);
            Assert.IsTrue(received);
        }

        [Test]
        public void Given_InternalOnExitCalled_Then_OnExitIsCalled_And_EventRaised()
        {
            var received = false;
            m_internal.OnLifecycleEvent += when => received = when == When.OnExit;

            m_internal.InternalOnExit();

            Assert.IsTrue(m_sut.ExitCalled);
            Assert.IsTrue(received);
        }

        [Test]
        public void Given_InternalOnDisposeCalled_Then_OnDisposeIsCalled_And_EventRaised()
        {
            var received = false;
            m_internal.OnLifecycleEvent += when => received = when == When.OnDispose;

            m_internal.InternalOnDispose();

            Assert.IsTrue(m_sut.DisposeCalled);
            Assert.IsTrue(received);
        }

        [Test]
        public void Given_ComponentAdded_Then_DelegatesToBlackboard()
        {
            var component = new object();
            m_sut.Add(component);
            m_blackboard.Received(1).Add(component);
        }

        [Test]
        public void Given_ComponentRequested_Then_DelegatesToBlackboard()
        {
            m_blackboard.Get<string>().Returns(TEST_STRING);
            var result = m_sut.Get<string>();
            Assert.AreEqual(TEST_STRING, result);
        }

        [Test]
        public void Given_SubscriberAddedAndRemoved_Then_RemovePreventsEventInvocation()
        {
            bool received = false;
            Action<When> handler = when => received = true;

            m_internal.OnLifecycleEvent += handler;
            m_internal.OnLifecycleEvent -= handler;

            m_internal.InternalOnEnter();

            Assert.IsFalse(received);
        }

        struct DummyTrigger : ITrigger { }

        class ConcreteState : State
        {
            public bool EnterCalled;
            public bool ExitCalled;
            public bool CreateCalled;
            public bool DisposeCalled;

            public override void OnEnter()
            {
                base.OnEnter();
                EnterCalled = true;
            }

            public override void OnExit()
            {
                base.OnExit();
                ExitCalled = true;
            }

            public override void OnCreate()
            {
                base.OnCreate();
                CreateCalled = true;
            }

            public override void OnDispose()
            {
                base.OnDispose();
                DisposeCalled = true;
            }
        }
    }

}
