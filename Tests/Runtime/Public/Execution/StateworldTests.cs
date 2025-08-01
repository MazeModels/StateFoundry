using NUnit.Framework;
using System;

namespace Maze.StateFoundry.Tests
{
    class StateworldTests
    {
        MockStatechart m_statechart;
        Stateworld m_sut;

        [SetUp]
        public void SetUp()
        {
            m_statechart = new MockStatechart();
            m_sut = new Stateworld(m_statechart);
            m_sut.Start();
        }

        [TearDown]
        public void TearDown()
        {
            m_statechart?.Dispose();
            m_sut?.Dispose();
        }


        [Test]
        public void Given_TriggerListener_When_TriggerSent_Then_CallbackInvoked()
        {
            var received = false;

            m_sut.Listen<TriggerB>(_ => received = true);
            m_sut.Send(new TriggerB());

            Assert.IsTrue(received);
        }

        [Test]
        public void Given_SendCalledBeforeStart_Then_ThrowsInvalidOperationException()
        {
            using var sut = new Stateworld(new MockStatechart());

            Assert.Throws<InvalidOperationException>(() => sut.Send(new TriggerB()));
        }

        [Test]
        public void Given_OnEnterListener_When_EnteringState_Then_CallbackInvoked()
        {
            var called = false;

            m_sut.OnEnter<StateB>(_ => called = true);
            m_sut.Send(new TriggerB());

            Assert.IsTrue(called);
        }

        [Test]
        public void Given_OnExitListener_When_ExitingState_Then_CallbackInvoked()
        {
            var called = false;

            m_sut.OnExit<StateA>(_ => called = true);
            m_sut.Send(new TriggerB());

            Assert.IsTrue(called);
        }

        [Test]
        public void Given_OnDisposeListener_When_StateDisposed_Then_CallbackInvoked()
        {
            var called = false;

            m_sut.OnDispose<StateA>(_ => called = true);
            m_statechart?.Dispose();
            m_statechart = null;

            Assert.IsTrue(called);
        }

        [Test]
        public void Given_OnCreateListener_When_StateCreated_Then_CallbackInvoked()
        {
            using var sut = new Stateworld(m_statechart);
            var called = false;

            sut.OnCreate<StateA>(_ => called = true);
            sut.Start();

            Assert.IsTrue(called);
        }
    }

    class MockStatechart : Statechart<StateA> { }
    struct TriggerA : ITrigger { }
    struct TriggerB : ITrigger { }
    class StateA : State, IGet<TriggerB, StateB> { }
    class StateB : State, IGet<TriggerA, StateA> { }
}