using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EasyAssertions
{
    /// <summary>
    /// A helper class for building assertion failure messages in a consistent format.
    /// </summary>
    public static class FailureMessage
    {
        private const int SampleSize = 10;
        private const int MaxStringWidth = 60;
        private const int IdealArrowIndex = 20;
        private static readonly Regex NewCollectionPattern = new Regex(@"^new.*\{.*\}", RegexOptions.Compiled);
        private static readonly Regex MemberPattern = new Regex(@"value\(.*?\)\.", RegexOptions.Compiled);
        private static readonly Regex BoxingPattern = new Regex(@"^Convert\((.*)\)$", RegexOptions.Compiled);

        /// <summary>
        /// Returns the source representation of the actual value.
        /// </summary>
        public static string ActualExpression => TestExpression.GetActual().NullIfEmpty() ?? "Actual value";

        /// <summary>
        /// Returns the source representation of the expected value, if available,
        /// and a snippet of the expected value underneath it.
        /// </summary>
        public static string Expected(string expectedValue, string actualValue, int differenceIndex, string indent)
        {
            return Expected(Snippet(expectedValue, actualValue, differenceIndex), indent);
        }

        /// <summary>
        /// Returns the source representation of the expected value, if available,
        /// and the expected value underneath it.
        /// </summary>
        public static string Expected(object expectedValue, string indent)
        {
            string sourceExpression = ExpectedSourceIfDifferentToValue(expectedValue);
            return sourceExpression == null
                ? Value(expectedValue)
                : sourceExpression + indent + Value(expectedValue);
        }

        /// <summary>
        /// Returns the source representation of the expected value.
        /// Returns null if the source representation is the same as the value output
        /// (e.g. if the source was a literal value)
        /// </summary>
        public static string ExpectedSourceIfDifferentToValue(object expectedValue)
        {
            string expectedExpression = TestExpression.GetExpected() ?? string.Empty;

            return MatchesExpectedValueOutput(expectedExpression, expectedValue)
                   || IsStringLiteral(expectedExpression)
                   || IsNumericLiteral(expectedExpression)
                   || IsBooleanLiteral(expectedExpression, expectedValue)
                   || IsCharLiteral(expectedExpression)
                   || IsNullLiteral(expectedExpression)
                   || NewCollectionPattern.IsMatch(expectedExpression)
                ? null
                : expectedExpression.NullIfEmpty();
        }

        private static bool MatchesExpectedValueOutput(string expectedExpression, object expectedValue)
        {
            string expectedValueOutput = string.Empty + expectedValue;
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

        private static bool IsBooleanLiteral(string expectedExpression, object expectedValue)
        {
            return expectedValue is bool
                   && expectedExpression == expectedValue.ToString().ToLower();
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
        /// Returns <see cref="singleMessage"/> or <see cref="multipleMessage"/>, depending on whether
        /// <see cref="collection"/> has 1 element or more.
        /// </summary>
        public static string Count(ICollection<object> collection, string singleMessage, string multipleMessage)
        {
            return Count(collection.Count, singleMessage, multipleMessage);
        }

        /// <summary>
        /// Returns <see cref="singleMessage"/> or <see cref="multipleMessage"/>, depending on whether
        /// <see cref="count"/> is 1 or higher.
        /// </summary>
        public static string Count(int count, string singleMessage, string multipleMessage)
        {
            return count == 1 ? singleMessage : multipleMessage;
        }

        /// <summary>
        /// Returns <see cref="emptyMessage"/>, <see cref="singleMessage"/> or <see cref="multipleMessage"/>, depending on
        /// whether <see cref="collection"/> has no elements, a single element, or more elements.
        /// </summary>
        public static string Count(ICollection<object> collection, string emptyMessage, string singleMessage, string multipleMessage)
        {
            return Count(collection.Count, emptyMessage, singleMessage, multipleMessage);
        }

        /// <summary>
        /// Returns <see cref="emptyMessage"/>, <see cref="singleMessage"/> or <see cref="multipleMessage"/>, depending on
        /// whether <see cref="count"/> is 0, 1 or higher.
        /// </summary>
        public static string Count(int count, string emptyMessage, string singleMessage, string multipleMessage)
        {
            return count == 0 ? emptyMessage
                : count == 1 ? singleMessage
                    : multipleMessage;
        }

        /// <summary>
        /// Returns a string representation of the first item in <see cref="collection"/>.
        /// </summary>
        public static string Single(ICollection<object> collection)
        {
            return Value(collection.FirstOrDefault());
        }

        /// <summary>
        /// Returns a sample of the first ten items in the given collection,
        /// or "empty." if there are no items in the collection.
        /// </summary>
        public static string Sample(ICollection<object> items)
        {
            switch (items.Count)
            {
                case 0:
                    return "empty.";
                case 1:
                    return "[" + Value(items.Single()) + "]";
                default:
                    return $@"[{SampleItems(items).Select(i => Environment.NewLine + "    " + i)
                        .Join(",")}
]";
            }
        }

        private static IEnumerable<string> SampleItems(ICollection<object> items)
        {
            return items.Count > SampleSize
                ? items.Take(SampleSize).Select(Value).Concat(new[] { "..." }).ToList()
                : items.Select(Value);
        }

        /// <summary>
        /// Outputs a ^ character.
        /// When aligned with the actual or expected value in an error message,
        /// the ^ character will line up with the character at the specified index.
        /// </summary>
        public static string Arrow(string actualValue, string expectedValue, int failureIndex)
        {
            int arrowIndex = failureIndex - SnippetStart(actualValue, expectedValue, failureIndex);
            arrowIndex += ((string)actualValue)
                .Substring(0, arrowIndex)
                .Count(Escapes.ContainsKey);

            return new string(' ', arrowIndex + 1) + '^';   // + 1 for the quotes wrapped around string values
        }

        private static string Snippet(string wholeString, string otherString, int failureIndex)
        {
            int fromIndex = SnippetStart(wholeString, otherString, failureIndex);
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

            return prefix + StringEscape(wholeString.Substring(fromIndex, snippetLength)) + suffix;
        }

        private static int SnippetStart(string wholeString, string otherString, int failureIndex)
        {
            int maxLength = Math.Max(wholeString.Length, otherString.Length);
            return Math.Max(0, Math.Min(failureIndex - IdealArrowIndex, maxLength - MaxStringWidth));
        }
        
        private static string StringEscape(string value)
        {
            return Escapes.Aggregate(value, (s, escape) => s.Replace(escape.Key.ToString(CultureInfo.InvariantCulture), escape.Value));
        }

        private static readonly Dictionary<char, string> Escapes = new Dictionary<char, string>
            {
                { '\r', "\\r" },
                { '\n', "\\n" }
            };

        /// <summary>
        /// Renders a string value as a snippet.
        /// </summary>
        public static string Value(string value)
        {
            return '"' + Snippet(value, value, 0) + '"';
        }

        /// <summary>
        /// Renders a string value as a snippet around the <see cref="differenceIndex"/>,
        /// with the goal of lining up with a snippet of <see cref="otherValue"/>.
        /// </summary>
        public static string Value(string value, string otherValue, int differenceIndex)
        {
            return '"' + Snippet(value, otherValue, differenceIndex) + '"';
        }

        /// <summary>
        /// Returns whitespace with the same length as <see cref="text"/>.
        /// </summary>
        public static string SpaceFor(string text)
        {
            return new string(' ', text.Length);
        }

        /// <summary>
        /// Outputs a path of nodes.
        /// </summary>
        public static string Path(ICollection<object> path)
        {
            return new[] { "root" }
                .Concat(
                    (path ?? Enumerable.Empty<object>()))
                .Select(v => v.ToString())
                .Join(" -> ");
        }

        /// <summary>
        /// Returns the source representation of <see cref="function"/>.
        /// </summary>
        public static string Value(LambdaExpression function)
        {
            string body = function.Body.ToString();
            body = MemberPattern.Replace(body, string.Empty);
            body = BoxingPattern.Replace(body, "$1");
            return body;
        }

        /// <summary>
        /// Returns a string representation of <see cref="type"/>.
        /// </summary>
        public static string Value(Type type)
        {
            return $"<{type.Name}>";
        }

        /// <summary>
        /// Returns a string representation of <see cref="regex"/>.
        /// </summary>
        public static string Value(Regex regex)
        {
            string value = "/" + regex + "/";

            if (regex.Options != RegexOptions.None)
                value += " {" + regex.Options + "}";

            return value;
        }

        /// <summary>
        /// Returns a string representation of <see cref="timespan"/>.
        /// </summary>
        public static string Value(TimeSpan timespan)
        {
            return timespan.TotalMilliseconds < 1 ? $"{timespan.Ticks} ticks"
                : timespan.TotalSeconds < 1 ? timespan.Milliseconds + "ms"
                : timespan.ToString();
        }

        /// <summary>
        /// Returns a string representation of an object.
        /// </summary>
        public static string Value(object value)
        {
            string str = value as string;
            Regex regex = value as Regex;

            return str != null ? Value(str)
                : regex != null ? Value(regex)
                : "<" + (value ?? "null") + ">";
        }

        /// <summary>
        /// Returns <see cref="value"/> on a new line if it has anything in it.
        /// </summary>
        public static string OnNewLine(this string value)
        {
            return string.IsNullOrEmpty(value) 
                ? string.Empty
                : Environment.NewLine + value;
        }

        /// <summary>
        /// Returns <see cref="value"/> with a trailing space, if it has anything in it.
        /// </summary>
        public static string WithSpace(this string value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value + " ";
        }
    }
}