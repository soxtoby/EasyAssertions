using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class ContinuationTests
    {
        [Test]
        public void GetActualSegment()
        {
            Continuation sut = new Continuation(new SourceAddress(), "property");

            ExpressionSegment result = sut.GetActualSegment("foo.property.bar", 3);

            Assert.AreEqual(string.Empty, result.Expression);
            Assert.AreEqual(12, result.IndexOfNextSegment);
        }

        [Test]
        public void GetExpectedSegment()
        {
            Continuation sut = new Continuation(new SourceAddress(), "property");

            ExpressionSegment result = sut.GetExpectedSegment("foo.property.bar", 3);

            Assert.AreEqual(string.Empty, result.Expression);
            Assert.AreEqual(12, result.IndexOfNextSegment);
        }
    }
}