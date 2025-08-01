using NUnit.Framework;
using System;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatechartIntegrationTests
    {
        MockStatechart m_sut;

        [SetUp]
        public void Setup()
        {
            m_sut = new MockStatechart();
            m_sut.Start();
        }

        [TearDown]
        public void Teardown()
        {
            m_sut.Dispose();
        }


        [Test]
        public void Given_TriggerListener_When_TriggerSent_Then_CallbackIsInvoked()
        {
            var wasCalled = false;
            m_sut.Listen<MockTrigger>(_ => wasCalled = true);

            m_sut.Send(new MockTrigger());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Given_SendCalledBeforeStart_Then_ThrowsInvalidOperationException()
        {
            using var sut = new MockStatechart();

            Assert.Throws<InvalidOperationException>(() => sut.Send(new MockTrigger()));
        }

        [Test]
        public void Given_EnterCallback_When_InitialStateEntered_Then_CallbackIsInvoked()
        {
            var wasCalled = false;
            m_sut.OnEnter<MockNextState>(_ => wasCalled = true);

            m_sut.Send(new MockTrigger());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Given_Component_When_AddedAndRetrieved_Then_SameInstanceIsReturned()
        {
            var component = new MockComponent();

            m_sut.Add(component);

            var retrieved = m_sut.Get<MockComponent>();

            Assert.AreEqual(component, retrieved);
        }

        [Test]
        public void Given_OnCreateCallback_When_InitialStateCreated_Then_CallbackIsInvoked()
        {
            using var sut = new MockStatechart();
            var wasCalled = false;

            sut.OnCreate<MockInitialState>(_ => wasCalled = true);
            sut.Start();

            sut.Send(new MockTrigger());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Given_OnDisposeCallback_When_StatechartDisposed_Then_CallbackIsInvoked()
        {
            var wasCalled = false;
            m_sut.OnDispose<MockInitialState>(_ => wasCalled = true);

            m_sut.Dispose();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Given_ExitCallback_When_TriggerCausesExit_Then_CallbackIsInvoked()
        {
            var wasCalled = false;
            m_sut.OnExit<MockInitialState>(_ => wasCalled = true);

            m_sut.Send(new MockTrigger());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Given_Statechart_When_CastToInternalInterface_Then_ExposesRunnerInstance()
        {
            var runner = ((IInternalStatechart)m_sut).Runner;

            Assert.IsNotNull(runner);
        }


        class MockStatechart : Statechart<MockInitialState> { }
        class MockInitialState : State, IGet<MockTrigger, MockNextState> { }
        class MockNextState : State { }
        struct MockTrigger : ITrigger { }
        class MockComponent { }
    }
}

