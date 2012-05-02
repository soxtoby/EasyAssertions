using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EasyAssertions
{
    internal class DefaultFailureMessageFormatter : IFailureMessageFormatter
    {
        private const string ExpectedText = "Expected: ";
        private const string ActualText = "Actual:   ";
        private const string ArrowPrefix = "           ";
        private const string Ellipses = "...";

        private DefaultFailureMessageFormatter() { }

        public static readonly DefaultFailureMessageFormatter Instance = new DefaultFailureMessageFormatter();

        public string NotSame(object expected, object actual, string message = null)
        {
            return "Not the same object." + Environment.NewLine + NotEqual(expected, actual, message);
        }

        public string DoNotMatch(IEnumerable expected, IEnumerable actual, string message = null)
        {
            List<object> expectedList = expected.Cast<object>().ToList();
            List<object> actualList = actual.Cast<object>().ToList();
            int differenceIndex = Enumerable.Range(0, expectedList.Count)
                .First(i => !Compare.ObjectsAreEqual(actualList[i], expectedList[i]));
            return "Enumerables differ at index " + differenceIndex + '.' + Environment.NewLine + NotEqual(expectedList[differenceIndex], actualList[differenceIndex], message);
        }

        public string DoesNotContain(string expectedSubstring, string actual, string message = null)
        {
            return "Expected to contain: \"" + expectedSubstring + '"' + Environment.NewLine
                + ActualText + actual
                + MessageOnNewLine(message);
        }

        public string NotEqual(object expected, object actual, string message = null)
        {
            return ExpectedText + '<' + expected + '>'
                + Environment.NewLine + ActualText + '<' + actual + '>'
                + MessageOnNewLine(message);
        }

        public string NotEqual(string expected, string actual, string message = null)
        {
            int differenceIndex = Enumerable.Range(0, expected.Length)
                .First(i => actual[i] != expected[i]);

            const int maxStringWidth = 60;
            const int maxArrowIndex = 20;

            int from = Math.Max(0, differenceIndex - maxArrowIndex);

            string expectedSnippet = GetSnippet(expected, from, maxStringWidth);
            string actualSnippet = GetSnippet(actual, from, maxStringWidth);

            int arrowIndex = differenceIndex - from;
            arrowIndex += actualSnippet.Substring(0, arrowIndex).Count(c => Escapes.ContainsKey(c));
            string arrow = new string(' ', arrowIndex) + '^';

            return "Strings differ at index " + differenceIndex + '.' + Environment.NewLine
                + ExpectedText + '"' + Escape(expectedSnippet) + '"' + Environment.NewLine
                + ActualText + '"' + Escape(actualSnippet) + '"' + Environment.NewLine
                + ArrowPrefix + arrow
                + MessageOnNewLine(message);
        }

        private static string GetSnippet(string wholeString, int fromIndex, int maxLength)
        {
            int snippetLength = maxLength;
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

            return prefix + wholeString.Substring(fromIndex, snippetLength) + suffix;
        }

        private static string Escape(string value)
        {
            return Escapes.Aggregate(value, (s, escape) => s.Replace(escape.Key.ToString(CultureInfo.InvariantCulture), escape.Value));
        }

        public string NoException(Type expectedExceptionType, Expression<Action> function, string message = null)
        {
            return CleanFunctionBody(function) + " didn't throw." + Environment.NewLine
                + ExpectedText + '<' + expectedExceptionType.Name + '>'
                + MessageOnNewLine(message);
        }

        public string WrongException(Type expectedExceptionType, Type actualExceptionType, Expression<Action> function, string message = null)
        {
            return "Wrong exception type thrown by " + CleanFunctionBody(function) + Environment.NewLine
                + ExpectedText + '<' + expectedExceptionType.Name + '>' + Environment.NewLine
                + ActualText + '<' + actualExceptionType.Name + '>'
                + MessageOnNewLine(message);
        }

        private static string CleanFunctionBody(Expression<Action> function)
        {
            return MemberPattern.Replace(function.Body.ToString(), string.Empty);
        }

        private static readonly Regex MemberPattern = new Regex(@"value\(.*?\)\.", RegexOptions.Compiled);

        private static readonly Dictionary<char, string> Escapes = new Dictionary<char, string>
            {
                { '\r', "\\r" },
                { '\n', "\\n" }
            };

        private static string MessageOnNewLine(string message)
        {
            return message == null
                ? string.Empty
                : Environment.NewLine + message;
        }
    }
}