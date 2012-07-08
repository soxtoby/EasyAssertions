using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class BraceMatcherTests
    {
        [Test]
        public void UnmatchesBraces_ReturnsNegativeOne()
        {
            BraceMatcher sut = MakeBraceMatcher("(");

            int result = sut.MatchFrom(0);

            Assert.AreEqual(-1, result);
        }

        [Test]
        public void EmptyBraces()
        {
            BraceMatcher sut = MakeBraceMatcher("()");

            int result = sut.MatchFrom(0);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TwoPairs_MatchesFirstPairOnly()
        {
            BraceMatcher sut = MakeBraceMatcher("()()");

            int result = sut.MatchFrom(0);

            Assert.AreEqual(1, result);
        }

        [Test]
        public void TwoPairs_StartAfterFirst_MatchesSecondPair()
        {
            BraceMatcher sut = MakeBraceMatcher("()()");

            int result = sut.MatchFrom(2);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void NestedBraces()
        {
            BraceMatcher sut = MakeBraceMatcher("(())");

            int result = sut.MatchFrom(0);

            Assert.AreEqual(3, result);
        }

        [Test]
        public void BracesInsideString_AreIgnored()
        {
            BraceMatcher sut = MakeBraceMatcher(@"( "")"" )");

            int result = sut.MatchFrom(0);

            Assert.AreEqual(6, result);
        }

        [Test]
        public void BracesInsideStringWithEscapedQuotes_AreIgnored()
        {
            BraceMatcher sut = MakeBraceMatcher(@"( "" \"" ) "" )");

            int result = sut.MatchFrom(0);

            Assert.AreEqual(11, result);
        }

        [Test]
        public void FindNextComma()
        {
            BraceMatcher sut = new BraceMatcher("method(param1, param2, param3)", open: '\0', close: ',');

            int result = sut.MatchFrom(0, initialDepth: 1);

            Assert.AreEqual(13, result);
        }

        private static BraceMatcher MakeBraceMatcher(string source)
        {
            return new BraceMatcher(source);
        }
    }
}