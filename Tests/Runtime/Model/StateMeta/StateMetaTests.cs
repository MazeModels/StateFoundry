using System;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateMetaTests
    {
        Type m_exampleType;

        [SetUp]
        public void SetUp()
        {
            m_exampleType = typeof(DummyState);
        }

        [Test]
        public void Given_NewStateMeta_Then_TypeIsSetCorrectly()
        {
            var state = new StateMeta(m_exampleType);
            Assert.AreEqual(m_exampleType, state.Type);
        }

        [Test]
        public void Given_NewStateMeta_Then_ChildrenIsInitiallyEmpty()
        {
            var state = new StateMeta(m_exampleType);

            Assert.IsNotNull(state.Children);
            Assert.IsEmpty(state.Children);
        }

        [Test]
        public void Given_NewStateMeta_Then_TransitionsIsInitiallyEmpty()
        {
            var state = new StateMeta(m_exampleType);

            Assert.IsNotNull(state.Transitions);
            Assert.IsEmpty(state.Transitions);
        }

        [Test]
        public void Given_NewStateMeta_Then_DirectTransitionsIsInitiallyEmpty()
        {
            var state = new StateMeta(m_exampleType);

            Assert.IsNotNull(state.DirectTransition);
            Assert.IsEmpty(state.DirectTransition);
        }

        [Test]
        public void Given_NewStateMeta_Then_CaptionsIsInitiallyEmpty()
        {
            var state = new StateMeta(m_exampleType);

            Assert.IsNotNull(state.Captions);
            Assert.IsEmpty(state.Captions);
        }

        [Test]
        public void Given_StateMeta_When_SetParentCalled_Then_ParentIsAssigned()
        {
            var parent = new StateMeta(typeof(DummyParentState));
            var child = new StateMeta(m_exampleType);

            child.SetParent(parent);

            Assert.AreEqual(parent, child.Parent);
        }

        [Test]
        public void Given_StateMeta_When_AddChildCalled_Then_ChildIsInChildrenList()
        {
            var parent = new StateMeta(m_exampleType);
            var child = new StateMeta(typeof(DummyChildState));

            parent.AddChild(child);

            CollectionAssert.Contains(parent.Children, child);
        }

        [Test]
        public void Given_StateMeta_When_AddTransitionCalled_Then_TransitionIsStored()
        {
            var source = new StateMeta(m_exampleType);
            var target = new StateMeta(typeof(DummyTargetState));
            Type trigger = typeof(DummyTrigger);

            source.AddTransition(trigger, target);

            Assert.IsTrue(source.Transitions.ContainsKey(trigger));
            Assert.AreEqual(target, source.Transitions[trigger]);
        }

        [Test]
        public void Given_StateMeta_When_AddDirectTransitionCalled_Then_DirectTransitionIsStored()
        {
            var source = new StateMeta(m_exampleType);
            var target = new StateMeta(typeof(DummyTargetState));
            Type trigger = typeof(DummyTrigger);

            source.AddDirectTransition(trigger, target);

            Assert.IsTrue(source.DirectTransition.ContainsKey(trigger));
            Assert.AreEqual(target, source.DirectTransition[trigger]);
        }

        [Test]
        public void Given_StateMeta_When_AddCaptionCalled_Then_CaptionIsStored()
        {
            var state = new StateMeta(m_exampleType);
            string caption = "Entering this state initializes data.";

            state.AddCaption(caption);

            CollectionAssert.Contains(state.Captions, caption);
        }

        [Test]
        public void Given_StateMeta_When_ToStringCalled_Then_ReturnsTypeName()
        {
            var state = new StateMeta(typeof(DummyState));

            string result = state.ToString();

            Assert.AreEqual(nameof(DummyState), result);
        }
        
        class DummyState { }
        class DummyParentState { }
        class DummyChildState { }
        class DummyTargetState { }
        class DummyTrigger { }
    }
}