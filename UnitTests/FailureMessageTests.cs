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
        public void ExpectedExpression_IsEmpty_ReturnsNull()
        {
            testExpression.GetExpectedExpression().Returns(string.Empty);
            sut.ExpectedValue = "not empty";

            Assert.IsNull(sut.ExpectedExpression);
        }
    }
}