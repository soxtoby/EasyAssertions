using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static EasyAssertions.FailureMessageHelper;

namespace EasyAssertions
{
    internal class DefaultFailureMessageFormatter : IFailureMessageFormatter
    {
        private DefaultFailureMessageFormatter() { }

        public static readonly DefaultFailureMessageFormatter Instance = new DefaultFailureMessageFormatter();

        public string NotEqual(object expected, object actual, string message = null)
        {
            return $@"{ActualExpression}
should be {Expected(expected, @"
          ")}
but was   {Value(actual)}" + message.OnNewLine();
        }

        public string NotEqual(string expected, string actual, Case caseSensitivity = Case.Sensitive, string message = null)
        {
            int differenceIndex = Enumerable.Range(0, Math.Max(expected.Length, actual.Length))
                .FirstOrDefault(i => i == actual.Length || i == expected.Length || !CharactersMatch(expected, actual, i, caseSensitivity));

            return $@"{ActualExpression}
should be {Expected(expected, differenceIndex, @"
          ")}
but was   {Value(actual, differenceIndex)}
          {Arrow(actual, differenceIndex)}
Difference at index {differenceIndex}." + message.OnNewLine();
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
            return $@"{ActualExpression}
should not be {Expected(notExpected, @"
              ")}
but was       {Value(actual)}" + message.OnNewLine();
        }

        public string IsNull(string message = null)
        {
            return $@"{ActualExpression}
should not be null, but was." + message.OnNewLine();
        }

        public string NotSame(object expected, object actual, string message = null)
        {
            return $@"{ActualExpression}
should be instance {Expected(expected, @"
                   ")}
but was            {Value(actual)}" + message.OnNewLine();
        }

        public string AreSame(object actual, string message = null)
        {
            return $@"{ActualExpression}
shouldn't be instance {Expected(actual, @"
                      ")}" + message.OnNewLine();
        }

        public string NotEmpty(IEnumerable actual, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return $@"{ActualExpression}
should be empty
but {Count(actualList,
                       $"had 1 element: {Single(actualList)}",
                       $"had {actualList.Count} elements: {Sample(actualList)}")}" + message.OnNewLine();
        }

        public string NotEmpty(string actual, string message = null)
        {
            return $@"{ActualExpression}
should be empty
but was {Value(actual)}" + message.OnNewLine();
        }

        public string IsEmpty(string message = null)
        {
            return $@"{ActualExpression}
should not be empty, but was." + message.OnNewLine();
        }

        public string LengthMismatch(int expectedLength, IEnumerable actual, string message = null)
        {
            List<object> actualItems = actual.Cast<object>().ToList();
            return $@"{ActualExpression}
should have {expectedLength} {Count(expectedLength, "element", "elements")}
but {Count(actualItems,
                       "was empty.",
                       $"had 1 element: {Single(actualItems)}",
                       $"had {actualItems.Count} elements: {Sample(actualItems)}")}"
                   + message.OnNewLine();
        }

        public string DoesNotContain(object expected, IEnumerable actual, string itemType = null, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return $@"{ActualExpression}
should contain {itemType.WithSpace()}{Expected(expected, @"
               " + SpaceFor(itemType.WithSpace()))}
but was {Count(actualList,
                       "empty.",
                       $"      [{Single(actualList)}]", Sample(actualList))}"
                   + message.OnNewLine();
        }

        public string DoesNotContainItems(IEnumerable expected, IEnumerable actual, string message = null)
        {
            HashSet<object> actualItems = new HashSet<object>(actual.Cast<object>());
            List<object> expectedItems = expected.Cast<object>().ToList();
            int missingItemIndex;
            object missingItem;
            FindMissingItem(expectedItems, actualItems, out missingItemIndex, out missingItem);

            string expectedSource = SourceExpectedDifferentToValue(expected);

            return expectedSource == null
                ? $@"{ActualExpression}
should contain expected item {missingItemIndex} {Value(missingItem)}
but was {Sample(actualItems)}" + message.OnNewLine()
                : $@"{ActualExpression}
should contain {expectedSource}
but was missing item {missingItemIndex} {Value(missingItem)}
and was {Sample(actualItems)}" + message.OnNewLine();
        }

        public string Contains(object expectedToNotContain, IEnumerable actual, string itemType = null, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();

            return $@"{ActualExpression}
shouldn't contain {itemType.WithSpace()}{Expected(expectedToNotContain, @"
                  " + SpaceFor(itemType.WithSpace()))}
but was {Sample(actualList)}" + message.OnNewLine();
        }

        public string Contains(IEnumerable expectedToNotContain, IEnumerable actual, string message = null)
        {
            List<object> actualItems = actual.Cast<object>().ToList();
            HashSet<object> actualSet = new HashSet<object>(actualItems);
            List<object> unexpectedList = expectedToNotContain.Cast<object>().ToList();
            int unexpectedItemIndex = unexpectedList.FindIndex(actualSet.Contains);
            object unexpectedItem = unexpectedItemIndex == -1 ? null
                : unexpectedList[unexpectedItemIndex];
            int itemIndexInActual = actual.Cast<object>().ToList().IndexOf(unexpectedItem);

            string expectedSource = SourceExpectedDifferentToValue(expectedToNotContain);

            return expectedSource != null
                ? $@"{ActualExpression}
shouldn't contain {expectedSource}
but contained {Value(unexpectedItem)}
and was {Sample(actualItems)}
Match at index {itemIndexInActual}." + message.OnNewLine()
                : $@"{ActualExpression}
shouldn't contain {Value(unexpectedItem)}
but was {Sample(actualItems)}
Match at index {itemIndexInActual}." + message.OnNewLine();
        }

        public string OnlyContains(IEnumerable expected, IEnumerable actual, string message = null)
        {
            List<object> actualItems = new List<object>(actual.Cast<object>());
            HashSet<object> expectedItems = new HashSet<object>(expected.Cast<object>());

            string expectedSource = SourceExpectedDifferentToValue(expected);

            return expectedSource != null
                ? $@"{ActualExpression}
should contain more than just {expectedSource}
but was {Sample(actualItems)}" + message.OnNewLine()
                : $@"{ActualExpression}
should contain more than just {Sample(expectedItems)}
but was {Sample(actualItems)}" + message.OnNewLine();
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

            string expectedSource = SourceExpectedDifferentToValue(expectedItems);

            return expectedSource != null
                ? $@"{ActualExpression}
should only contain {expectedSource}
but also contains {Sample(extraItems)}" + message.OnNewLine()
                : $@"{ActualExpression}
should only contain {Sample(expectedItems)}
but also contains {Sample(extraItems)}" + message.OnNewLine();
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

            string expectedSource = SourceExpectedDifferentToValue(expected);

            if (expectedList.Count != actualList.Count)
                return $@"{ActualExpression}{(expectedSource == null ? null : $"doesn't match {expectedSource}.").OnNewLine()}
should have {Count(expectedList, "1 element", $"{expectedList.Count} elements")}
but {Count(actualList,
                           "was empty.",
                           $"had 1 element: {Single(actualList)}",
                           $"had {actualList.Count} elements: {Sample(actualList)}")}"
                       + message.OnNewLine();

            object expectedItem, actualItem;
            int differenceIndex = FindDifference(expectedList, actualList, (a, e) => predicate((TActual)a, (TExpected)e), out expectedItem, out actualItem);

            return $@"{ActualExpression}
{(expectedSource != null ? $"doesn't match {expectedSource}. Differs" : "differs")} at index {differenceIndex}.
should match {Value(expectedItem)}
but was      {Value(actualItem)}" + message.OnNewLine();
        }

        public string ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null)
        {
            object expectedItem, actualItem;
            int differenceIndex = FindDifference(expected, actual, ReferenceEquals, out expectedItem, out actualItem);

            return $@"{ActualExpression}
differs at index {differenceIndex}.
should be instance {Value(expectedItem)}
but was            {Value(actualItem)}" + message.OnNewLine();
        }

        public string DoesNotStartWith(IEnumerable expected, IEnumerable actual, Func<object, object, bool> predicate, string message = null)
        {
            List<object> expectedItems = expected.Cast<object>().ToList();
            List<object> actualItems = actual.Cast<object>().ToList();

            if (actualItems.Count < expectedItems.Count)
            {
                return $@"{ActualExpression}
should have at least {expectedItems.Count} {Count(expectedItems, "element", "elements")}
but {Count(actualItems,
                    "was empty.",
                    $"had 1 element: {Single(actualItems)}",
                    $"had {actualItems.Count} elements: {Sample(actualItems)}")}" + message.OnNewLine();
            }

            object expectedValue, actualValue;
            int differenceIndex = FindDifference(expectedItems, actualItems, predicate, out expectedValue, out actualValue);

            return $@"{ActualExpression}
differs at index {differenceIndex}.
should be {Value(expectedValue)}
but was   {Value(actualValue)}" + message.OnNewLine();
        }

        public string TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message = null)
        {
            return TreesDoNotMatch(expected, actual, getChildren, predicate, message, Enumerable.Empty<TActual>());
        }

        private string TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            if (Compare.CollectionsMatch(actual, expected.Values(), predicate))
                return ChildrenDoNotMatch(expected, actual, getChildren, predicate, message, path);

            List<object> actualItems = actual.Cast<object>().ToList();
            List<object> expectedItems = expected.Values().Cast<object>().ToList();

            return actualItems.Count != expectedItems.Count
                ? TreeNodeChildrenLengthMismatch(expectedItems, actualItems, message, path)
                : TreeNodeValueDoesNotMatch(expectedItems, actualItems, predicate, message, path);
        }

        private static string TreeNodeChildrenLengthMismatch<TActual>(ICollection<object> expectedItems, ICollection<object> actualItems, string message, IEnumerable<TActual> path)
        {
            List<object> pathItems = path.Cast<object>().ToList();
            string expectedSource = SourceExpectedDifferentToValue(expectedItems);

            return $@"{ActualExpression}{(expectedItems == null ? null : $"doesn't match {expectedSource}.").OnNewLine()}
{Path(pathItems)} node should have {Count(expectedItems, "1 child", $"{expectedItems.Count} children")}
but {Count(actualItems,
                       "was empty.",
                       $"had 1 child: {Single(actualItems)}",
                       $"had {actualItems.Count} children: {Sample(actualItems)}")}" + message.OnNewLine();
        }

        private static string TreeNodeValueDoesNotMatch<TActual>(IList<object> expectedItems, IList<object> actualItems, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            object expectedItem;
            object actualItem;
            int differenceIndex = FindDifference(expectedItems, actualItems, predicate, out expectedItem, out actualItem);
            List<object> pathItems = path.Cast<object>().ToList();
            string expectedSource = SourceExpectedDifferentToValue(expectedItem);

            return $@"{ActualExpression}
doesn't match {expectedSource ?? "expected tree"}.
Differs at {Path(pathItems)}, child index {differenceIndex}.
should be {Value(expectedItem)}
but was   {Value(actualItem)}" + message.OnNewLine();
        }

        private string ChildrenDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
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
            return $@"{Value(function)}
should throw {Value(expectedExceptionType)}
but didn't throw at all." + message.OnNewLine();
        }

        public string WrongException(Type expectedExceptionType, Type actualExceptionType, LambdaExpression function, string message = null)
        {
            return $@"{Value(function)}
should throw {Value(expectedExceptionType)}
but threw    {Value(actualExceptionType)}" + message.OnNewLine();
        }

        public string DoesNotContain(string expectedSubstring, string actual, string message = null)
        {
            return $@"{ActualExpression}
should contain {Expected(expectedSubstring, @"
               ")}
but was        {Value(actual)}" + message.OnNewLine();
        }

        public string Contains(string expectedToNotContain, string actual, string message = null)
        {
            int matchIndex = actual.IndexOf(expectedToNotContain, StringComparison.Ordinal);

            return $@"{ActualExpression}
shouldn't contain {Expected(expectedToNotContain, @"
                  ")}
but was           {Value(actual, matchIndex)}
                  {Arrow(actual, matchIndex)}
Match at index {matchIndex}." + message.OnNewLine();
        }

        public string DoesNotStartWith(string expectedStart, string actual, string message = null)
        {
            return $@"{ActualExpression}
should start with {Expected(expectedStart, @"
                  ")}
but starts with   {Value(actual)}" + message.OnNewLine();
        }

        public string DoesNotEndWith(string expectedEnd, string actual, string message = null)
        {
            return $@"{ActualExpression}
should end with {Expected(expectedEnd, expectedEnd.Length - 1, @"
                ")}
but ends with   {Value(actual, actual.Length - 1)}" + message.OnNewLine();
        }

        public string NotGreaterThan(object expected, object actual, string message = null)
        {
            return $@"{ActualExpression}
should be greater than {Expected(expected, @"
                       ")}
but was                {Value(actual)}" + message.OnNewLine();
        }

        public string NotLessThan(object expected, object actual, string message = null)
        {
            return $@"{ActualExpression}
should be less than {Expected(expected, @"
                    ")}
but was             {Value(actual)}" + message.OnNewLine();
        }

        public string DoesNotMatch(Regex regex, string actual, string message = null)
        {
            return $@"{ActualExpression}
should match {Expected(regex, @"
             ")}
but was {Value(actual)}" + message.OnNewLine();
        }

        public string Matches(Regex regex, string actual, string message = null)
        {
            return $@"{ActualExpression}
shouldn't match {Expected(regex, @"
                ")}
but was {Value(actual)}" + message.OnNewLine();
        }
    }
}
