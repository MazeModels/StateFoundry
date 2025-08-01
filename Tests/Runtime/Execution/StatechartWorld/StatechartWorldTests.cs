using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatechartWorldTests
    {
        IInternalStatechart m_chart1;
        IInternalStatechart m_chart2;
        IStatechartRunner m_runner1;
        IStatechartRunner m_runner2;

        StatechartWorld m_statechartWorld;

        [SetUp]
        public void SetUp()
        {
            m_chart1 = Substitute.For<IInternalStatechart>();
            m_chart2 = Substitute.For<IInternalStatechart>();
            m_runner1 = Substitute.For<IStatechartRunner>();
            m_runner2 = Substitute.For<IStatechartRunner>();

            m_chart1.Runner.Returns(m_runner1);
            m_chart2.Runner.Returns(m_runner2);

            m_statechartWorld = new StatechartWorld(new List<IStatechart> { m_chart1, m_chart2 });
        }

        [TearDown]
        public void TearDown()
        {
            m_statechartWorld.Dispose();
        }


        [Test]
        public void Given_WorldWithTwoCharts_And_SendTrigger_Then_TriggerIsSentToEachChart()
        {
            var trigger = new DummyTrigger();

            m_statechartWorld.Send(trigger);

            m_chart1.Received(1).Send(trigger);
            m_chart2.Received(1).Send(trigger);
        }

        [Test]
        public void Given_WorldWithTwoCharts_When_StartCalled_Then_StartIsCalledOnEachChart()
        {
            m_statechartWorld.Start();

            m_chart1.Received(1).Start();
            m_chart2.Received(1).Start();
        }

        [Test]
        public void Given_WorldWithTwoCharts_And_ListenCallback_Then_ListenIsForwardedToEachChart()
        {
            Action<DummyTrigger> cb = _ => { };

            m_statechartWorld.Listen(cb);

            m_chart1.Received(1).Listen(cb);
            m_chart2.Received(1).Listen(cb);
        }

        [Test]
        public void Given_WorldWithTwoCharts_And_OnEnterCallback_Then_CallbackIsForwarded()
        {
            Action<DummyState> cb = _ => { };

            m_statechartWorld.OnEnter(cb);

            m_chart1.Received(1).OnEnter(cb);
            m_chart2.Received(1).OnEnter(cb);
        }

        [Test]
        public void Given_WorldWithTwoCharts_And_OnExitCallback_Then_CallbackIsForwarded()
        {
            Action<DummyState> cb = _ => { };

            m_statechartWorld.OnExit(cb);

            m_chart1.Received(1).OnExit(cb);
            m_chart2.Received(1).OnExit(cb);
        }

        [Test]
        public void Given_WorldWithTwoCharts_And_OnCreateCallback_Then_CallbackIsForwarded()
        {
            Action<DummyState> cb = _ => { };

            m_statechartWorld.OnCreate(cb);

            m_chart1.Received(1).OnCreate(cb);
            m_chart2.Received(1).OnCreate(cb);
        }

        [Test]
        public void Given_WorldWithTwoCharts_And_OnDisposeCallback_Then_CallbackIsForwarded()
        {
            Action<DummyState> cb = _ => { };

            m_statechartWorld.OnDispose(cb);

            m_chart1.Received(1).OnDispose(cb);
            m_chart2.Received(1).OnDispose(cb);
        }

        [Test]
        public void Given_TwoCharts_And_TriggerRaisedByChart1_Then_TriggerRelayedOnlyToChart2()
        {
            var trigger = new DummyTrigger();

            m_runner1.OnTrigger += Raise.Event<Action<ITrigger>>(trigger);

            m_chart2.Received(1).Send(trigger);
            m_chart2.Received(1).Send(trigger);
            m_chart1.DidNotReceive().Send(trigger);
        }

        [Test]
        public void Given_WorldDisposed_And_TriggerRaised_Then_NoChartReceivesAnything()
        {
            m_statechartWorld.Dispose();

            var trigger = new DummyTrigger();
            m_runner1.OnTrigger += Raise.Event<Action<ITrigger>>(trigger);

            m_chart1.DidNotReceive().Send(trigger);
            m_chart2.DidNotReceive().Send(trigger);
        }

        struct DummyTrigger : ITrigger { }
        class DummyState : State { }
    }
}
