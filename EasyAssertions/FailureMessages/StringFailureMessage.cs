using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building string assertion failure messages in a consistent format.
    /// </summary>
    public class StringFailureMessage : FailureMessage
    {
        private const int MaxStringWidth = 60;
        private const int IdealArrowIndex = 20;

        /// <summary>
        /// The string value that the actual string value was compared against.
        /// Outputs a snippet of the string around the <see cref="FailureMessage.FailureIndex"/>, wrapped in " ".
        /// </summary>
        public override object ExpectedValue
        {
            get { return GetSnippet((string)RawExpectedValue); }
            set { RawExpectedValue = value; }
        }

        /// <summary>
        /// The string value being asserted on.
        /// Outputs a snippet of the string around the <see cref="FailureMessage.FailureIndex"/>, wrapped in " ".
        /// </summary>
        public override object ActualValue
        {
            get { return GetSnippet((string)RawActualValue); }
            set { RawActualValue = value; }
        }

        /// <summary>
        /// Outputs a ^ character.
        /// When aligned with the actual or expected value in the <see cref="FailureMessage.MessageTemplate"/>,
        /// the ^ character will line up with the character at the <see cref="FailureMessage.FailureIndex"/>.
        /// </summary>
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

        /// <summary>
        /// Reduces a string down to a smaller snippet around the <see cref="FailureMessage.FailureIndex"/>,
        /// to avoid blowing out the size of the failure message. The result is escaped for SmartFormat.
        /// </summary>
        protected string GetSnippet(string wholeString)
        {
            int fromIndex = SnippetStart(wholeString);
            int snippetLength = MaxStringWidth;
            string prefix = string.Empty;
            string suffix = string.Empty;

            if (fromIndex > 0)
            {
                prefix = "...";
                snippetLength -= prefix.Length;
                fromIndex += prefix.Length;
            }

            if (fromIndex + snippetLength >= wholeString.Length)
            {
                snippetLength = wholeString.Length - fromIndex;
            }
            else
            {
                suffix = "...";
                snippetLength -= suffix.Length;
            }

            return EscapeForTemplate('"' + prefix + StringEscape(wholeString.Substring(fromIndex, snippetLength)) + suffix + '"');
        }

        private int SnippetStart(string wholeString)
        {
            return Math.Max(0, Math.Min(FailureIndex - IdealArrowIndex, wholeString.Length - MaxStringWidth));
        }

        /// <summary>
        /// Escapes new-lines in a string value for output in the failure message.
        /// </summary>
        protected static string StringEscape(string value)
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