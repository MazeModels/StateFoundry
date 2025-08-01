using System;
using NUnit.Framework;

namespace Maze.StateFoundry.Tests
{
    public class CaptionAttributeTests
    {
        [Test]
        public void Given_NullCaption_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CaptionAttribute(null!));
        }

        [Test]
        public void Given_EmptyCaption_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CaptionAttribute(""));
        }

        [Test]
        public void Given_ValidCaption_Then_CaptionPropertyIsSetCorrectly()
        {
            var sut = new CaptionAttribute("abc");
            Assert.That(sut.Caption, Is.EqualTo("abc"));
        }

        [Test]
        public void Given_ValidWhenEnum_And_ValidWhatString_Then_CaptionIsFormattedCorrectly()
        {
            var when = When.OnEnter;
            var what = "Start";

            var sut = new CaptionAttribute(when, what);
            Assert.That(sut.Caption, Is.EqualTo($"{when} → {what}"));
        }

        [Test]
        public void Given_EnumConstructor_And_NullWhat_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CaptionAttribute(When.OnEnter, null!));
        }

        [Test]
        public void Given_EnumConstructor_And_EmptyWhat_Then_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CaptionAttribute(When.OnEnter, ""));
        }

        [Test]
        public void Given_CaptionAttributeType_Then_ValidOnIsClassOnly()
        {
            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(typeof(CaptionAttribute), typeof(AttributeUsageAttribute));

            Assert.That(usage.ValidOn, Is.EqualTo(AttributeTargets.Class));
        }

        [Test]
        public void Given_CaptionAttributeType_Then_AllowMultipleIsTrue()
        {
            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(typeof(CaptionAttribute), typeof(AttributeUsageAttribute));

            Assert.That(usage.AllowMultiple, Is.True);
        }

        [Test]
        public void Given_CaptionAttributeType_Then_InheritedIsFalse()
        {
            var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(typeof(CaptionAttribute), typeof(AttributeUsageAttribute));

            Assert.That(usage.Inherited, Is.False);
        }

        [Test]
        public void Given_ClassWithMultipleCaptionAttributes_Then_RetrievesAllAttributes()
        {
            var attributes = typeof(DummyWithTwoCaptions).GetCustomAttributes(typeof(CaptionAttribute), false);

            Assert.That(attributes.Length, Is.EqualTo(2));
        }

        [Test]
        public void Given_BaseClassWithCaption_And_DerivedClass_Then_CaptionNotInherited()
        {
            var attributes = typeof(DerivedWithoutCaption).GetCustomAttributes(typeof(CaptionAttribute), true);

            Assert.That(attributes.Length, Is.EqualTo(0));
        }

        [Caption("one")]
        [Caption("two")]
        class DummyWithTwoCaptions { }

        [Caption("base")]
        class BaseWithCaption { }

        class DerivedWithoutCaption : BaseWithCaption { }
    }
}
