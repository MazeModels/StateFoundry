using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatechartEventsTests
    {
        TriggerA m_trigger;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_trigger = new TriggerA();
        }


        [Test]
        public void Given_TriggerWithNoMatchingTransition_Then_SendReturnsNull()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);
            var result = sut.Send(m_trigger);

            Assert.IsNull(result);
        }

        [Test]
        public void Given_TriggerHandledByCurrentState_Then_SendReturnsTargetStateData()
        {
            var data = InitStateData<TransitionState>();
            var pool = InitPool<TransitionState>(data);

            var targetData = Substitute.For<IStateData>();
            pool.GetStates().Returns(new Dictionary<Type, IStateData>
            {
                { typeof(TransitionState), data },
                { typeof(TargetState), targetData }
            });

            var sut = new StatechartEvents<TransitionState>(pool);
            var result = sut.Send(m_trigger);

            Assert.AreSame(targetData, result);
        }

        [Test]
        public void Given_CallbackRegistered_And_StateEmitsTrigger_Then_CallbackIsInvoked()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            TriggerA received = default;
            sut.Listen<TriggerA>(t => received = t);

            data.State.Send(m_trigger);

            Assert.AreEqual(m_trigger, received);
        }

        [Test]
        public void Given_MultipleCallbacksRegistered_And_StateEmitsTrigger_Then_AllCallbacksAreInvoked()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            int m_counter = 0;
            sut.Listen<TriggerA>(_ => m_counter++);
            sut.Listen<TriggerA>(_ => m_counter++);

            data.State.Send(m_trigger);

            Assert.AreEqual(2, m_counter);
        }

        [Test]
        public void Given_LifecycleCallbackRegistered_And_EventFires_Then_CallbackIsInvoked()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            NoTransitionState m_callbackArg = null;
            sut.OnLifecycleEvent<NoTransitionState>(When.OnEnter, s => m_callbackArg = s);

            data.State.InternalOnEnter();

            Assert.AreSame(data.State, m_callbackArg);
        }

        [Test]
        public void Given_TriggerTargetsUnregisteredState_Then_SendThrows()
        {
            var data = InitStateData<TransitionState>();
            var pool = InitPool<TransitionState>(data);

            var sut = new StatechartEvents<TransitionState>(pool);

            Assert.Throws<ArgumentException>(() => sut.Send(m_trigger));
        }

        [Test]
        public void Given_CallbackRegistered_And_DisposeCalled_Then_CallbackNotInvokedAfterwards()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            bool called = false;
            sut.Listen<TriggerA>(_ => called = true);

            sut.Dispose();
            data.State.Send(m_trigger);

            Assert.IsFalse(called);
        }

        [Test]
        public void Given_TriggerHandledByParentState_Then_SendReturnsTargetStateData()
        {
            var parentData = InitStateData<TransitionState>();
            var childData = InitStateData<NoTransitionState>();
            childData.Parent.Returns(parentData);

            var pool = InitPool<NoTransitionState>(childData);
            var targetData = Substitute.For<IStateData>();

            pool.GetStates().Returns(new Dictionary<Type, IStateData>
            {
                { typeof(NoTransitionState), childData },
                { typeof(TransitionState), parentData },
                { typeof(TargetState), targetData }
            });

            var sut = new StatechartEvents<NoTransitionState>(pool);
            var result = sut.Send(m_trigger);

            Assert.AreSame(targetData, result);
        }

        [Test]
        public void Given_NoCallbackRegisteredForTrigger_Then_TriggerIsIgnored()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            Assert.DoesNotThrow(() => data.State.Send(m_trigger));
        }

        [Test]
        public void Given_NullCallbackPassedToListen_Then_ThrowsArgumentNullException()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            Assert.Throws<ArgumentNullException>(() => sut.Listen<TriggerA>(null));
        }

        [Test]
        public void Given_NullCallbackPassedToOnLifecycleEvent_Then_ThrowsArgumentNullException()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            Assert.Throws<ArgumentNullException>(() => sut.OnLifecycleEvent<NoTransitionState>(When.OnEnter, null));
        }

        [Test]
        public void Given_StateNotRegistered_Then_OnLifecycleEventThrowsArgumentException()
        {
            var data = InitStateData<NoTransitionState>();
            var pool = InitPool<NoTransitionState>(data);

            var sut = new StatechartEvents<NoTransitionState>(pool);

            Assert.Throws<ArgumentException>(() => sut.OnLifecycleEvent<TargetState>(When.OnEnter, _ => { }));
        }

        [Test]
        public void Given_StateImplementsMultipleIGet_Then_OnlyCorrectGetIsInvoked()
        {
            var data = InitStateData<MultiTransitionState>();
            var target = InitStateData<TargetState>();
            var nonTarget = InitStateData<NonTargetState>();
            var pool = InitPool<MultiTransitionState>(data);

            pool.GetStates().Returns(new Dictionary<Type, IStateData>
            {
                { typeof(MultiTransitionState), data },
                { typeof(TargetState), target },
                { typeof(NonTargetState), nonTarget }
            });

            var sut = new StatechartEvents<MultiTransitionState>(pool);

            var triggerA = new TriggerA();
            var result = sut.Send(triggerA);

            Assert.AreSame(target, result);
            Assert.AreNotSame(nonTarget, result);
        }



        static IStateData InitStateData<TInitialState>() where TInitialState : State, new()
        {
            var initialState = new TInitialState();

            var data = Substitute.For<IStateData>();
            data.Parent.Returns((IStateData)null);
            data.State.Returns(initialState);

            return data;
        }

        static IStatePool<TInitialState> InitPool<TInitialState>(IStateData data) where TInitialState : State, new()
        {
            var pool = Substitute.For<IStatePool<TInitialState>>();

            pool.GetCurrentState().Returns(data);
            pool.GetStates().Returns(new Dictionary<Type, IStateData>
        {
            { typeof(TInitialState), data }
        });

            return pool;
        }


        public class TargetState : State { }
        public class NonTargetState : State { }
        public class NoTransitionState : State { }
        public class TransitionState : State, IGet<TriggerA, TargetState> { }
        public class MultiTransitionState : State, IGet<TriggerA, TargetState>, IGet<TriggerB, TargetState> { }

        public struct TriggerA : ITrigger { }
        public struct TriggerB : ITrigger { }
    }
}
