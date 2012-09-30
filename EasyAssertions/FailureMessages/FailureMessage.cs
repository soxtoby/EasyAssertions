using System;
using System.Linq;
using SmartFormat;
using SmartFormat.Core;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building assertion failure messages in a consistent format.
    /// <see cref="ToString"/> formats the provided <see cref="MessageTemplate"/> with the properties of the <c>FailureMessage</c>.
    /// </summary>
    public class FailureMessage
    {
        /// <summary>
        /// Formats the provided <see cref="MessageTemplate"/> with the properties of the <see cref="FailureMessage"/>.
        /// Prepends the message with the <see cref="ActualExpression"/> on its own line.
        /// Appends the <see cref="UserMessage"/> string on a new line if one is supplied.
        /// </summary>
        public override string ToString()
        {
            SmartFormatter formatter = Smart.CreateDefaultSmartFormat();
            formatter.ErrorAction = ErrorAction.OutputErrorInResult;
            formatter.FormatterExtensions.Insert(0, ExpectedFormatter.Instance);
            formatter.Parser.UseAlternativeEscapeChar('\\');

            return formatter.Format(ActualExpression
                + "{BR}" + MessageTemplate
                    + "{UserMessage:{0.BR}{}|}", this);
        }

        /// <summary>
        /// A SmartFormat format string used to build the resulting failure message.
        /// The message should continue naturally from the source representation of the actual value.
        /// E.g. "should be empty."
        /// </summary>
        /// <remarks>
        /// See https://github.com/scottrippey/SmartFormat/wiki and http://www.codeproject.com/Articles/42310/Custom-Formatting-in-NET-Enhancing-String-Format-t
        /// for more information about the SmartFormat syntax.
        /// Note that { and } should be escaped as \{ and \}, rather than doubling the braces as in .NET composite format strings.
        /// </remarks>
        public virtual string MessageTemplate { get; set; }

        /// <summary>
        /// Provides access to the expected value without modification.
        /// </summary>
        public object RawExpectedValue { get; protected set; }

        /// <summary>
        /// The value that the actual value was compared against.
        /// Objects are wrapped in &lt; &gt; and strings are wrapped in " ".
        /// </summary>
        public virtual object ExpectedValue
        {
            get { return Output(RawExpectedValue); }
            set { RawExpectedValue = value; }
        }

        /// <summary>
        /// Provides access to the actual value without modification.
        /// </summary>
        public object RawActualValue;

        /// <summary>
        /// The value being asserted on.
        /// Objects are wrapped in &lt; &gt; and strings are wrapped in " ".
        /// </summary>
        public virtual object ActualValue
        {
            get { return Output(RawActualValue); }
            set { RawActualValue = value; }
        }

        /// <summary>
        /// A user-provided message that will be appended to the message output.
        /// </summary>
        public virtual string UserMessage { get; set; }

        /// <summary>
        /// When referenced in the <see cref="MessageTemplate"/>, will output the <see cref="ExpectedExpression"/>,
        /// with the <see cref="ExpectedValue"/> aligned underneath it. If <c>ExpectedExpression</c> returns null,
        /// only <c>ExpectedValue</c> will be output.
        /// </summary>
        /// <remarks>
        /// Expected can be referenced in the <see cref="MessageTemplate"/> as just "{Expected}".
        /// Alternatively, a format for the expected value can be specified after a colon like so:
        ///     "{Expected:format_string}"
        /// </remarks>
        public virtual Expected Expected { get { return new FailureMessageExpected(this); } }

        /// <summary>
        /// The source representation of the actual value.
        /// </summary>
        public virtual string ActualExpression
        {
            get
            {
                return TestExpression.GetActual().NullIfEmpty()
                    ?? "Actual value";
            }
        }

        /// <summary>
        /// The source representation of the expected value.
        /// Returns null if the source representation is the same as the value output
        /// (e.g. if the source was a literal value)
        /// </summary>
        public virtual string ExpectedExpression
        {
            get
            {
                string expectedExpression = TestExpression.GetExpected() ?? string.Empty;

                return MatchesExpectedValueOutput(expectedExpression)
                    || IsStringLiteral(expectedExpression)
                    || IsNumericLiteral(expectedExpression)
                    || IsBooleanLiteral(expectedExpression)
                    || IsCharLiteral(expectedExpression)
                    || IsNullLiteral(expectedExpression)
                        ? null
                        : expectedExpression.NullIfEmpty();
            }
        }

        private bool MatchesExpectedValueOutput(string expectedExpression)
        {
            string expectedValueOutput = string.Empty + RawExpectedValue;
            return expectedExpression == expectedValueOutput;
        }

        private static bool IsStringLiteral(string expectedExpression)
        {
            return expectedExpression.TrimStart('@').FirstOrDefault() == '"';
        }

        private static bool IsNumericLiteral(string expectedExpression)
        {
            char firstChar = expectedExpression.FirstOrDefault();
            return firstChar >= 48
                && firstChar <= 57;
        }

        private bool IsBooleanLiteral(string expectedExpression)
        {
            return RawExpectedValue is bool
                && expectedExpression == RawExpectedValue.ToString().ToLower();
        }

        private static bool IsCharLiteral(string expectedExpression)
        {
            return expectedExpression.FirstOrDefault() == '\'';
        }

        private static bool IsNullLiteral(string expectedExpression)
        {
            return expectedExpression == "null";
        }

        /// <summary>
        /// For providing positioning information about the assertion failure.
        /// </summary>
        public int FailureIndex { get; set; }

        /// <summary>
        /// A handy alias to <see cref="Environment.NewLine"/> that can be used inside the <see cref="MessageTemplate"/>.
        /// </summary>
        public readonly string BR = Environment.NewLine;

        /// <summary>
        /// Wraps objects in &lt; &gt; and strings in " ".
        /// </summary>
        protected static string Output(object value)
        {
            return value is string
                ? "\"" + value + "\""
                : "<" + (value ?? "null") + ">";
        }
    }
}