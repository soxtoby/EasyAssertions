using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public abstract class AssertionTests
    {
        protected IFailureMessageFormatter MockFormatter;

        [SetUp]
        public void SetUp()
        {
            MockFormatter = Substitute.For<IFailureMessageFormatter>();
            FailureMessageFormatter.Override(MockFormatter);
        }

        [TearDown]
        public void TearDown()
        {
            FailureMessageFormatter.Default();
        }
    }
}