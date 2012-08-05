using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class FailureMessageTests
    {
        private TestExpressionProvider testExpression;
        private FailureMessage sut;

        [SetUp]
        public void SetUp()
        {
            testExpression = Substitute.For<TestExpressionProvider>();
            TestExpression.OverrideProvider(testExpression);
            sut = new FailureMessage();
        }

        [TearDown]
        public void TearDown()
        {
            TestExpression.DefaultProvider();
        }

        [Test]
        public void ActualExpression_IsEmpty_ReturnsDefault()
        {
            testExpression.GetActualExpression().Returns(string.Empty);

            Assert.AreEqual("Actual value", sut.ActualExpression);
        }

        [Test]
        public void ExpectedExpression_NonEmpty_ReturnsValue()
        {
            testExpression.GetExpectedExpression().Returns("foo");

            Assert.AreEqual("foo", sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_IsEmpty_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns(string.Empty);
            sut.ExpectedValue = "not empty";

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_IntegerLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("5");
            sut.ExpectedValue = 5;

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_FloatLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("1f");
            sut.ExpectedValue = 1f;

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_StringLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("\"foo\"");
            sut.ExpectedValue = "foo";

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_VerbatimStringLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("@\"foo\"");
            sut.ExpectedValue = "foo";

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_CharLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("'a'");
            sut.ExpectedValue = 'a';

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpression_BooleanLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("true");
            sut.ExpectedValue = true;

            Assert.IsNull(sut.ExpectedExpression);
        }

        [Test]
        public void ExpectedExpresson_NullLiteral_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns("null");

            Assert.IsNull(sut.ExpectedExpression);
        }
    }
}