using NUnit.Framework;
using System.Text.RegularExpressions;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class RegexFailureMessageTests
    {
        [Test]
        public void ExpectedValue_RegexHasDefaultOptions_WrapsPatternInSlashes()
        {
            RegexFailureMessage sut = new RegexFailureMessage { ExpectedValue = new Regex("foo") };
            Assert.AreEqual("/foo/", sut.ExpectedValue);
        }

        [Test]
        public void ExpectedValue_RegexHasOptionsSpecified_AppendsOptions()
        {
            RegexFailureMessage sut = new RegexFailureMessage { ExpectedValue = new Regex("foo", RegexOptions.IgnoreCase | RegexOptions.Multiline) };
            Assert.AreEqual("/foo/ {IgnoreCase, Multiline}", sut.ExpectedValue);
        }
    }
}