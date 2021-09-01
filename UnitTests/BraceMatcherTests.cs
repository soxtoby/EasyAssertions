using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class BraceMatcherTests
    {
        [Test]
        public void UnmatchedBraces_ReturnsNegativeOne()
        {
            var result = BraceMatcher.FindClosingBrace("(");

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void EmptyBraces()
        {
            var result = BraceMatcher.FindClosingBrace("()");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TwoPairs_MatchesFirstPairOnly()
        {
            var result = BraceMatcher.FindClosingBrace("()()");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TwoPairs_StartAfterFirst_MatchesSecondPair()
        {
            var result = BraceMatcher.FindClosingBrace("()()", 2);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void NestedBraces()
        {
            var result = BraceMatcher.FindClosingBrace("(())");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void BracesInsideString_AreIgnored()
        {
            var result = BraceMatcher.FindClosingBrace(@"( "")"" )");

            Assert.AreEqual(6, result);
        }

        [Test]
        public void BracesInsideStringWithEscapedQuotes_AreIgnored()
        {
            var result = BraceMatcher.FindClosingBrace(@"( "" \"" ) "" )");

            Assert.AreEqual(11, result);
        }

        [Test]
        public void FindNextComma_ReturnsIndexOfComma()
        {
            var result = BraceMatcher.FindNext("method(param1, param2, param3)", ',', 7);

            Assert.AreEqual(13, result);
        }

        [Test]
        public void FindNextComma_NoComma_ReturnsNegativeOne()
        {
            var result = BraceMatcher.FindNext("foo", ',');

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void FindNextComma_IgnoresCommasInsideBraces()
        {
            var result = BraceMatcher.FindNext("new[] { 1, 2 }, param2, param3", ',');

            Assert.AreEqual(14, result);
        }

        [Test]
        public void FindNextComma_NoCommaOutsideBraces_ReturnsNegativeOne()
        {
            var result = BraceMatcher.FindNext("new[] { 1, 2 }", ',');

            Assert.AreEqual(-1, result);
        }
    }
}