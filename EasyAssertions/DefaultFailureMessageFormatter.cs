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
            return TestExpression.GetActual()
                + Environment.NewLine + Expected(ExpectedText, expected)
                + Environment.NewLine + ActualText + ObjectValue(actual)
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

            return TestExpression.GetActual() + Environment.NewLine
                + Expected(ExpectedText, expected, expectedSnippet) + Environment.NewLine
                + ActualText + StringValue(actualSnippet) + Environment.NewLine
                + ArrowPrefix + arrow + Environment.NewLine
                + "Difference at index " + differenceIndex + '.'
                + MessageOnNewLine(message);
        }

        public string AreEqual(object notExpected, object actual, string message = null)
        {
            return TestExpression.GetActual()
                + Environment.NewLine + Expected("should not be ", notExpected)
                + Environment.NewLine + "but was       " + ObjectValue(actual)
                + MessageOnNewLine(message);
        }

        private static string Expected(string expectationMessage, object expectedValue)
        {
            string expectedExpression = TestExpression.GetExpected();
            string expectedValueOutput = string.Empty + expectedValue;

            expectationMessage += expectedExpression == expectedValueOutput
                ? ObjectValue(expectedValueOutput)
                : expectedExpression
                    + Environment.NewLine + new string(' ', expectationMessage.Length) + ObjectValue(expectedValueOutput);

            return expectationMessage;
        }

        public string IsNull(string message = null)
        {
            return TestExpression.GetActual()
                 + Environment.NewLine + "should not be null, but was."
                 + MessageOnNewLine(message);
        }

        public string NotSame(object expected, object actual, string message = null)
        {
            return TestExpression.GetActual()
                + Environment.NewLine + Expected(ExpectedInstanceText, expected)
                + Environment.NewLine + ActualInstanceText + ObjectValue(actual)
                + MessageOnNewLine(message);
        }

        public string AreSame(object actual, string message = null)
        {
            return TestExpression.GetActual()
                + Environment.NewLine + Expected("shouldn't be instance ", actual)
                + MessageOnNewLine(message);
        }

        public string NotEmpty(IEnumerable actual, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return TestExpression.GetActual()
                + Environment.NewLine + "should be empty"
                + Environment.NewLine + ActualLengthElements(actualList)
                + MessageOnNewLine(message);
        }

        public string IsEmpty(string message = null)
        {
            return TestExpression.GetActual()
                + Environment.NewLine + "should not be empty, but was."
                + MessageOnNewLine(message);
        }

        public string LengthMismatch(int expectedLength, IEnumerable actual, string message = null)
        {
            return TestExpression.GetActual()
                + LengthDifference(expectedLength, actual)
                + MessageOnNewLine(message);
        }

        private static string ActualLengthElements(ICollection<object> actualList)
        {
            if (actualList.None())
                return "but was empty.";

            string message = "but had " + actualList.Count;

            if (actualList.Count == 1)
                message += " element: " + ObjectValue(actualList.Single());
            else
                message += " elements: ["
                    + SelectFirstFew(3, actualList, EnumerableElement, EnumerableEllipses).Join(",")
                    + Environment.NewLine + "]";

            return message;
        }

        public string DoesNotContain(object expected, IEnumerable actual, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return TestExpression.GetActual()
                + Environment.NewLine + Expected("should contain ", expected)
                + Environment.NewLine + "but was " + ActualElements(actualList, 6)
                + MessageOnNewLine(message);
        }

        public string DoesNotContainItems(IEnumerable expected, IEnumerable actual, string message = null)
        {
            HashSet<object> actualItems = new HashSet<object>(actual.Cast<object>());
            int missingItemIndex;
            object missingItem;
            FindMissingItem(expected, actualItems, out missingItemIndex, out missingItem);

            return TestExpression.GetActual()
                + ExpectedCollection(
                      Environment.NewLine + "should contain {0}"
                    + Environment.NewLine + "but was missing item " + missingItemIndex + ' ' + ObjectValue(missingItem)
                    + Environment.NewLine + "and was " + ActualElements(actualItems, 0)
                    ,
                      Environment.NewLine + "should contain expected item " + missingItemIndex + ' ' + ObjectValue(missingItem)
                    + Environment.NewLine + "but was " + ActualElements(actualItems, 0))
                + MessageOnNewLine(message);
        }

        public string DoesNotOnlyContain(IEnumerable expected, IEnumerable actual, string message = null)
        {
            if (!Compare.ContainsAllItems(actual, expected))
                return DoesNotContainItems(expected, actual, message);

            HashSet<object> expectedItems = new HashSet<object>(expected.Cast<object>());
            List<object> extraItems = actual.Cast<object>().Where(a => !expectedItems.Contains(a)).ToList();

            return TestExpression.GetActual()
                + ExpectedCollection(
                      Environment.NewLine + "should only contain {0}"
                    + Environment.NewLine + "but also contains " + ActualElements(extraItems, 0)
                    ,
                      Environment.NewLine + "should only contain " + ActualElements(expectedItems, 0, "nothing")
                    + Environment.NewLine + "but also contains " + ActualElements(extraItems, 0));
        }

        private static void FindMissingItem(IEnumerable expected, HashSet<object> actualItems, out int missingItemIndex, out object missingItem)
        {
            missingItemIndex = 0;
            missingItem = null;
            foreach (object expectedItem in expected)
            {
                if (!actualItems.Contains(expectedItem))
                {
                    missingItem = expectedItem;
                    break;
                }
                missingItemIndex++;
            }
        }

        private static string ActualElements(ICollection<object> actualList, int singleItemIndent, string emptyString = "empty.")
        {
            if (actualList.None())
                return emptyString;

            if (actualList.Count == 1)
                return new string(' ', singleItemIndent) + '[' + ObjectValue(actualList.Single()) + ']';

            return "["
                + SelectFirstFew(10, actualList, EnumerableElement, EnumerableEllipses).Join(",")
                + Environment.NewLine + "]";
        }

        private static string EnumerableElement(object item)
        {
            return Environment.NewLine + "    " + ObjectValue(item);
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

            if (expectedList.Count != actualList.Count)
                return TestExpression.GetActual() + ExpectedCollection(" doesn't match {0}.", string.Empty)
                    + LengthDifference(expectedList.Count, actualList)
                    + MessageOnNewLine(message);

            object expectedValue, actualValue;
            int differenceIndex = FindDifference(expectedList, actualList, Compare.ObjectsAreEqual, out expectedValue, out actualValue);

            return TestExpression.GetActual() + ExpectedCollection(" doesn't match {0}. D", " d") + "iffers at index " + differenceIndex + '.'
                + Environment.NewLine + ExpectedText + ObjectValue(expectedValue)
                + Environment.NewLine + ActualText + ObjectValue(actualValue)
                + MessageOnNewLine(message);
        }

        private static string LengthDifference(int expectedLength, IEnumerable actual)
        {
            return Environment.NewLine + "should have " + expectedLength + (expectedLength == 1 ? " element" : " elements")
                + Environment.NewLine + ActualLengthElements(actual.Cast<object>().ToList());
        }

        private static string ExpectedCollection(string expectedMessage, string defaultMessage)
        {
            string expectedExpression = TestExpression.GetExpected();
            return NewCollectionPattern.IsMatch(expectedExpression)
                ? defaultMessage
                : string.Format(expectedMessage, expectedExpression);
        }

        private static readonly Regex NewCollectionPattern = new Regex(@"^new.*\{.*\}");

        public string ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null)
        {
            object expectedValue, actualValue;
            int differenceIndex = FindDifference(expected, actual, ReferenceEquals, out expectedValue, out actualValue);

            return TestExpression.GetActual() + " differs at index " + differenceIndex + '.'
                + Environment.NewLine + ExpectedInstanceText + ObjectValue(expectedValue)
                + Environment.NewLine + ActualInstanceText + ObjectValue(actualValue)
                + MessageOnNewLine(message);
        }

        private static int FindDifference(IEnumerable expected, IEnumerable actual, Func<object, object, bool> areEqual, out object expectedValue, out object actualValue)
        {
            List<object> expectedList = expected.Cast<object>().ToList();
            List<object> actualList = actual.Cast<object>().ToList();
            return FindDifference(expectedList, actualList, areEqual, out expectedValue, out actualValue);
        }

        private static int FindDifference(IList<object> expectedList, IList<object> actualList, Func<object, object, bool> areEqual, out object expectedValue, out object actualValue)
        {
            int differenceIndex = Enumerable.Range(0, expectedList.Count)
                .First(i => !areEqual(actualList[i], expectedList[i]));

            expectedValue = expectedList[differenceIndex];
            actualValue = actualList[differenceIndex];

            return differenceIndex;
        }

        public string NoException(Type expectedExceptionType, LambdaExpression function, string message = null)
        {
            return CleanFunctionBody(function)
                + Environment.NewLine + ExpectedExceptionText + ObjectValue(expectedExceptionType.Name)
                + Environment.NewLine + "but didn't throw at all."
                + MessageOnNewLine(message);
        }

        public string WrongException(Type expectedExceptionType, Type actualExceptionType, LambdaExpression function, string message = null)
        {
            return CleanFunctionBody(function)
                + Environment.NewLine + ExpectedExceptionText + ObjectValue(expectedExceptionType.Name)
                + Environment.NewLine + ActualExceptionText + ObjectValue(actualExceptionType.Name)
                + MessageOnNewLine(message);
        }

        private static readonly Regex MemberPattern = new Regex(@"value\(.*?\)\.", RegexOptions.Compiled);

        private static string CleanFunctionBody(LambdaExpression function)
        {
            return MemberPattern.Replace(function.Body.ToString(), string.Empty);
        }

        public string DoesNotContain(string expectedSubstring, string actual, string message = null)
        {
            string actualSnippet = GetSnippet(actual, 0, MaxStringWidth);

            return TestExpression.GetActual()
                + Environment.NewLine + Expected("should contain ", expectedSubstring, expectedSubstring)
                + Environment.NewLine + "but was        " + StringValue(actualSnippet)
                + MessageOnNewLine(message);
        }

        public string DoesNotStartWith(string expectedStart, string actual, string message = null)
        {
            string actualSnippet = GetSnippet(actual, 0, MaxStringWidth);

            return TestExpression.GetActual()
                + Environment.NewLine + Expected("should start with ", expectedStart, expectedStart)
                + Environment.NewLine + "but starts with   " + StringValue(actualSnippet)
                + MessageOnNewLine(message);
        }

        public string DoesNotEndWith(string expectedEnd, string actual, string message = null)
        {
            int from = Math.Max(0, actual.Length - MaxStringWidth);
            string actualSnippet = GetSnippet(actual, from, MaxStringWidth);

            return TestExpression.GetActual()
                + Environment.NewLine + Expected("should end with ", expectedEnd, expectedEnd)
                + Environment.NewLine + "but ends with   " + StringValue(actualSnippet)
                + MessageOnNewLine(message);
        }

        private static string Expected(string expectationMessage, string expectedValue, string expectedSnippet)
        {
            string expectedExpression = TestExpression.GetExpected();

            expectationMessage += expectedExpression.Trim('@', '"') == expectedValue
                ? StringValue(expectedSnippet)
                : expectedExpression
                    + Environment.NewLine + new string(' ', expectationMessage.Length) + StringValue(expectedSnippet);

            return expectationMessage;
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

        private static string ObjectValue(object value)
        {
            return "<" + value + ">";
        }

        private static string StringValue(string expectedSnippet)
        {
            return '"' + Escape(expectedSnippet) + '"';
        }

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
