using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class StringFailureMessageTests
    {
        private StringFailureMessage sut;

        [SetUp]
        public void SetUp()
        {
            sut = new StringFailureMessage();
        }

        [Test]
        public void ActualValue_IsEscaped()
        {
            sut.ActualValue = "{foo}";

            Assert.AreEqual(@"""\{foo\}""", sut.ActualValue);
        }

        [Test]
        public void ExpectedValue_IsEscaped()
        {
            sut.ExpectedValue = "{foo}";

            Assert.AreEqual(@"""\{foo\}""", sut.ExpectedValue);
        }
    }
}