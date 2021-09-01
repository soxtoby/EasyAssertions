using System;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class MessageHelperTests
    {
        [Test]
        public void Sample_LimitsTo10Items()
        {
            var result = MessageHelper.Sample(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });

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
            var result = MessageHelper.Sample(new object[] { new object[] { new object[] { 1 } } });

            Assert.AreEqual("[ [ [ <1> ] ] ]", result);
        }

        [Test]
        public void Sample_NestedEnumerables_OutputsInnerItems()
        {
            var result = MessageHelper.Sample(new object[]
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
            var result = MessageHelper.Sample(new[] { "foo", "bar" });

            Assert.AreEqual(@"[
    ""foo"",
    ""bar""
]", result);
        }

        [Test]
        public void Sample_NestedEnumerable_LimitsTo10Items()
        {
            var result = MessageHelper.Sample(new object[]
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
            var result = MessageHelper.Sample(new object[]
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
            var result = MessageHelper.Sample(new object[]
                {
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>(),
                    Array.Empty<object>()
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
            var result = MessageHelper.Sample(new object[]
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