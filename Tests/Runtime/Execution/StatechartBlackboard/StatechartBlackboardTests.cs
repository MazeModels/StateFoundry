using NUnit.Framework;
using System;

namespace Maze.StateFoundry.Tests
{
    [TestFixture]
    public class StatechartBlackboardTests
    {
        StatechartBlackboard m_blackboard;

        [SetUp]
        public void SetUp()
        {
            m_blackboard = new StatechartBlackboard();
        }

        [Test]
        public void Given_EmptyBlackboard_And_AddComponent_Then_ComponentIsStored()
        {
            var expected = "test";

            m_blackboard.Add(expected);
            var actual = m_blackboard.Get<string>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_BlackboardWithComponent_And_GetComponent_Then_ReturnsStoredComponent()
        {
            var expected = 42;
            m_blackboard.Add(expected);

            var actual = m_blackboard.Get<int>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Given_BlackboardWithoutComponent_And_GetComponent_Then_ReturnsDefault()
        {
            var actual = m_blackboard.Get<DateTime>();

            Assert.AreEqual(default(DateTime), actual);
        }

        [Test]
        public void Given_BlackboardWithMultipleComponents_And_GetSpecificComponent_Then_ReturnsCorrectComponent()
        {
            m_blackboard.Add(123);
            m_blackboard.Add("abc");

            var intVal = m_blackboard.Get<int>();
            var strVal = m_blackboard.Get<string>();

            Assert.AreEqual(123, intVal);
            Assert.AreEqual("abc", strVal);
        }

        [Test]
        public void Given_BlackboardWithComponent_And_AddSameTypeComponent_Then_OverwritesComponent()
        {
            m_blackboard.Add(123);
            m_blackboard.Add(456);

            var actual = m_blackboard.Get<int>();

            Assert.AreEqual(456, actual);
        }

        [Test]
        public void Given_BlackboardWithReferenceType_And_GetComponent_Then_ReturnsSameInstance()
        {
            var instance = new TestClass { Value = 100 };
            m_blackboard.Add(instance);

            var actual = m_blackboard.Get<TestClass>();

            Assert.AreSame(instance, actual);
        }

        [Test]
        public void Given_BlackboardWithReferenceType_And_ModifyRetrievedInstance_Then_ModificationIsReflected()
        {
            var instance = new TestClass { Value = 100 };
            m_blackboard.Add(instance);

            var retrieved = m_blackboard.Get<TestClass>();
            retrieved.Value = 200;

            Assert.AreEqual(200, instance.Value);
        }

        class TestClass
        {
            public int Value { get; set; }
        }
    }
}