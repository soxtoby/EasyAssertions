using NSubstitute;
using NUnit.Framework;
using System;

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

        protected void AssertArgumentNullException(string paramName, TestDelegate assertionCall)
        {
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(assertionCall);

            Assert.AreEqual(paramName, result.ParamName);
        }
    }
}