using System;
using System.Collections;
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
    public static class MessageHelper
    {
        private const int SampleSize = 10;
        private const int MaxStringWidth = 60;
        private const int IdealArrowIndex = 20;
        private static readonly Regex NewCollectionPattern = new(@"^new.*\{.*\}", RegexOptions.Compiled);
        private static readonly Regex MemberPattern = new(@"value\(.*?\)\.", RegexOptions.Compiled);
        private static readonly Regex BoxingPattern = new(@"^Convert\((.*)\)$", RegexOptions.Compiled);

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
        public static string Expected(object? expectedValue, string indent)
        {
            var sourceExpression = ExpectedSourceIfDifferentToValue(expectedValue);
            return sourceExpression == null
                ? Value(expectedValue)
                : sourceExpression + indent + Value(expectedValue);
        }

        /// <summary>
        /// Returns the source representation of the expected value.
        /// Returns null if the source representation is the same as the value output
        /// (e.g. if the source was a literal value)
        /// </summary>
        public static string? ExpectedSourceIfDifferentToValue(object? expectedValue)
        {
            var expectedExpression = TestExpression.GetExpected();

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

        private static bool MatchesExpectedValueOutput(string expectedExpression, object? expectedValue)
        {
            var expectedValueOutput = string.Empty + expectedValue;
            return expectedExpression == expectedValueOutput;
        }

        private static bool IsStringLiteral(string expectedExpression)
        {
            return expectedExpression.TrimStart('@').FirstOrDefault() == '"';
        }

        private static bool IsNumericLiteral(string expectedExpression)
        {
            var firstChar = expectedExpression.FirstOrDefault();
            return firstChar >= 48
                   && firstChar <= 57;
        }

        private static bool IsBooleanLiteral(string expectedExpression, object? expectedValue)
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
        /// Returns <paramref name="singleMessage"/> or <paramref name="multipleMessage"/>, depending on whether
        /// <paramref name="collection"/> has 1 element or more.
        /// </summary>
        public static string Count(ICollection<object?> collection, string singleMessage, string multipleMessage)
        {
            return Count(collection.Count, singleMessage, multipleMessage);
        }

        /// <summary>
        /// Returns <paramref name="singleMessage"/> or <paramref name="multipleMessage"/>, depending on whether
        /// <paramref name="count"/> is 1 or higher.
        /// </summary>
        public static string Count(int count, string singleMessage, string multipleMessage)
        {
            return count == 1 ? singleMessage : multipleMessage;
        }

        /// <summary>
        /// Returns <paramref name="emptyMessage"/>, <paramref name="singleMessage"/> or <paramref name="multipleMessage"/>, depending on
        /// whether <paramref name="collection"/> has no elements, a single element, or more elements.
        /// </summary>
        public static string Count(ICollection<object?> collection, string emptyMessage, string singleMessage, string multipleMessage)
        {
            return Count(collection.Count, emptyMessage, singleMessage, multipleMessage);
        }

        /// <summary>
        /// Returns <paramref name="emptyMessage"/>, <paramref name="singleMessage"/> or <paramref name="multipleMessage"/>, depending on
        /// whether <paramref name="count"/> is 0, 1 or higher.
        /// </summary>
        public static string Count(int count, string emptyMessage, string singleMessage, string multipleMessage)
        {
            return count == 0 ? emptyMessage
                : count == 1 ? singleMessage
                    : multipleMessage;
        }

        /// <summary>
        /// Returns a string representation of the first item in <paramref name="collection"/>.
        /// </summary>
        public static string Single(ICollection<object?> collection)
        {
            return Value(collection.FirstOrDefault());
        }

        /// <summary>
        /// Returns a sample of the first ten items in the given collection,
        /// or "empty." if there are no items in the collection.
        /// </summary>
        public static string Sample<T>(ICollection<T> items)
        {
            if (items.None())
                return "empty.";

            var remainingItems = SampleSize;
            var sample = BuildSample(items);

            return sample.Count == 1
                ? $"[ {sample[0]} ]"
                : "[" + sample.Select(i => Environment.NewLine + "    " + i).Join("") + Environment.NewLine + "]";

            List<string> BuildSample(IEnumerable innerItems)
            {
                var innerSample = new List<string>();

                foreach (var item in innerItems)
                {
                    if (innerSample.Any())
                        innerSample[innerSample.Count - 1] += ",";

                    if (remainingItems == 0)
                    {
                        innerSample.Add("...");
                        break;
                    }

                    if (item is IEnumerable enumerableItem && !(item is string))
                    {
                        var itemSample = BuildSample(enumerableItem);
                        switch (itemSample.Count)
                        {
                            case 0:
                                innerSample.Add("[]");
                                remainingItems--;
                                break;
                            case 1:
                                innerSample.Add($"[ {itemSample[0]} ]");
                                break;
                            default:
                                innerSample.Add("[");
                                innerSample.AddRange(itemSample.Select(s => "    " + s));
                                innerSample.Add("]");
                                break;
                        }
                    }
                    else
                    {
                        innerSample.Add(Value(item));
                        remainingItems--;
                    }
                }

                return innerSample;
            }
        }

        /// <summary>
        /// Outputs a ^ character.
        /// When aligned with the actual or expected value in an error message,
        /// the ^ character will line up with the character at the specified index.
        /// </summary>
        public static string Arrow(string actualValue, string expectedValue, int failureIndex)
        {
            var arrowIndex = failureIndex - SnippetStart(actualValue, expectedValue, failureIndex);
            arrowIndex += actualValue
                .Substring(0, arrowIndex)
                .Count(Escapes.ContainsKey);

            return new string(' ', arrowIndex + 1) + '^';   // + 1 for the quotes wrapped around string values
        }

        private static string Snippet(string wholeString, string otherString, int failureIndex)
        {
            var fromIndex = SnippetStart(wholeString, otherString, failureIndex);
            var snippetLength = MaxStringWidth;
            var prefix = string.Empty;
            var suffix = string.Empty;

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
            var maxLength = Math.Max(wholeString.Length, otherString.Length);
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
        /// Renders a string value as a snippet around the <paramref name="differenceIndex"/>,
        /// with the goal of lining up with a snippet of <paramref name="otherValue"/>.
        /// </summary>
        public static string Value(string value, string otherValue, int differenceIndex)
        {
            return '"' + Snippet(value, otherValue, differenceIndex) + '"';
        }

        /// <summary>
        /// Returns whitespace with the same length as <paramref name="text"/>.
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
                .Concat(path)
                .Select(v => v.ToString())
                .Join(" -> ");
        }

        /// <summary>
        /// Returns the source representation of <paramref name="function"/>.
        /// </summary>
        public static string Value(LambdaExpression function)
        {
            var body = function.Body.ToString();
            body = MemberPattern.Replace(body, string.Empty);
            body = BoxingPattern.Replace(body, "$1");
            return body;
        }

        /// <summary>
        /// Returns a string representation of <paramref name="type"/>.
        /// </summary>
        public static string Value(Type type)
        {
            return $"<{type.Name}>";
        }

        /// <summary>
        /// Returns a string representation of <paramref name="regex"/>.
        /// </summary>
        public static string Value(Regex regex)
        {
            var value = "/" + regex + "/";

            if (regex.Options != RegexOptions.None)
                value += " {" + regex.Options + "}";

            return value;
        }

        /// <summary>
        /// Returns a string representation of <paramref name="timespan"/>.
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
        public static string Value(object? value)
        {
            return value switch
                {
                    string str => Value(str),
                    Regex regex => Value(regex),
                    _ => "<" + (value ?? "null") + ">"
                };
        }

        /// <summary>
        /// Returns <paramref name="value"/> on a new line if it has anything in it.
        /// </summary>
        public static string OnNewLine(this string? value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : Environment.NewLine + value;
        }

        /// <summary>
        /// Returns <paramref name="value"/> with a trailing space, if it has anything in it.
        /// </summary>
        public static string WithSpace(this string? value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value + " ";
        }
    }
}