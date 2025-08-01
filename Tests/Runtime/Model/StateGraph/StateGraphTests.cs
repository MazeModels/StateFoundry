using System;
using System.Linq;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StateGraphTests
    {
        [Test]
        public void Given_ConcreteRootState_And_GraphIsBuilt_Then_InitialStateIsInGraph()
        {
            var graph = new StateGraph(typeof(RootState));

            Assert.That(graph.States.ContainsKey(typeof(RootState)), Is.True);
        }

        [Test]
        public void Given_StateWithBaseState_And_GraphIsBuilt_Then_AncestorsAreLinked()
        {
            var graph = new StateGraph(typeof(LeafState));

            var leaf = graph.States[typeof(LeafState)];
            var mid = graph.States[typeof(MidState)];
            var root = graph.States[typeof(RootState)];

            Assert.That(leaf.Parent, Is.EqualTo(mid));
            Assert.That(mid.Parent, Is.EqualTo(root));

            Assert.That(mid.Children.Contains(leaf), Is.True);
            Assert.That(root.Children.Contains(mid), Is.True);
        }

        [Test]
        public void Given_StateWithDirectTransition_And_GraphIsBuilt_Then_DirectTransitionIsMapped()
        {
            var graph = new StateGraph(typeof(MidState));

            var mid = graph.States[typeof(MidState)];

            Assert.That(mid.DirectTransition.ContainsKey(typeof(Trigger)), Is.True);

            var destination = mid.DirectTransition[typeof(Trigger)];
            Assert.That(destination.Type, Is.EqualTo(typeof(LeafState)));
        }

        [Test]
        public void Given_StateWithDirectTransition_And_GraphIsBuilt_Then_TransitionIsPropagatedToChildren()
        {
            var graph = new StateGraph(typeof(LeafState));

            var leaf = graph.States[typeof(LeafState)];

            Assert.That(leaf.Transitions.ContainsKey(typeof(Trigger)), Is.True);
            Assert.That(leaf.Transitions[typeof(Trigger)].Type, Is.EqualTo(typeof(LeafState)));
        }

        [Test]
        public void Given_MultipleBuilds_And_AlreadyVisitedState_Then_BuildMetaGraphIsNotReentered()
        {
            var graph = new StateGraph(typeof(LeafState));

            Assert.That(graph.States.Keys, Does.Contain(typeof(MidState)));
            Assert.That(graph.States.Keys, Does.Contain(typeof(LeafState)));
        }

        [Test]
        public void Given_NonDerivedType_And_ConstructingGraph_Then_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StateGraph(typeof(string)));
        }

        [Test]
        public void Given_AbstractState_And_ConstructingGraph_Then_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StateGraph(typeof(AbstractState)));
        }


        class RootState : State { }

        class MidState : RootState, IGet<Trigger, LeafState> { }

        class LeafState : MidState { }

        struct Trigger :ITrigger { }

        abstract class AbstractState : State { }
    }
}
