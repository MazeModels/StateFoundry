using NUnit.Framework;
using NSubstitute;
using System;
using System.Linq;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatechartNavigatorTests
    {
        StatechartNavigator m_navigator;

        [SetUp]
        public void SetUp()
        {
            m_navigator = new StatechartNavigator();
        }

        [Test]
        public void Given_TwoStatesWithCommonAncestor_And_FindCommonAncestor_Then_ReturnsCorrectAncestor()
        {
            var root = CreateState();
            var child1 = CreateState(root);
            var child2 = CreateState(root);

            var ancestor = m_navigator.FindCommonAncestor(child1, child2);

            Assert.AreEqual(root, ancestor);
        }

        [Test]
        public void Given_OneStateIsAncestorOfOther_And_FindCommonAncestor_Then_ReturnsAncestor()
        {
            var root = CreateState();
            var child = CreateState(root);

            var ancestor = m_navigator.FindCommonAncestor(root, child);

            Assert.AreEqual(root, ancestor);
        }

        [Test]
        public void Given_TwoStatesWithNoCommonAncestor_And_FindCommonAncestor_Then_ReturnsNull()
        {
            var root1 = CreateState();
            var root2 = CreateState();
            var child1 = CreateState(root1);
            var child2 = CreateState(root2);

            var ancestor = m_navigator.FindCommonAncestor(child1, child2);

            Assert.IsNull(ancestor);
        }

        [Test]
        public void Given_StateAndAncestor_And_FindPathToAncestor_Then_ReturnsCorrectPath()
        {
            var root = CreateState();
            var mid = CreateState(root);
            var leaf = CreateState(mid);

            var path = m_navigator.FindPathToAncestor(leaf, root).ToList();

            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(leaf, path[0]);
            Assert.AreEqual(mid, path[1]);
            Assert.IsFalse(path.Contains(root));
        }

        [Test]
        public void Given_StateAndAncestor_And_FindPathFromAncestor_Then_ReturnsCorrectPathInReverse()
        {
            var root = CreateState();
            var mid = CreateState(root);
            var leaf = CreateState(mid);

            var path = m_navigator.FindPathFromAncestor(leaf, root).ToList();

            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(mid, path[0]);
            Assert.AreEqual(leaf, path[1]);
        }

        [Test]
        public void Given_State_And_FindPathFromRoot_Then_ReturnsLineageReversed()
        {
            var root = CreateState();
            var mid = CreateState(root);
            var leaf = CreateState(mid);

            var path = m_navigator.FindPathFromRoot(leaf).ToList();

            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(root, path[0]);
            Assert.AreEqual(mid, path[1]);
            Assert.AreEqual(leaf, path[2]);
        }

        [Test]
        public void Given_SingleNode_And_FindPathToAncestorWithSelf_Then_ReturnsEmpty()
        {
            var node = CreateState();

            var path = m_navigator.FindPathToAncestor(node, node).ToList();

            Assert.IsEmpty(path);
        }


        [Test]
        public void Given_NullFirstState_And_FindCommonAncestor_Then_ThrowsArgumentNullException()
        {
            var state = CreateState();

            Assert.Throws<ArgumentNullException>(() => m_navigator.FindCommonAncestor(null, state));
        }

        [Test]
        public void Given_NullSecondState_And_FindCommonAncestor_Then_ThrowsArgumentNullException()
        {
            var state = CreateState();

            Assert.Throws<ArgumentNullException>(() => m_navigator.FindCommonAncestor(state, null));
        }

        [Test]
        public void Given_NullData_And_FindPathToAncestor_Then_ThrowsArgumentNullException()
        {
            var ancestor = CreateState();

            Assert.Throws<ArgumentNullException>(() => m_navigator.FindPathToAncestor(null, ancestor));
        }

        [Test]
        public void Given_NullAncestor_And_FindPathToAncestor_Then_ThrowsArgumentNullException()
        {
            var data = CreateState();

            Assert.Throws<ArgumentNullException>(() => m_navigator.FindPathToAncestor(data, null));
        }

        [Test]
        public void Given_NullData_And_FindPathFromAncestor_Then_ThrowsArgumentNullException()
        {
            var ancestor = CreateState();

            Assert.Throws<ArgumentNullException>(() => m_navigator.FindPathFromAncestor(null, ancestor));
        }

        [Test]
        public void Given_NullAncestor_And_FindPathFromAncestor_Then_ThrowsArgumentNullException()
        {
            var data = CreateState();

            Assert.Throws<ArgumentNullException>(() => m_navigator.FindPathFromAncestor(data, null));
        }

        [Test]
        public void Given_NullData_And_FindPathFromRoot_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => m_navigator.FindPathFromRoot(null));
        }

        [Test]
        public void Given_StateAndAncestor_And_FindPathToAncestor_Then_AncestorIsExcludedFromResult()
        {
            var root = CreateState();
            var mid = CreateState(root);
            var leaf = CreateState(mid);

            var path = m_navigator.FindPathToAncestor(leaf, root).ToList();

            Assert.IsFalse(path.Contains(root), "Ancestor should be excluded from result");
        }

        [Test]
        public void Given_StateAndAncestor_And_FindPathFromAncestor_Then_AncestorIsExcludedFromResult()
        {
            var root = CreateState();
            var mid = CreateState(root);
            var leaf = CreateState(mid);

            var path = m_navigator.FindPathFromAncestor(leaf, root).ToList();

            Assert.IsFalse(path.Contains(root), "Ancestor should be excluded from result");
        }


        [Test]
        public void Given_State_And_FindPathToRoot_Then_ReturnsPathFromStateToRoot()
        {
            var root = CreateState();
            var mid = CreateState(root);
            var leaf = CreateState(mid);

            var path = m_navigator.FindPathToRoot(leaf).ToList();

            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(leaf, path[0]);
            Assert.AreEqual(mid, path[1]);
            Assert.AreEqual(root, path[2]);
        }

        [Test]
        public void Given_SingleState_And_FindPathToRoot_Then_ReturnsOnlyThatState()
        {
            var node = CreateState();

            var path = m_navigator.FindPathToRoot(node).ToList();

            Assert.AreEqual(1, path.Count);
            Assert.AreEqual(node, path[0]);
        }

        [Test]
        public void Given_NullData_And_FindPathToRoot_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => m_navigator.FindPathToRoot(null).ToList());
        }



        IStateData CreateState(IStateData parent = null)
        {
            var state = Substitute.For<IStateData>();
            state.Parent.Returns(parent);
            return state;
        }
    }
}
