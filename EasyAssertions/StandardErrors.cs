using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static EasyAssertions.MessageHelper;

namespace EasyAssertions
{
    class StandardErrors : IStandardErrors
    {
        private static IStandardErrors current;
        public static IStandardErrors Current => current ?? new StandardErrors(StandardTests.Instance, ErrorFactory.Instance);

        private readonly StandardTests test;
        private readonly ErrorFactory error;

        private StandardErrors(StandardTests standardTests, ErrorFactory errorFactory)
        {
            test = standardTests;
            error = errorFactory;
        }

        public static void Override(IStandardErrors newStandardErrors)
        {
            current = newStandardErrors;
        }

        public static void Default()
        {
            current = null;
        }

        public Exception NotEqual(object expected, object actual, string message = null)
        {
            return error.WithActualExpression($@"
should be {Expected(expected, @"
          ")}
but was   {Value(actual)}" + message.OnNewLine());
        }

        public Exception NotEqual(string expected, string actual, Case caseSensitivity = Case.Sensitive, string message = null)
        {
            int differenceIndex = Enumerable.Range(0, Math.Max(expected.Length, actual.Length))
                .FirstOrDefault(i => i == actual.Length || i == expected.Length || !CharactersMatch(expected, actual, i, caseSensitivity));

            return error.WithActualExpression($@"
should be {Expected(expected, actual, differenceIndex, @"
          ")}
but was   {Value(actual, expected, differenceIndex)}
          {Arrow(actual, expected, differenceIndex)}
Difference at index {differenceIndex}." + message.OnNewLine());
        }

        private static bool CharactersMatch(string expected, string actual, int index, Case caseSensitivity)
        {
            StringComparison stringComparison = caseSensitivity == Case.Sensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return expected.Substring(index, 1)
                .Equals(actual.Substring(index, 1), stringComparison);
        }

        public Exception AreEqual(object notExpected, object actual, string message = null)
        {
            return error.WithActualExpression($@"
should not be {Expected(notExpected, @"
              ")}
but was       {Value(actual)}" + message.OnNewLine());
        }

        public Exception IsNull(string message = null)
        {
            return error.WithActualExpression(@"
should not be null, but was." + message.OnNewLine());
        }

        public Exception NotSame(object expected, object actual, string message = null)
        {
            return error.WithActualExpression($@"
should be instance {Expected(expected, @"
                   ")}
but was            {Value(actual)}" + message.OnNewLine());
        }

        public Exception AreSame(object actual, string message = null)
        {
            return error.WithActualExpression($@"
shouldn't be instance {Expected(actual, @"
                      ")}" + message.OnNewLine());
        }


        public Exception NotEmpty(IEnumerable actual, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();
            return error.WithActualExpression($@"
should be empty
but {Count(actualList,
                $"had 1 element: {Single(actualList)}",
                $"had {actualList.Count} elements: {Sample(actualList)}")}" + message.OnNewLine());
        }

        public Exception NotEmpty(string actual, string message = null)
        {
            return error.WithActualExpression($@"
should be empty
but was {Value(actual)}" + message.OnNewLine());
        }

        public Exception IsEmpty(string message = null)
        {
            return error.WithActualExpression(@"should not be empty, but was." + message.OnNewLine());
        }

        public Exception LengthMismatch(int expectedLength, IEnumerable actual, string message = null)
        {
            List<object> actualItems = actual.Cast<object>().ToList();
            return error.WithActualExpression($@"
should have {expectedLength} {Count(expectedLength, "element", "elements")}
but {Count(actualItems,
                    "was empty.",
                    $"had 1 element: {Single(actualItems)}",
                    $"had {actualItems.Count} elements: {Sample(actualItems)}")}"
                + message.OnNewLine());
        }

        public Exception DoesNotContain(object expected, IEnumerable actual, string itemType = null, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();
            return error.WithActualExpression($@"
should contain {itemType.WithSpace()}{Expected(expected, @"
               " + SpaceFor(itemType.WithSpace()))}
but was {Count(actualList,
                    "empty.",
                    $"      [{Single(actualList)}]", Sample(actualList))}"
                + message.OnNewLine());
        }

        public Exception DoesNotContainItems<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            List<TExpected> expectedItems = expected.ToList();
            List<TActual> actualItems = actual.ToList();
            FindDifferences(expectedItems, actualItems, predicate, out List<TExpected> missingExpected, out _, out List<int> missingIndices);

            return DoesNotContainItems(expectedItems, actualItems, missingExpected, missingIndices, message);
        }

        private Exception DoesNotContainItems<TActual, TExpected>(List<TExpected> expectedItems, List<TActual> actualItems, List<TExpected> missingExpected, List<int> missingIndices, string message)
        {
            string expectedSource = ExpectedSourceIfDifferentToValue(expectedItems);
            return error.Custom(expectedSource == null
                ? $@"{ActualExpression}
should contain expected item {missingIndices[0]} {Value(missingExpected[0])}
but was {Sample(actualItems)}" + message.OnNewLine()
                : $@"{ActualExpression}
should contain {expectedSource}
but was missing item {missingIndices[0]} {Value(missingExpected[0])}
and was {Sample(actualItems)}" + message.OnNewLine());
        }

        public Exception Contains(object expectedToNotContain, IEnumerable actual, string itemType = null, string message = null)
        {
            List<object> actualList = actual.Cast<object>().ToList();
            return error.WithActualExpression($@"
shouldn't contain {itemType.WithSpace()}{Expected(expectedToNotContain, @"
                  " + SpaceFor(itemType.WithSpace()))}
but was {Sample(actualList)}" + message.OnNewLine());
        }

        public Exception Contains(IEnumerable expectedToNotContain, IEnumerable actual, string message = null)
        {
            List<object> actualItems = actual.Cast<object>().ToList();
            HashSet<object> actualSet = new HashSet<object>(actualItems);
            List<object> unexpectedList = expectedToNotContain.Cast<object>().ToList();
            int unexpectedItemIndex = unexpectedList.FindIndex(actualSet.Contains);
            object unexpectedItem = unexpectedItemIndex == -1 ? null
                : unexpectedList[unexpectedItemIndex];
            int itemIndexInActual = actualItems.IndexOf(unexpectedItem);

            string expectedSource = ExpectedSourceIfDifferentToValue(expectedToNotContain);
            return error.Custom(expectedSource != null
                ? $@"{ActualExpression}
shouldn't contain {expectedSource}
but contained {Value(unexpectedItem)}
and was {Sample(actualItems)}
Match at index {itemIndexInActual}." + message.OnNewLine()
                : $@"{ActualExpression}
shouldn't contain {Value(unexpectedItem)}
but was {Sample(actualItems)}
Match at index {itemIndexInActual}." + message.OnNewLine());
        }

        public Exception OnlyContains(IEnumerable expected, IEnumerable actual, string message = null)
        {
            List<object> actualItems = new List<object>(actual.Cast<object>());
            List<object> expectedItems = expected.Cast<object>().ToList();

            string expectedSource = ExpectedSourceIfDifferentToValue(expected);
            return error.Custom(expectedSource != null
                ? $@"{ActualExpression}
should contain more than just {expectedSource}
but was {Sample(actualItems)}" + message.OnNewLine()
                : $@"{ActualExpression}
should contain more than just {Sample(expectedItems)}
but was {Sample(actualItems)}" + message.OnNewLine());
        }

        public Exception DoesNotOnlyContain<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            List<TExpected> expectedItems = expected.ToList();
            List<TActual> actualItems = actual.ToList();

            if (expectedItems.None())
                return NotEmpty(actualItems, message);

            FindDifferences(expectedItems, actualItems, predicate, out List<TExpected> missingExpected, out List<TActual> extraActual, out List<int> missingIndices);

            if (missingExpected.Any())
                return DoesNotContainItems(expectedItems, actualItems, missingExpected, missingIndices, message);
            return ContainsExtraItem(expectedItems, extraActual, message);
        }

        public Exception ContainsExtraItem<TActual, TExpected>(IEnumerable<TExpected> expectedSuperset, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            List<TExpected> expectedItems = expectedSuperset.ToList();
            FindDifferences(expectedItems, actual, predicate, out _, out List<TActual> extraActual, out _);

            return ContainsExtraItem(expectedItems, extraActual, message);
        }

        private Exception ContainsExtraItem<TActual, TExpected>(List<TExpected> expectedItems, List<TActual> extraActual, string message)
        {
            string expectedSource = ExpectedSourceIfDifferentToValue(expectedItems);
            return error.Custom(expectedSource != null
                ? $@"{ActualExpression}
should only contain {expectedSource}
but also contains {Sample(extraActual)}" + message.OnNewLine()
                : $@"{ActualExpression}
should only contain {Sample(expectedItems)}
but also contains {Sample(extraActual)}" + message.OnNewLine());
        }

        private static void FindDifferences<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, out List<TExpected> missingExpected, out List<TActual> extraActual, out List<int> missingIndices)
        {
            missingExpected = new List<TExpected>();
            extraActual = actual.ToList();
            missingIndices = new List<int>();

            int expectedIndex = 0;
            foreach (TExpected expectedItem in expected)
            {
                int matchingIndex = extraActual.FindIndex(a => predicate(a, expectedItem));
                if (matchingIndex == -1)
                {
                    missingIndices.Add(expectedIndex);
                    missingExpected.Add(expectedItem);
                }
                else
                {
                    extraActual.RemoveAt(matchingIndex);
                }
                expectedIndex++;
            }
        }

        public Exception ContainsDuplicate(IEnumerable actual, string message = null)
        {
            List<object> actualItems = actual.Cast<object>().ToList();
            object duplicateItem = actualItems.GroupBy(i => i).First(g => g.Count() > 1).Key;
            List<int> duplicateIndices = new List<int>();
            for (int i = 0; i < actualItems.Count; i++)
                if (test.ObjectsAreEqual(actualItems[i], duplicateItem))
                    duplicateIndices.Add(i);
            return error.WithActualExpression($@"
should not contain duplicates
but {Value(duplicateItem)}
was found at indices {duplicateIndices.Take(duplicateIndices.Count - 1).Join(", ")} and {duplicateIndices.Last()}."
                + message.OnNewLine());
        }

        public Exception DoNotMatch<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            List<object> expectedList = expected.Cast<object>().ToList();
            List<object> actualList = actual.Cast<object>().ToList();

            string expectedSource = ExpectedSourceIfDifferentToValue(expected);

            if (expectedList.Count != actualList.Count)
                return error.Custom($@"{ActualExpression}{(expectedSource == null ? null : $"doesn't match {expectedSource}.").OnNewLine()}
should have {Count(expectedList, "1 element", $"{expectedList.Count} elements")}
but {Count(actualList,
                        "was empty.",
                        $"had 1 element: {Single(actualList)}",
                        $"had {actualList.Count} elements: {Sample(actualList)}")}"
                    + message.OnNewLine());

            int differenceIndex = FindDifference(expectedList, actualList, (a, e) => predicate((TActual)a, (TExpected)e), out object expectedItem, out object actualItem);

            return error.WithActualExpression($@"
{(expectedSource != null ? $"doesn't match {expectedSource}. Differs" : "differs")} at index {differenceIndex}.
should match {Value(expectedItem)}
but was      {Value(actualItem)}" + message.OnNewLine());
        }

        public Exception ItemsNotSame(IEnumerable expected, IEnumerable actual, string message = null)
        {
            int differenceIndex = FindDifference(expected, actual, ReferenceEquals, out object expectedItem, out object actualItem);
            return error.WithActualExpression($@"
differs at index {differenceIndex}.
should be instance {Value(expectedItem)}
but was            {Value(actualItem)}" + message.OnNewLine());
        }

        public Exception DoesNotStartWith<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            List<object> expectedItems = expected.Cast<object>().ToList();
            List<object> actualItems = actual.Cast<object>().ToList();

            if (actualItems.Count < expectedItems.Count)
                return NotLongEnough(expectedItems, actualItems, message);

            int differenceIndex = FindDifference(expectedItems, actualItems, (a, e) => predicate((TActual)a, (TExpected)e), out object expectedValue, out object actualValue);

            return DiffersAtIndex(differenceIndex, expectedValue, actualValue, message);
        }

        public Exception DoesNotEndWith<TActual, TExpected>(IEnumerable<TExpected> expected, IEnumerable<TActual> actual, Func<TActual, TExpected, bool> predicate, string message = null)
        {
            List<object> expectedItems = expected.Cast<object>().ToList();
            List<object> actualItems = actual.Cast<object>().ToList();

            if (actualItems.Count < expectedItems.Count)
                return NotLongEnough(expectedItems, actualItems, message);

            int differenceInLength = actualItems.Count - expectedItems.Count;
            int differenceIndex = FindDifference(expectedItems, actualItems.Skip(differenceInLength), (a, e) => predicate((TActual)a, (TExpected)e), out object expectedValue, out object actualValue)
                + differenceInLength;

            return DiffersAtIndex(differenceIndex, expectedValue, actualValue, message);
        }

        private Exception NotLongEnough(List<object> expectedItems, List<object> actualItems, string message)
        {
            return error.WithActualExpression($@"
should have at least {expectedItems.Count} {Count(expectedItems, "element", "elements")}
but {Count(actualItems,
                "was empty.",
                $"had 1 element: {Single(actualItems)}",
                $"had {actualItems.Count} elements: {Sample(actualItems)}")}" + message.OnNewLine());
        }

        private Exception DiffersAtIndex(int differenceIndex, object expectedValue, object actualValue, string message)
        {
            return error.WithActualExpression($@"
differs at index {differenceIndex}.
should be {Value(expectedValue)}
but was   {Value(actualValue)}" + message.OnNewLine());
        }

        public Exception TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message = null)
        {
            return error.Custom(TreesDoNotMatch(expected, actual, getChildren, predicate, message, Enumerable.Empty<TActual>()));
        }

        private string TreesDoNotMatch<TActual, TExpected>(IEnumerable<TestNode<TExpected>> expected, IEnumerable<TActual> actual, Func<TActual, IEnumerable<TActual>> getChildren, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            if (test.CollectionsMatch(actual, expected.Values(), predicate))
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
            string expectedSource = ExpectedSourceIfDifferentToValue(expectedItems);

            return $@"{ActualExpression}{$"doesn't match {expectedSource}.".OnNewLine()}
{Path(pathItems)} node should have {Count(expectedItems, "1 child", $"{expectedItems.Count} children")}
but {Count(actualItems,
                "was empty.",
                $"had 1 child: {Single(actualItems)}",
                $"had {actualItems.Count} children: {Sample(actualItems)}")}" + message.OnNewLine();
        }

        private static string TreeNodeValueDoesNotMatch<TActual>(IList<object> expectedItems, IList<object> actualItems, Func<object, object, bool> predicate, string message, IEnumerable<TActual> path)
        {
            int differenceIndex = FindDifference(expectedItems, actualItems, predicate, out object expectedItem, out object actualItem);
            List<object> pathItems = path.Cast<object>().ToList();
            string expectedSource = ExpectedSourceIfDifferentToValue(expectedItem);

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

        public Exception NoException(Type expectedExceptionType, LambdaExpression function = null, string message = null)
        {
            return error.Custom($@"{(function != null ? Value(function) : ActualExpression)}
should throw {Value(expectedExceptionType)}
but didn't throw at all." + message.OnNewLine());
        }

        public Exception WrongException(Type expectedExceptionType, Exception actualException, LambdaExpression function = null, string message = null)
        {
            return error.Custom($@"{(function != null ? Value(function) : ActualExpression)}
should throw {Value(expectedExceptionType)}
but threw    {Value(actualException.GetType())}" + message.OnNewLine(), actualException);
        }

        public Exception DoesNotContain(string expectedSubstring, string actual, string message = null)
        {
            return error.WithActualExpression($@"
should contain {Expected(expectedSubstring, @"
               ")}
but was        {Value(actual)}" + message.OnNewLine());
        }

        public Exception Contains(string expectedToNotContain, string actual, string message = null)
        {
            int matchIndex = actual.IndexOf(expectedToNotContain, StringComparison.Ordinal);

            return error.WithActualExpression($@"
shouldn't contain {Expected(expectedToNotContain, @"
                  ")}
but was           {Value(actual, expectedToNotContain, matchIndex)}
                  {Arrow(actual, expectedToNotContain, matchIndex)}
Match at index {matchIndex}." + message.OnNewLine());
        }

        public Exception DoesNotStartWith(string expectedStart, string actual, string message = null)
        {
            return error.WithActualExpression($@"
should start with {Expected(expectedStart, @"
                  ")}
but starts with   {Value(actual)}" + message.OnNewLine());
        }

        public Exception DoesNotEndWith(string expectedEnd, string actual, string message = null)
        {
            return error.WithActualExpression($@"
should end with {Expected(expectedEnd, actual, expectedEnd.Length - 1, @"
                ")}
but ends with   {Value(actual, expectedEnd, actual.Length - 1)}" + message.OnNewLine());
        }

        public Exception NotGreaterThan(object expected, object actual, string message = null)
        {
            return error.WithActualExpression($@"
should be greater than {Expected(expected, @"
                       ")}
but was                {Value(actual)}" + message.OnNewLine());
        }

        public Exception NotLessThan(object expected, object actual, string message = null)
        {
            return error.WithActualExpression($@"
should be less than {Expected(expected, @"
                    ")}
but was             {Value(actual)}" + message.OnNewLine());
        }

        public Exception DoesNotMatch(Regex regex, string actual, string message = null)
        {
            return error.WithActualExpression($@"
should match {Expected(regex, @"
             ")}
but was {Value(actual)}" + message.OnNewLine());
        }

        public Exception Matches(Regex regex, string actual, string message = null)
        {
            return error.WithActualExpression($@"
shouldn't match {Expected(regex, @"
                ")}
but was {Value(actual)}" + message.OnNewLine());
        }

        public Exception TaskTimedOut(TimeSpan timeout, string message = null)
        {
            return error.WithActualExpression($@"timed out after {Value(timeout)}." + message.OnNewLine());
        }

        public Exception Matches(IEnumerable notExpected, IEnumerable actual, string message = null)
        {
            List<object> actualItems = actual.Cast<object>().ToList();

            string expectedSource = ExpectedSourceIfDifferentToValue(notExpected);

            return error.WithActualExpression(expectedSource == null
                ? $@"
should not match {Sample(actualItems)}
but did." + message.OnNewLine()
                : $@"
should not match {expectedSource}
but was {Sample(actualItems)}" + message.OnNewLine());
        }
    }
}
