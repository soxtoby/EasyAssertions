using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class BraceMatcherTests
    {
        [Test]
        public void UnmatchedBraces_ReturnsNegativeOne()
        {
            int result = BraceMatcher.FindClosingBrace("(");

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void EmptyBraces()
        {
            int result = BraceMatcher.FindClosingBrace("()");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TwoPairs_MatchesFirstPairOnly()
        {
            int result = BraceMatcher.FindClosingBrace("()()");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TwoPairs_StartAfterFirst_MatchesSecondPair()
        {
            int result = BraceMatcher.FindClosingBrace("()()", 2);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void NestedBraces()
        {
            int result = BraceMatcher.FindClosingBrace("(())");

            Assert.AreEqual(3, result);
        }

        [Test]
        public void BracesInsideString_AreIgnored()
        {
            int result = BraceMatcher.FindClosingBrace(@"( "")"" )");

            Assert.AreEqual(6, result);
        }

        [Test]
        public void BracesInsideStringWithEscapedQuotes_AreIgnored()
        {
            int result = BraceMatcher.FindClosingBrace(@"( "" \"" ) "" )");

            Assert.AreEqual(11, result);
        }

        [Test]
        public void FindNextComma_ReturnsIndexOfComma()
        {
            int result = BraceMatcher.FindNext("method(param1, param2, param3)", ',', 7);

            Assert.AreEqual(13, result);
        }

        [Test]
        public void FindNextComma_NoComma_ReturnsNegativeOne()
        {
            int result = BraceMatcher.FindNext("foo", ',');

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void FindNextComma_IgnoresCommasInsideBraces()
        {
            int result = BraceMatcher.FindNext("new[] { 1, 2 }, param2, param3", ',');

            Assert.AreEqual(14, result);
        }

        [Test]
        public void FindNextComma_NoCommaOutsideBraces_ReturnsNegativeOne()
        {
            int result = BraceMatcher.FindNext("new[] { 1, 2 }", ',');

            Assert.AreEqual(-1, result);
        }
    }
}