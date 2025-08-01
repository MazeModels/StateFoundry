using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatePoolTests
    {
        IBlackboard m_blackboard;
        IStateFactory m_stateFactory;
        IStateDataFactory m_stateDataFactory;
        IStatechartNavigator m_navigator;
        IStateGraphFactory m_stateGraphFactory;

        StatePool<InitialState> m_sut;


        [SetUp]
        public void SetUp()
        {
            m_blackboard = Substitute.For<IBlackboard>();

            m_stateFactory = MockStateFactory();
            m_stateDataFactory = MockStateDataFactory();
            m_navigator = new StatechartNavigator();
            m_stateGraphFactory = new StateGraphFactory();

            m_sut = FlatStatePool();
        }

        [TearDown]
        public void TearDown()
        {
            m_sut?.Dispose();
        }


        [Test]
        public void Given_ValidConstruction_Then_StatesPropertyContainsAllInstantiatedStates()
        {
            using StatePool<LeafState1> sut = NestedStatePool();
            var states = sut.GetStates();

            Assert.IsNotNull(states);
            Assert.AreEqual(4, states.Count);
            Assert.IsTrue(states.ContainsKey(typeof(TopState)));
            Assert.IsTrue(states.ContainsKey(typeof(MidState)));
            Assert.IsTrue(states.ContainsKey(typeof(LeafState1)));
            Assert.IsTrue(states.ContainsKey(typeof(LeafState2)));
        }

        [Test]
        public void Given_AfterConstruction_Then_GetCurrentStateReturnsInitialState()
        {
            var state = m_sut.GetCurrentState();

            Assert.IsNotNull(state);
            Assert.AreEqual(typeof(InitialState), state.Type);
        }

        [Test]
        public void Given_SetCurrentState_Then_GetCurrentStateReturnsNewState()
        {
            using StatePool<LeafState1> sut = NestedStatePool();
            IStateData data = GetStateData<MidState, LeafState1>(sut);

            sut.SetCurrentState(data);
            var state = sut.GetCurrentState();

            Assert.IsNotNull(state);
            Assert.AreEqual(typeof(MidState), state.Type);
            Assert.AreEqual(state, data);
        }

        [Test]
        public void Given_SetCurrentState_And_NoPreviousState_Then_EnterFromRootIsCalled()
        {
            var states = m_sut.GetStates();


            foreach (var stateData in states.Values)
            {
                int callCount = stateData.Type == typeof(InitialState) ? 1 : 0;
                stateData.State.Received(callCount).InternalOnEnter();
                stateData.State.DidNotReceive().InternalOnExit();
            }

        }

        [Test]
        public void Given_SetCurrentState_And_NoCommonAncestor_Then_ExitToRootAndEnterFromRootAreCalled()
        {
            GetAllClearReceivedCalls(m_sut);
            IStateData data = GetStateData<MockState1, InitialState>(m_sut);

            m_sut.SetCurrentState(data);

            var states = m_sut.GetStates();
            states[typeof(InitialState)].State.Received(0).InternalOnEnter();
            states[typeof(InitialState)].State.Received(1).InternalOnExit();

            states[typeof(MockState1)].State.Received(1).InternalOnEnter();
            states[typeof(MockState1)].State.Received(0).InternalOnExit();

            states[typeof(MockState2)].State.Received(0).InternalOnEnter();
            states[typeof(MockState2)].State.Received(0).InternalOnExit();
        }

        [Test]
        public void Given_SetCurrentState_And_CommonAncestorExists_Then_ExitAndEnterAreCalledCorrectly()
        {
            using StatePool<LeafState1> sut = NestedStatePool();
            GetAllClearReceivedCalls(sut);

            IStateData data = GetStateData<LeafState2, LeafState1>(sut);
            sut.SetCurrentState(data);


            var states = sut.GetStates();
            states[typeof(TopState)].State.Received(0).InternalOnEnter();
            states[typeof(TopState)].State.Received(0).InternalOnExit();

            states[typeof(MidState)].State.Received(0).InternalOnEnter();
            states[typeof(MidState)].State.Received(0).InternalOnExit();

            states[typeof(LeafState1)].State.Received(0).InternalOnEnter();
            states[typeof(LeafState1)].State.Received(1).InternalOnExit();

            states[typeof(LeafState2)].State.Received(1).InternalOnEnter();
            states[typeof(LeafState2)].State.Received(0).InternalOnExit();
        }

        [Test]
        public void Given_SetCurrentState_And_SameStateAsCurrent_Then_NoCallbacksAreCalled()
        {
            var currentState = m_sut.GetCurrentState();
            GetAllClearReceivedCalls(m_sut);

            m_sut.SetCurrentState(currentState);

            var states = m_sut.GetStates();
            states[typeof(InitialState)].State.DidNotReceive().InternalOnEnter();
            states[typeof(InitialState)].State.DidNotReceive().InternalOnExit();

            states[typeof(MockState1)].State.DidNotReceive().InternalOnEnter();
            states[typeof(MockState1)].State.DidNotReceive().InternalOnExit();

            states[typeof(MockState2)].State.DidNotReceive().InternalOnEnter();
            states[typeof(MockState2)].State.DidNotReceive().InternalOnExit();

        }

        [Test]
        public void Given_Dispose_Then_AllStatesAreDisposed()
        {
            m_sut?.Dispose();

            foreach (IStateData state in m_sut.GetStates().Values)
            {
                state.Received(1).Dispose();
            }
        }

        [Test]
        public void Given_StateWithNonParameterlessConstructor_Then_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using StatePool<NonEmptyConstructorState> sut = NonEmptyConstructorStatePool();
            });

        }

        [Test]
        public void Given_NullBlackboard_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using StatePool<InitialState> sut = new StatePool<InitialState>(null, m_navigator, m_stateGraphFactory, m_stateFactory, m_stateDataFactory);
            });
        }

        [Test]
        public void Given_NullNavigator_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using StatePool<InitialState> sut = new StatePool<InitialState>(m_blackboard, null, m_stateGraphFactory, m_stateFactory, m_stateDataFactory);
            });
        }

        [Test]
        public void Given_NullGraphFactory_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using StatePool<InitialState> sut = new StatePool<InitialState>(m_blackboard, m_navigator, null, m_stateFactory, m_stateDataFactory);
            });
        }

        [Test]
        public void Given_NullStateFactory_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using StatePool<InitialState> sut = new StatePool<InitialState>(m_blackboard, m_navigator, m_stateGraphFactory, null, m_stateDataFactory);
            });
        }

        [Test]
        public void Given_NullStateDataFactory_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using StatePool<InitialState> sut = new StatePool<InitialState>(m_blackboard, m_navigator, m_stateGraphFactory, m_stateFactory, null);
            });
        }


        [Test]
        public void Given_ValidConstruction_And_ParentChildConnectionsExist_Then_SetupGraphLinksStatesCorrectly()
        {
            using StatePool<LeafState1> sut = NestedStatePool();

            var states = sut.GetStates();

            IStateData top = states[typeof(TopState)];
            IStateData mid = states[typeof(MidState)];
            IStateData leaf = states[typeof(LeafState1)];

            Assert.AreEqual(leaf.Parent, mid);
            Assert.IsTrue(mid.Children.Contains(leaf));

            Assert.AreEqual(mid.Parent, top);
            Assert.IsTrue(top.Children.Contains(mid));

            Assert.IsNull(top.Parent);
            Assert.AreEqual(0, leaf.Children.Count);
        }

        [Test]
        public void Given_ValidConstruction_And_TransitionsExist_Then_SetupGraphLinksTransitionsCorrectly()
        {
            using StatePool<LeafState1> sut = NestedStatePool();

            var states = sut.GetStates();

            IStateData top = states[typeof(TopState)];
            IStateData mid = states[typeof(MidState)];
            IStateData leaf1 = states[typeof(LeafState1)];
            IStateData leaf2 = states[typeof(LeafState2)];

            Assert.IsTrue(mid.Transitions.ContainsKey(typeof(Trigger1)));
            Assert.AreEqual(leaf1, mid.Transitions[typeof(Trigger1)]);
            
            Assert.IsTrue(mid.Transitions.ContainsKey(typeof(Trigger2)));
            Assert.AreEqual(leaf2, mid.Transitions[typeof(Trigger2)]);
        }


        StatePool<InitialState> FlatStatePool()
        {
            return new StatePool<InitialState>(m_blackboard, m_navigator, m_stateGraphFactory, m_stateFactory, m_stateDataFactory);
        }

        StatePool<LeafState1> NestedStatePool()
        {
            return new StatePool<LeafState1>(m_blackboard, m_navigator, m_stateGraphFactory, m_stateFactory, m_stateDataFactory);
        }

        StatePool<NonEmptyConstructorState> NonEmptyConstructorStatePool()
        {
            return new StatePool<NonEmptyConstructorState>(m_blackboard, m_navigator, m_stateGraphFactory, m_stateFactory, m_stateDataFactory);
        }

        IStateFactory MockStateFactory()
        {
            var mockFactory = Substitute.For<IStateFactory>();

            mockFactory.Build(Arg.Any<Type>()).Returns(info => Substitute.For<IInternalState>());

            return mockFactory;
        }

        static IStateDataFactory MockStateDataFactory()
        {
            var mockFactory = Substitute.For<IStateDataFactory>();

            mockFactory.Build(Arg.Any<IStateMeta>(), Arg.Any<IBlackboard>(), Arg.Any<IStateFactory>()).Returns(info =>
            {
                var concreteData = new StateData(info.Arg<IStateMeta>(), info.Arg<IBlackboard>(), info.Arg<IStateFactory>());
                var mockData = Substitute.For<IStateData>();

                mockData.Type.Returns(concreteData.Type);
                mockData.State.Returns(concreteData.State);
                mockData.Parent.Returns(concreteData.Parent);
                mockData.Transitions.Returns(concreteData.Transitions);
                mockData.Children.Returns(concreteData.Children);

                UpdateParentWhenRequested(mockData);
                UpdateTransitionsWhenRequested(concreteData, mockData);
                UpdateChildrenWhenRequested(concreteData, mockData);

                return mockData;
            });

            return mockFactory;
        }

        static void UpdateParentWhenRequested(IStateData mockData)
        {
            mockData.When(x => x.SetParent(Arg.Any<IStateData>()))
                    .Do(callInfo =>
                    {
                        mockData.Parent.Returns(callInfo.Arg<IStateData>());
                    });
        }

        static void UpdateTransitionsWhenRequested(StateData concreteData, IStateData mockData)
        {
            mockData.When(x => x.AddTransition(Arg.Any<Type>(), Arg.Any<IStateData>()))
                    .Do(callInfo =>
                    {
                        var trigger = callInfo.Arg<Type>();
                        var destination = callInfo.Arg<IStateData>();
                        concreteData.AddTransition(trigger, destination);
                        mockData.Transitions.Returns(concreteData.Transitions);
                    });
        }

        static void UpdateChildrenWhenRequested(StateData concreteData, IStateData mockData)
        {
            mockData.When(x => x.AddChild(Arg.Any<IStateData>()))
                    .Do(callInfo =>
                    {
                        concreteData.AddChild(callInfo.Arg<IStateData>());
                        mockData.Children.Returns(concreteData.Children);
                    });
        }

        static IStateData GetStateData<TState, TInitialState>(StatePool<TInitialState> sut) where TState : State, new()
                                                                                            where TInitialState : State, new()
        {
            var states = sut.GetStates();
            return states[typeof(TState)];
        }

        static void GetAllClearReceivedCalls<TInitialState>(StatePool<TInitialState> sut) where TInitialState : State, new()
        {
            foreach (var stateData in sut.GetStates().Values)
            {
                stateData.State.ClearReceivedCalls();
            }
        }


        sealed class InitialState : State, IGet<Trigger1, MockState1>, IGet<Trigger2, MockState2> { }
        sealed class MockState1 : State { }
        sealed class MockState2 : State { }
        class TopState : State { }
        class MidState : TopState, IGet<Trigger1, LeafState1>, IGet<Trigger2, LeafState2> { }
        class LeafState1 : MidState { }
        class LeafState2 : MidState { }
        struct Trigger1 : ITrigger { }
        struct Trigger2 : ITrigger { }
        sealed class NonEmptyConstructorState : State
        {
            public NonEmptyConstructorState() { }
            public NonEmptyConstructorState(bool flag) { }
        }
    }
}
