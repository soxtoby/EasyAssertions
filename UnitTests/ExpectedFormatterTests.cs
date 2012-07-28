using System;
using NSubstitute;
using NUnit.Framework;
using SmartFormat;
using SmartFormat.Core;

namespace EasyAssertions.UnitTests
{
    [TestFixture]
    public class ExpectedFormatterTests
    {
        private SmartFormatter formatter;
        private Expected expected;

        [SetUp]
        public void SetUp()
        {
            formatter = Smart.CreateDefaultSmartFormat();
            formatter.ErrorAction = ErrorAction.ThrowError;
            formatter.FormatterExtensions.Insert(0, ExpectedFormatter.Instance);
            expected = Substitute.For<Expected>();
            expected.Value.Returns("value");
        }

        [Test]
        public void ExpectedExpressionIsNull_JustValue()
        {
            string result = formatter.Format("{0}", expected);

            Assert.AreEqual("value", result);
        }

        [Test]
        public void ExpectedExpressionAvailable_ValueOnNewLine()
        {
            expected.Expression.Returns("foo");

            string result = formatter.Format("{0}", expected);

            Assert.AreEqual("foo" + Environment.NewLine + "value", result);
        }

        [Test]
        public void ExpectedExpressionAvailable_ValueIndented()
        {
            expected.Expression.Returns("foo");

            string result = formatter.Format(" {0}", expected);

            Assert.AreEqual(" foo" + Environment.NewLine + " value", result);
        }

        [Test]
        public void SecondLine_ExpectedExpressionAvailable_ValueIndented()
        {
            expected.Expression.Returns("bar");

            string result = formatter.Format("foo" + Environment.NewLine + " {0}", expected);

            Assert.AreEqual("foo" + Environment.NewLine + " bar" + Environment.NewLine + " value", result);
        }

        [Test]
        public void FormatExpressionSpecified_UsedToFormatValue()
        {
            expected.Value.Returns(new { StringValue = "foo" });

            string result = formatter.Format("{0:{Value.StringValue}}", expected);

            Assert.AreEqual("foo", result);
        }
    }
}