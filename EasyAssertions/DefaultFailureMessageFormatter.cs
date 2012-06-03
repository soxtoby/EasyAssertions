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
        private const string ExpectedText = "should be ";
        private const string ExpectedInstanceText = "should be instance ";
        private const string ExpectedExceptionText = "should throw ";
        private const string ActualText = "but was   ";
        private const string ActualInstanceText = "but was            ";
        private const string ActualExceptionText = "but threw    ";
        private const string ArrowPrefix = "           ";
        private const string Ellipses = "...";
        private const int MaxStringWidth = 60;
        private const int MaxArrowIndex = 20;
        private static readonly string EnumerableEllipses = Environment.NewLine + "    " + Ellipses;

        private DefaultFailureMessageFormatter() { }

        public static readonly DefaultFailureMessageFormatter Instance = new DefaultFailureMessageFormatter();

        public string NotEqual(object expected, object actual, string message = null)
        {
            return TestExpression.Get()
                + Environment.NewLine + ExpectedText + '<' + expected + '>'
                + Environment.NewLine + ActualText + '<' + actual + '>'
                + MessageOnNewLine(message);
        }

        public string NotEqual(string expected, string actual, string message = null)
        {
            int differenceIndex = Enumerable.Range(0, expected.Length)
                .First(i => actual[i] != expected[i]);

            int from = Math.Max(0, differenceIndex - MaxArrowIndex);

            string expectedSnippet = GetSnippet(expected, from, MaxStringWidth);
            string actualSnippet = GetSnippet(actual, from, MaxStringWidth);

            int arrowIndex = differenceIndex - from;
            arrowIndex += actualSnippet.Substring(0, arrowIndex).Count(c => Escapes.ContainsKey(c));
            string arrow = new string(' ', arrowIndex) + '^';

            return TestExpression.Get() + Environment.NewLine
                + ExpectedText + '"' + Escape(expectedSnippet) + '"' + Environment.NewLine
                + ActualText + '"' + Escape(actualSnippet) + '"' + Environment.NewLine
                + ArrowPrefix + arrow + Environment.NewLine
                + "Difference at index " + differenceIndex + '.'
                + MessageOnNewLine(message);
        }

        public string AreEqual(object notExpected, object actual, string message = null)
        {
            return TestExpression.Get()
                + Environment.NewLine + "should not be <" + notExpected + '>'
                + Environment.NewLine + "but was       <" + actual + '>'
                + MessageOnNewLine(message);
        }

        public string NotSame(object expected, object actual, string message = null)
        {
            return TestExpression.Get()
                + Environment.NewLine + ExpectedInstanceText + '<' + expected + '>'
                + Environment.NewLine + ActualInstanceText + '<' + actual + '>'
                + MessageOnNewLine(message);
        }

        public string AreSame(object actual, string message = null)
        {
            return TestExpression.Get()
                + Environment.NewLine + "shouldn't be instance <" + actual + '>'
                + MessageOnNewLine(message);
        }

        public string NotEmpty(IEnumerable actual, string message = null)
        {
            var actualList = actual.Cast<object>().ToList();

            return TestExpression.Get()
                + Environment.NewLine + "should be empty"
                + Environment.NewLine + ActualElements(actualList)
                + MessageOnNewLine(message);
        }

        public string IsEmpty(string message = null)
        {
            return TestExpression.Get()
                + Environment.NewLine + "should not be empty, but was."
                + MessageOnNewLine(message);
        }

        public string LengthMismatch(IEnumerable actual, int expectedLength, string message = null)
        {
            return TestExpression.Get()
                + Environment.NewLine + "should have " + expectedLength + (expectedLength == 1 ? " element" : " elements")
                + Environment.NewLine + ActualElements(actual.Cast<object>().ToList())
                + MessageOnNewLine(message);
        }

        private static string ActualElements(ICollection<object> actualList)
        {
            if (actualList.None())
                return "but was empty.";

            string message = "but had " + actualList.Count;

            if (actualList.Count == 1)
                message += " element: <" + actualList.Single() + '>';
            else
                message += " elements: ["
                    + SelectFirstFew(3, actualList, EnumerableElement, EnumerableEllipses).Join(",")
                        + Environment.NewLine + "]";

            return message;
        }

        private static string EnumerableElement(object i)
        {
            return Environment.NewLine + "    <" + i + '>';
        }

        private static IEnumerable<string> SelectFirstFew<T>(int count, IEnumerable<T> enumerable, Func<T, string> select, string extra)
        {
            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                int i = 0;
                while (enumerator.MoveNext())
                {
                    if (i++ == count)
                    {
                        yield return extra;
                        break;
                    }
                    yield return select(enumerator.Current);
                }
            }
        }

        public string DoNotMatch(IEnumerable expected, IEnumerable actual, string message = null)
        {
            List<object> expectedList = expected.Cast<object>().ToList();
            List<object> actualList = actual.Cast<object>().ToList();
            int differenceIndex = Enumerable.Range(0, expectedList.Count)
                .First(i => !Compare.ObjectsAreEqual(actualList[i], expectedList[i]));

            object expectedValue = expectedList[differenceIndex];
            object actualValue = actualList[differenceIndex];

            return TestExpression.Get() + " differs at index " + differenceIndex + '.'
                + Environment.NewLine + ExpectedText + '<' + expectedValue + '>'
                + Environment.NewLine + ActualText + '<' + actualValue + '>'
                + MessageOnNewLine(message);
        }

        public string NoException(Type expectedExceptionType, Expression<Action> function, string message = null)
        {
            return CleanFunctionBody(function)
                + Environment.NewLine + ExpectedExceptionText + '<' + expectedExceptionType.Name + '>'
                + Environment.NewLine + "but didn't throw at all."
                + MessageOnNewLine(message);
        }

        public string WrongException(Type expectedExceptionType, Type actualExceptionType, Expression<Action> function, string message = null)
        {
            return CleanFunctionBody(function)
                + Environment.NewLine + ExpectedExceptionText + '<' + expectedExceptionType.Name + '>'
                + Environment.NewLine + ActualExceptionText + '<' + actualExceptionType.Name + '>'
                + MessageOnNewLine(message);
        }

        private static readonly Regex MemberPattern = new Regex(@"value\(.*?\)\.", RegexOptions.Compiled);

        private static string CleanFunctionBody(Expression<Action> function)
        {
            return MemberPattern.Replace(function.Body.ToString(), string.Empty);
        }

        public string DoesNotContain(string expectedSubstring, string actual, string message = null)
        {
            string actualSnippet = GetSnippet(actual, 0, MaxStringWidth);

            return TestExpression.Get() + Environment.NewLine
                + "should contain \"" + Escape(expectedSubstring) + '"' + Environment.NewLine
                + "but was        \"" + Escape(actualSnippet) + '"'
                + MessageOnNewLine(message);
        }

        public string DoesNotEndWith(string expectedEnd, string actual, string message = null)
        {
            int from = Math.Max(0, actual.Length - MaxStringWidth);

            return TestExpression.Get()
                + Environment.NewLine + "should end with \"" + Escape(expectedEnd) + '"'
                    + Environment.NewLine + "but ends with   \"" + Escape(GetSnippet(actual, @from, MaxStringWidth)) + '"'
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

        private static readonly Dictionary<char, string> Escapes = new Dictionary<char, string>
            {
                { '\r', "\\r" },
                { '\n', "\\n" }
            };

        private static string Escape(string value)
        {
            return Escapes.Aggregate(value, (s, escape) => s.Replace(escape.Key.ToString(CultureInfo.InvariantCulture), escape.Value));
        }

        private static string MessageOnNewLine(string message)
        {
            return message == null
                ? string.Empty
                : Environment.NewLine + message;
        }
    }
}