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
                    MessageTemplate = "should be {Expected}{BR}"
                                    + "but was   {ActualValue}"
                }.ToString();
        }

        public string NotEqual(string expected, string actual, Case caseSensitivity = Case.Sensitive, string message = null)
        {
            int differenceIndex = Enumerable.Range(0, Math.Max(expected.Length, actual.Length))
                .FirstOrDefault(i => i == actual.Length || i == expected.Length || !CharactersMatch(expected, actual, i, caseSensitivity));

            return new StringFailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expected,
                    FailureIndex = differenceIndex,
                    UserMessage = message,
                    MessageTemplate = "should be {Expected}{BR}"
                                    + "but was   {ActualValue}{BR}"
                                    + "          {Arrow}{BR}"
                                    + "Difference at index {FailureIndex}."
                }.ToString();
        }

        private static bool CharactersMatch(string expected, string actual, int index, Case caseSensitivity)
        {
            StringComparison stringComparison = caseSensitivity == Case.Sensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return expected.Substring(index, 1)
                .Equals(actual.Substring(index, 1), stringComparison);
        }

        public string AreEqual(object notExpected, object actual, string message = null)
        {
            return new FailureMessage
                {
                    ExpectedValue = notExpected,
                    ActualValue = actual,
                    UserMessage = message,
                    MessageTemplate = "should not be {Expected}{BR}"
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
                    MessageTemplate = "should be instance {Expected}{BR}"
                                    + "but was            {ActualValue}"
                }.ToString();
        }

        public string AreSame(object actual, string message = null)
        {
            return new FailureMessage
                {
                    ExpectedValue = actual,
                    UserMessage = message,
                    MessageTemplate = "shouldn't be instance {Expected}"
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

        public string DoesNotContain(object expected, IEnumerable actual, string itemType = null, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return new CollectionFailureMessage
                {
                    ActualItems = actualList,
                    ExpectedValue = expected,
                    ItemType = itemType,
                    UserMessage = message,
                    MessageTemplate = "should contain {ItemType:{} |}{Expected}{BR}"
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

        public string Contains(object expectedToNotContain, IEnumerable actual, string itemType = null, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return new CollectionFailureMessage
                {
                    ActualItems = actualList,
                    ExpectedValue = expectedToNotContain,
                    ItemType = itemType,
                    UserMessage = message,
                    MessageTemplate = "shouldn't contain {ItemType:{} |}{Expected}{BR}"
                                    + "but was {ActualSample}"
                }.ToString();
        }

        public string Contains(IEnumerable expectedToNotContain, IEnumerable actual, string message = null)
        {
            HashSet<object> actualSet = new HashSet<object>(actual.Cast<object>());
            List<object> unexpectedList = expectedToNotContain.Cast<object>().ToList();
            int unexpectedItemIndex = unexpectedList.FindIndex(actualSet.Contains);
            object unexpectedItem = unexpectedItemIndex == -1 ? null
                : unexpectedList[unexpectedItemIndex];
            int itemIndexInActual = actual.Cast<object>().ToList().IndexOf(unexpectedItem);

            return new CollectionFailureMessage
                {
                    ActualItems = actualSet,
                    ExpectedValue = unexpectedItem,
                    FailureIndex = itemIndexInActual,
                    UserMessage = message,
                    MessageTemplate = "shouldn't contain {ExpectedExpression:{0:"
                                        + "{ExpectedExpression}{BR}"
                                        + "but contained {ExpectedValue}{BR}"
                                        + "and was {ActualSample}"
                                    + "}|{0:"
                                        + "{ExpectedValue}{BR}"
                                        + "but was {ActualSample}"
                                    + "}}{BR}"
                                    + "Match at index {FailureIndex}."
                }.ToString();
        }

        public string OnlyContains(IEnumerable expected, IEnumerable actual, string message = null)
        {
            List<object> actualItems = new List<object>(actual.Cast<object>());
            HashSet<object> expectedItems = new HashSet<object>(expected.Cast<object>());


            return new CollectionFailureMessage
                {
                    ActualItems = actualItems,
                    ExpectedItems = expectedItems,
                    UserMessage = message,
                    MessageTemplate = "{ExpectedExpression:{0:"
                                        + "should contain more than just {ExpectedExpression}{BR}"
                                        + "but was {ActualSample}"
                                    + "}|{0:"
                                        + "should contain more than just {ExpectedSample}{BR}"
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

            return ContainsExtraItem(expected, actual, message);
        }

        public string ContainsExtraItem(IEnumerable expected, IEnumerable actual, string message = null)
        {
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

        public string DoNotMatch<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
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
            int differenceIndex = FindDifference(expectedList, actualList, (a, e) => predicate((TActual)a, (TExpected)e), out expectedValue, out actualValue);

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
                                    + "should match {ExpectedValue}{BR}"
                                    + "but was      {ActualValue}"
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

        public string TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message = null)
        {
            return TreesDoNotMatch(expected, actual, getChildren, predicate, message, Enumerable.Empty<TActual>());
        }

        private static string TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            if (Compare.CollectionsMatch(actual, expected.Values(), predicate))
                return ChildrenDoNotMatch(expected, actual, getChildren, predicate, message, path);

            List<object> actualItems = actual.Cast<object>().ToList();
            List<object> expectedItems = expected.Values().Cast<object>().ToList();

            return actualItems.Count != expectedItems.Count
                ? TreeNodeChildrenLengthMismatch(expectedItems, actualItems, message, path)
                : TreeNodeValueDoesNotMatch(expectedItems, actualItems, predicate, message, path);
        }

        private static string TreeNodeChildrenLengthMismatch<TActual>(List<object> expectedItems, List<object> actualItems, string message, IEnumerable<TActual> path)
        {
            return new TreeFailureMessage
                {
                    ActualItems = actualItems,
                    ExpectedItems = expectedItems,
                    FailurePathValues = path.Cast<object>().ToList(),
                    UserMessage = message,
                    MessageTemplate = "{ExpectedExpression:doesn't match {}.{0.BR}|}"
                                      + "{FailurePath} node should have {ExpectedItems.Count:1 child|{} children}{BR}"
                                      + "but {ActualItems.Count:"
                                      + "was empty."
                                      + "|had 1 child: {0.ActualItems[0]}"
                                      + "|had {} children: {0.ActualSample}"
                                      + "}"
                }.ToString();
        }

        private static string TreeNodeValueDoesNotMatch<TActual>(IList<object> expectedItems, IList<object> actualItems, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            object expectedValue;
            object actualValue;
            int differenceIndex = FindDifference(expectedItems, actualItems, predicate, out expectedValue, out actualValue);

            return new TreeFailureMessage
                {
                    ActualValue = actualValue,
                    ExpectedValue = expectedValue,
                    FailureIndex = differenceIndex,
                    FailurePathValues = path.Cast<object>().ToList(),
                    UserMessage = message,
                    MessageTemplate = "doesn't match {ExpectedExpression:{0:{ExpectedExpression}}|expected tree}.{BR}"
                                      + "Differs at {FailurePath}, child index {FailureIndex}.{BR}"
                                      + "should be {ExpectedValue}{BR}"
                                      + "but was   {ActualValue}"
                }.ToString();
        }

        private static string ChildrenDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            return expected.Zip(actual, (e, a) => TreesDoNotMatch(e, getChildren(a), getChildren, predicate, message, path.Concat(new[] { a })))
                .FirstOrDefault(m => m != null);
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
                    MessageTemplate = "should contain {Expected}{BR}"
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
                    MessageTemplate = "shouldn't contain {Expected}{BR}"
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
                    MessageTemplate = "should start with {Expected}{BR}"
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
                    MessageTemplate = "should end with {Expected}{BR}"
                                    + "but ends with   {ActualValue}"
                }.ToString();
        }

        public string NotGreaterThan(object expected, object actual, string message = null)
        {
            return new FailureMessage
                {
                    ActualValue = actual,
                    ExpectedValue = expected,
                    UserMessage = message,
                    MessageTemplate = "should be greater than {Expected}{BR}"
                                    + "but was                {ActualValue}"
                }.ToString();
        }

        public string NotLessThan(object expected, object actual, string message = null)
        {
            return new FailureMessage
                {
                    ExpectedValue = expected,
                    ActualValue = actual,
                    UserMessage = message,
                    MessageTemplate = "should be less than {Expected}{BR}"
                                    + "but was             {ActualValue}"
                }.ToString();
        }
    }
}
