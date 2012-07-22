using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EasyAssertions
{
    public class StringFailureMessage : FailureMessage
    {
        private const int MaxStringWidth = 60;
        private const int IdealArrowIndex = 20;

        public override object ExpectedValue
        {
            get { return GetSnippet((string)RawExpectedValue); }
            set { RawExpectedValue = value; }
        }

        public override object ActualValue
        {
            get { return GetSnippet((string)RawActualValue); }
            set { RawActualValue = value; }
        }

        public override string ExpectedExpression
        {
            get
            {
                string expectedExpression = TestExpression.GetExpected();
                return expectedExpression.Trim('@', '"') == (string)RawExpectedValue
                    ? null
                    : expectedExpression;
            }
        }

        public virtual string Arrow
        {
            get
            {
                int arrowIndex = FailureIndex - SnippetStart((string)RawActualValue);
                arrowIndex += ((string)RawActualValue)
                    .Substring(0, arrowIndex)
                    .Count(Escapes.ContainsKey);

                return new string(' ', arrowIndex + 1) + '^';   // + 1 for the quotes wrapped around string values
            }
        }

        private string GetSnippet(string wholeString)
        {
            int fromIndex = SnippetStart(wholeString);
            int snippetLength = MaxStringWidth;
            string prefix = string.Empty;
            string suffix = string.Empty;

            if (fromIndex > 0)
            {
                prefix = Ellipses;
                snippetLength -= prefix.Length;
                fromIndex += prefix.Length;
            }

            if (fromIndex + snippetLength >= wholeString.Length)
            {
                snippetLength = wholeString.Length - fromIndex;
            }
            else
            {
                suffix = Ellipses;
                snippetLength -= suffix.Length;
            }

            return '"' + prefix + Escape(wholeString.Substring(fromIndex, snippetLength)) + suffix + '"';
        }

        private int SnippetStart(string wholeString)
        {
            return Math.Max(0, Math.Min(FailureIndex - IdealArrowIndex, wholeString.Length - MaxStringWidth));
        }

        private static string Escape(string value)
        {
            return Escapes.Aggregate(value, (s, escape) => s.Replace(escape.Key.ToString(CultureInfo.InvariantCulture), escape.Value));
        }

        private static readonly Dictionary<char, string> Escapes = new Dictionary<char, string>
            {
                { '\r', "\\r" },
                { '\n', "\\n" }
            };
    }
}