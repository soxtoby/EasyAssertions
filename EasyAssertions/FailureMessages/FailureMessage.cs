using System;
using SmartFormat;
using SmartFormat.Core;

namespace EasyAssertions
{
    public class FailureMessage
    {
        public override string ToString()
        {
            SmartFormatter formatter = Smart.CreateDefaultSmartFormat();
            formatter.ErrorAction = ErrorAction.ThrowError;
            formatter.FormatterExtensions.Insert(0, ExpectedFormatter.Instance);
            formatter.Parser.UseAlternativeEscapeChar('\\');

            return formatter.Format(ActualExpression
                + "{BR}" + MessageTemplate
                    + "{UserMessage:{0.BR}{}|}", this);
        }

        protected const string Ellipses = "...";

        public virtual string MessageTemplate { get; set; }

        public object RawExpectedValue;

        public virtual object ExpectedValue
        {
            get { return Output(RawExpectedValue); }
            set { RawExpectedValue = value; }
        }

        public object RawActualValue;

        public virtual object ActualValue
        {
            get { return Output(RawActualValue); }
            set { RawActualValue = value; }
        }

        public virtual string UserMessage { get; set; }

        public virtual ExpectedWrapper Expected { get { return new ExpectedWrapper(this); } }

        public virtual string ActualExpression { get { return TestExpression.GetActual(); } }

        public virtual string ExpectedExpression
        {
            get
            {
                string expectedExpression = TestExpression.GetExpected();
                string expectedValueOutput = String.Empty + RawExpectedValue;

                return expectedExpression == expectedValueOutput
                    ? null
                    : expectedExpression;
            }
        }

        public int FailureIndex { get; set; }

        public readonly string BR = Environment.NewLine;

        public string Output(object value)
        {
            return value is string
                ? "\"" + value + "\""
                : "<" + value + ">";
        }

        public class ExpectedWrapper
        {
            private readonly FailureMessage failureMessage;

            public ExpectedWrapper(FailureMessage failureMessage)
            {
                this.failureMessage = failureMessage;
            }

            public string Expression { get { return failureMessage.ExpectedExpression; } }
            public object Value { get { return failureMessage.ExpectedValue; } }
        }
    }
}