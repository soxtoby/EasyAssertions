using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EasyAssertions
{
    internal class DefaultFailureMessageFormatter : IFailureMessageFormatter
    {
        private DefaultFailureMessageFormatter() { }

        public static readonly DefaultFailureMessageFormatter Instance = new DefaultFailureMessageFormatter();

        public string NotEqual(object expected, object actual, string message = null)
        {
            return new FailureMessage
                {
                    ExpectedValue = expected,
                    ActualValue = actual,
                    UserMessage = message,
                    MessageTemplate = "should be {Expected:{Value}}{BR}"
                                    + "but was   {ActualValue}"
                }.ToString();
        }

        public string NotEqual(string expected, string actual, string message = null)
        {
            int differenceIndex = Enumerable.Range(0, expected.Length)
                .FirstOrDefault(i => actual[i] != expected[i]);

            return new StringFailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expected,
                    FailureIndex = differenceIndex,
                    UserMessage = message,
                    MessageTemplate = "should be {Expected:{Value}}{BR}"
                                    + "but was   {ActualValue}{BR}"
                                    + "          {Arrow}{BR}"
                                    + "Difference at index {FailureIndex}."
                }.ToString();
        }

        public string AreEqual(object notExpected, object actual, string message = null)
        {
            return new FailureMessage
                {
                    ExpectedValue = notExpected,
                    ActualValue = actual,
                    UserMessage = message,
                    MessageTemplate = "should not be {Expected:{Value}}{BR}"
                                    + "but was       {ActualValue}"
                }.ToString();
        }

        public string IsNull(string message = null)
        {
            return new FailureMessage
                {
                    UserMessage = message,
                    MessageTemplate = "should not be null, but was."
                }.ToString();
        }

        public string NotSame(object expected, object actual, string message = null)
        {
            return new FailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expected,
                    UserMessage = message,
                    MessageTemplate = "should be instance {Expected:{Value}}{BR}"
                                    + "but was            {ActualValue}"
                }.ToString();
        }

        public string AreSame(object actual, string message = null)
        {
            return new FailureMessage
                {
                    ExpectedValue = actual,
                    UserMessage = message,
                    MessageTemplate = "shouldn't be instance {Expected:{Value}}"
                }.ToString();
        }

        public string NotEmpty(IEnumerable actual, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return new CollectionFailureMessage
                {
                    ActualItems = actualList,
                    UserMessage = message,
                    MessageTemplate = "should be empty{BR}"
                                    + "but {ActualItems.Count:"
                                        + "had 1 element: {0.ActualItems[0]}"
                                        + "|had {} elements: {0.ActualSample}"
                                    + "}"
                }.ToString();
        }

        public string IsEmpty(string message = null)
        {
            return new FailureMessage
                {
                    UserMessage = message,
                    MessageTemplate = "should not be empty, but was."
                }.ToString();
        }

        public string LengthMismatch(int expectedLength, IEnumerable actual, string message = null)
        {
            return new CollectionFailureMessage
                {
                    FailureIndex = expectedLength,
                    ActualItems = actual.Cast<object>().ToList(),
                    UserMessage = message,
                    MessageTemplate = "should have {FailureIndex} {FailureIndex:element|elements}{BR}"
                                    + "but {ActualItems.Count:"
                                        + "was empty."
                                        + "|had 1 element: {0.ActualItems[0]}"
                                        + "|had {} elements: {0.ActualSample}"
                                    + "}"
                }.ToString();
        }

        public string DoesNotContain(object expected, IEnumerable actual, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return new CollectionFailureMessage
                {
                    ActualItems = actualList,
                    ExpectedValue = expected,
                    UserMessage = message,
                    MessageTemplate = "should contain {Expected:{Value}}{BR}"
                                    + "but was {ActualItems.Count:"
                                            + "empty."
                                           + "|      [{0.ActualItems[0]}]"
                                           + "|{0.ActualSample}"
                                    + "}"
                }.ToString();
        }

        public string DoesNotContainItems(IEnumerable expected, IEnumerable actual, string message = null)
        {
            HashSet<object> actualItems = new HashSet<object>(actual.Cast<object>());
            List<object> expectedItems = expected.Cast<object>().ToList();
            int missingItemIndex;
            object missingItem;
            FindMissingItem(expectedItems, actualItems, out missingItemIndex, out missingItem);

            return new CollectionFailureMessage
                {
                    ActualItems = actualItems,
                    ExpectedItems = expectedItems,
                    ExpectedValue = missingItem,
                    FailureIndex = missingItemIndex,
                    UserMessage = message,
                    MessageTemplate = "{ExpectedExpression:{0:"
                                        + "should contain {ExpectedExpression}{BR}"
                                        + "but was missing item {FailureIndex} {ExpectedValue}{BR}"
                                        + "and was {ActualSample}"
                                    + "}|{0:"
                                        + "should contain expected item {FailureIndex} {ExpectedValue}{BR}"
                                        + "but was {ActualSample}"
                                    + "}}"
                }.ToString();
        }

        public string DoesNotOnlyContain(IEnumerable expected, IEnumerable actual, string message = null)
        {
            if (expected.Cast<object>().None())
                return NotEmpty(actual, message);

            if (!Compare.ContainsAllItems(actual, expected))
                return DoesNotContainItems(expected, actual, message);

            HashSet<object> expectedItems = new HashSet<object>(expected.Cast<object>());
            List<object> extraItems = actual.Cast<object>().Where(a => !expectedItems.Contains(a)).ToList();

            return new CollectionFailureMessage
                {
                    ActualItems = extraItems,
                    ExpectedItems = expectedItems,
                    UserMessage = message,
                    MessageTemplate = "{ExpectedExpression:{0:"
                                        + "should only contain {ExpectedExpression}{BR}"
                                        + "but also contains {ActualSample}"
                                    + "}|{0:"
                                        + "should only contain {ExpectedSample}{BR}"
                                        + "but also contains {ActualSample}"
                                    + "}}"
                }.ToString();
        }

        private static void FindMissingItem(IEnumerable expected, ICollection<object> actualItems, out int missingItemIndex, out object missingItem)
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

        public string DoNotMatch(IEnumerable expected, IEnumerable actual, string message = null)
        {
            List<object> expectedList = expected.Cast<object>().ToList();
            List<object> actualList = actual.Cast<object>().ToList();

            if (expectedList.Count != actualList.Count)
                return new CollectionFailureMessage
                    {
                        ActualItems = actualList,
                        ExpectedItems = expectedList,
                        UserMessage = message,
                        MessageTemplate = "{ExpectedExpression:doesn't match {}.{0.BR}|}"
                                        + "should have {ExpectedItems.Count:1 element|{} elements}{BR}"
                                        + "but {ActualItems.Count:"
                                            + "was empty."
                                            + "|had 1 element: {0.ActualItems[0]}"
                                            + "|had {} elements: {0.ActualSample}"
                                        + "}"
                    }.ToString();

            object expectedValue, actualValue;
            int differenceIndex = FindDifference(expectedList, actualList, Compare.ObjectsAreEqual, out expectedValue, out actualValue);

            return new CollectionFailureMessage
                {
                    ActualValue = actualValue,
                    ExpectedValue = expectedValue,
                    FailureIndex = differenceIndex,
                    UserMessage = message,
                    MessageTemplate = "{ExpectedExpression:"
                                        + "doesn't match {}. Differs"
                                        + "|differs}"
                                    + " at index {FailureIndex}.{BR}"
                                    + "should be {ExpectedValue}{BR}"
                                    + "but was   {ActualValue}"
                }.ToString();
        }

        public string ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null)
        {
            object expectedValue, actualValue;
            int differenceIndex = FindDifference(expected, actual, ReferenceEquals, out expectedValue, out actualValue);

            return new FailureMessage
                {
                    ActualValue = actualValue,
                    ExpectedValue = expectedValue,
                    FailureIndex = differenceIndex,
                    UserMessage = message,
                    MessageTemplate = "differs at index {FailureIndex}.{BR}"
                                    + "should be instance {ExpectedValue}{BR}"
                                    + "but was            {ActualValue}"
                }.ToString();
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
            return new FunctionFailureMessage(function)
                {
                    ExpectedExceptionType = expectedExceptionType,
                    UserMessage = message,
                    MessageTemplate = "should throw {ExpectedExceptionName}{BR}"
                                    + "but didn't throw at all."
                }.ToString();
        }

        public string WrongException(Type expectedExceptionType, Type actualExceptionType, LambdaExpression function, string message = null)
        {
            return new FunctionFailureMessage(function)
                {
                    ExpectedExceptionType = expectedExceptionType,
                    ActualExceptionType = actualExceptionType,
                    UserMessage = message,
                    MessageTemplate = "should throw {ExpectedExceptionName}{BR}"
                                    + "but threw    {ActualExceptionName}"
                }.ToString();
        }

        public string DoesNotContain(string expectedSubstring, string actual, string message = null)
        {
            return new StringFailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expectedSubstring,
                    UserMessage = message,
                    MessageTemplate = "should contain {Expected:{Value}}{BR}"
                                    + "but was        {ActualValue}"
                }.ToString();
        }

        public string Contains(string expectedToNotContain, string actual, string message = null)
        {
            int matchIndex = actual.IndexOf(expectedToNotContain, StringComparison.Ordinal);

            return new StringFailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expectedToNotContain,
                    FailureIndex = matchIndex,
                    UserMessage = message,
                    MessageTemplate = "shouldn't contain {Expected:{Value}}{BR}"
                                    + "but was           {ActualValue}{BR}"
                                    + "                  {Arrow}{BR}"
                                    + "Match at index {FailureIndex}."
                }.ToString();
        }

        public string DoesNotStartWith(string expectedStart, string actual, string message = null)
        {
            return new StringFailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expectedStart,
                    UserMessage = message,
                    MessageTemplate = "should start with {Expected:{Value}}{BR}"
                                    + "but starts with   {ActualValue}"
                }.ToString();
        }

        public string DoesNotEndWith(string expectedEnd, string actual, string message = null)
        {
            return new StringFailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expectedEnd,
                    UserMessage = message,
                    FailureIndex = actual.Length - 1,
                    MessageTemplate = "should end with {Expected:{Value}}{BR}"
                                    + "but ends with   {ActualValue}"
                }.ToString();
        }
    }
}
