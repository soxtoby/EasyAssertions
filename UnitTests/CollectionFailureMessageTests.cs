using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class CollectionFailureMessageTests
    {
        private static readonly List<object> SingleItem = new List<object> { new FakeObject("foo") };
        private static readonly List<object> MultipleItems = new List<object> { 1, 2 };
        private static readonly List<object> ManyItems = new List<object> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        [Test]
        public void ItemType_IsEscaped()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ItemType = "{foo}" };

            Assert.AreEqual(@"\{foo\}", sut.ItemType);
        }

        [Test]
        public void ActualSample_Empty()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ActualItems = new List<object>() };

            SampleIsEmpty(sut.ActualSample);
        }

        [Test]
        public void ExpectedSample_Empty()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ExpectedItems = new List<object>() };

            SampleIsEmpty(sut.ExpectedSample);
        }

        private static void SampleIsEmpty(string sample)
        {
            Assert.AreEqual("empty.", sample);
        }

        [Test]
        public void ActualSample_SingleItem()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ActualItems = SingleItem };

            SampleHasSingleItem(sut.ActualSample);
        }

        [Test]
        public void ExpectedSample_SingleItem()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ExpectedItems = SingleItem };

            SampleHasSingleItem(sut.ExpectedSample);
        }

        private static void SampleHasSingleItem(string sample)
        {
            Assert.AreEqual("[<foo>]", sample);
        }

        [Test]
        public void ActualSample_MultipleItems()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ActualItems = MultipleItems };

            SampleHasMultipleItems(sut.ActualSample);
        }

        [Test]
        public void ExpectedSample_MultipleItems()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ExpectedItems = MultipleItems };

            SampleHasMultipleItems(sut.ExpectedSample);
        }

        private static void SampleHasMultipleItems(string sample)
        {
            Assert.AreEqual("[" + Environment.NewLine
                            + "    <1>," + Environment.NewLine
                            + "    <2>" + Environment.NewLine
                            + "]", sample);
        }

        [Test]
        public void ActualSample_ManyItems()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ActualItems = ManyItems };

            SampleHasFirstFewItems(sut.ActualSample);
        }

        [Test]
        public void ExpectedSample_ManyItems()
        {
            CollectionFailureMessage sut = new CollectionFailureMessage { ExpectedItems = ManyItems };

            SampleHasFirstFewItems(sut.ExpectedSample);
        }

        private static void SampleHasFirstFewItems(string sample)
        {
            Assert.AreEqual("[" + Environment.NewLine
                            + "    <1>," + Environment.NewLine
                            + "    <2>," + Environment.NewLine
                            + "    <3>," + Environment.NewLine
                            + "    <4>," + Environment.NewLine
                            + "    <5>," + Environment.NewLine
                            + "    <6>," + Environment.NewLine
                            + "    <7>," + Environment.NewLine
                            + "    <8>," + Environment.NewLine
                            + "    <9>," + Environment.NewLine
                            + "    <10>," + Environment.NewLine
                            + "    ..." + Environment.NewLine
                            + "]", sample);
        }
    }
}