using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class MessageHelperTests
    {
        [Test]
        public void Sample_LimitsTo10Items()
        {
            string result = MessageHelper.Sample(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
            
            Assert.AreEqual(@"[
    <1>,
    <2>,
    <3>,
    <4>,
    <5>,
    <6>,
    <7>,
    <8>,
    <9>,
    <10>,
    ...
]", result);
        }

        [Test]
        public void Sample_NestedEnumerableWithSingleItem_OutputsOnOneLine()
        {
            string result = MessageHelper.Sample(new object[] { new object[] { new object[] { 1 } } });

            Assert.AreEqual("[ [ [ <1> ] ] ]", result);
        }

        [Test]
        public void Sample_NestedEnumerables_OutputsInnerItems()
        {
            string result = MessageHelper.Sample(new object[]
                {
                    new object[] { 1, 2, 3, 4 },
                    new object[] { 5, 6, 7, 8 }
                });

            Assert.AreEqual(@"[
    [
        <1>,
        <2>,
        <3>,
        <4>
    ],
    [
        <5>,
        <6>,
        <7>,
        <8>
    ]
]", result);
        }

        [Test]
        public void Sample_Strings_OutputsWholeStrings()
        {
            string result = MessageHelper.Sample(new[] { "foo", "bar" });

            Assert.AreEqual(@"[
    ""foo"",
    ""bar""
]", result);
        }

        [Test]
        public void Sample_NestedEnumerable_LimitsTo10Items()
        {
            string result = MessageHelper.Sample(new object[]
                {
                    new object[] { 1, 2, 3, 4, 5, 6 },
                    new object[] { 7, 8, 9, 10, 11 }
                });

            Assert.AreEqual(@"[
    [
        <1>,
        <2>,
        <3>,
        <4>,
        <5>,
        <6>
    ],
    [
        <7>,
        <8>,
        <9>,
        <10>,
        ...
    ]
]", result);
        }

        [Test]
        public void Sample_NestedEnumerables_MoreEnumerablesAfterLimit()
        {
            string result = MessageHelper.Sample(new object[]
                {
                    new object[] { 1, 2, 3, 4, 5, 6 },
                    new object[] { 7, 8, 9, 10, 11 },
                    new object[] { }
                });

            Assert.AreEqual(@"[
    [
        <1>,
        <2>,
        <3>,
        <4>,
        <5>,
        <6>
    ],
    [
        <7>,
        <8>,
        <9>,
        <10>,
        ...
    ],
    ...
]", result);
        }

        [Test]
        public void Sample_EmptyEnumerables_EmptyEnumerableCountsAsItem()
        {
            string result = MessageHelper.Sample(new object[]
                {
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0],
                    new object[0]
                });

            Assert.AreEqual(@"[
    [],
    [],
    [],
    [],
    [],
    [],
    [],
    [],
    [],
    [],
    ...
]", result);
        }

        [Test]
        public void Sample_NestedSingleItemEnumerables()
        {
            string result = MessageHelper.Sample(new object[]
                {
                    new object[] { 1 },
                    new object[] { 2 },
                    new object[] { 3 },
                    new object[] { 4 },
                    new object[] { 5 },
                    new object[] { 6 },
                    new object[] { 7 },
                    new object[] { 8 },
                    new object[] { 9 },
                    new object[] { 10 },
                    new object[] { 11 }
                });

            Assert.AreEqual(@"[
    [ <1> ],
    [ <2> ],
    [ <3> ],
    [ <4> ],
    [ <5> ],
    [ <6> ],
    [ <7> ],
    [ <8> ],
    [ <9> ],
    [ <10> ],
    ...
]", result);
        }
    }
}