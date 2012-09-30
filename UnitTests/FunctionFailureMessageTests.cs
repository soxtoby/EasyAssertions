using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class FunctionFailureMessageTests
    {
        [Test]
        public void SimpleObjectFunc()
        {
            object obj = new object();
            FunctionFailureMessage sut = FailureMessage(() => obj);

            Assert.AreEqual("obj", sut.ActualExpression);
        }

        [Test]
        public void LiteralValueFunc()
        {
            FunctionFailureMessage sut = FailureMessage(() => 1);

            Assert.AreEqual("1", sut.ActualExpression);
        }

        [Test]
        public void ValueFunc()
        {
            int val = 1;
            FunctionFailureMessage sut = FailureMessage(() => val);

            Assert.AreEqual("val", sut.ActualExpression);
        }

        [Test]
        public void ActualExpresson_IsEscaped()
        {
            object foo = new object();
            FunctionFailureMessage sut = FailureMessage(() => new[] { foo });

            Assert.AreEqual(@"new [] \{foo\}", sut.ActualExpression);
        }

        private static FunctionFailureMessage FailureMessage(Expression<Func<object>> func)
        {
            return new FunctionFailureMessage(func);
        }
    }
}