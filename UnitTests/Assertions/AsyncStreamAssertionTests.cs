using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class AsyncStreamAssertionTests : AssertionTests
    {
        Func<Task, TimeSpan, bool> defaultWaitForTask = null!;
        Func<Task, TimeSpan, bool> wait = null!;
        IAsyncEnumerable<int> stream = null!;
        Func<int> getStreamResult = null!;

        [SetUp]
        public void SetUp()
        {
            defaultWaitForTask = TaskAssertions.WaitForTask;
            wait = Substitute.For<Func<Task, TimeSpan, bool>>();
            TaskAssertions.WaitForTask = wait;
            stream = TestStream();
            getStreamResult = Substitute.For<Func<int>>();
        }

        [TearDown]
        public void TearDown()
        {
            TaskAssertions.WaitForTask = defaultWaitForTask;
        }

        [Test]
        public void ShouldCompleteWithValue_CompletesWithinOneSecond_ReturnsResult()
        {
            AssertReturnsResult(1, TimeSpan.FromSeconds(1), () => stream.ShouldComplete());
        }

        [Test]
        public void ShouldCompleteWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => stream.ShouldComplete(msg));
        }

        [Test]
        public void ShouldCompleteWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = stream;

            AssertTimesOut(TimeSpan.FromSeconds(1), msg => actualExpression.ShouldComplete(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_CompletesWithinTimeout_ReturnsResult()
        {
            AssertReturnsResult(1, TimeSpan.FromMilliseconds(1), () => stream.ShouldComplete(1));
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => stream.ShouldComplete(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => stream.ShouldComplete(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = stream;
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => actualExpression.ShouldComplete(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CompletesWithinTimeout_ReturnsResult()
        {
            AssertReturnsResult(1, TimeSpan.FromMilliseconds(1), () => stream.ShouldComplete(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => stream.ShouldComplete(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = stream;
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => actualExpression.ShouldComplete(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFail_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromSeconds(1), () => stream.ShouldFail());
        }

        [Test]
        public void ShouldFail_TaskCancelled_ReturnsException()
        { }

        [Test]
        public void ShouldFail_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => stream.ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => stream.ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_CorrectlyRegistersAssertion()
        {
            var actualExpression = stream;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinMilliseconds_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => stream.ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_TaskCancelled_ReturnsException()
        { }

        [Test]
        public void ShouldFailWithinMilliseconds_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => stream.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => stream.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            var actualExpression = stream;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinTimeSpan_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => stream.ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TaskCancelled_ReturnsException()
        { }

        [Test]
        public void ShouldFailWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => stream.ShouldFail(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => stream.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => stream.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            var actualExpression = stream;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        void AssertReturnsResult(int expectedResult, TimeSpan timeout, Func<Actual<IReadOnlyList<int>>> callAssertion)
        {
            TaskReturns(expectedResult, timeout);

            var result = callAssertion();

            CollectionAssert.AreEqual(new[] { expectedResult }, result.And);
        }

        void AssertReturnsException<TException>(TException expectedException, TimeSpan timeout, Func<ActualException<TException>> callAssertion) where TException : Exception
        {
            TaskFails(expectedException, timeout);

            var result = callAssertion();

            Assert.AreSame(expectedException, result.And);
        }

        void AssertTimesOut(TimeSpan timeout, Action<string> callAssertion)
        {
            Error.TaskTimedOut(timeout, "foo").Returns(ExpectedException);
            TaskTimesOut(timeout);

            var result = Assert.Throws<Exception>(() => callAssertion("foo"));

            Assert.AreSame(ExpectedException, result);
        }

        void AssertFailsWithNoExceptionMessage<TException>(Func<string, ActualException<TException>> callAssertion) where TException : Exception
        {
            Error.NoException(typeof(TException), message: "foo").Returns(ExpectedException);
            TaskReturns(0);

            var result = Assert.Throws<Exception>(() => callAssertion("foo"));

            Assert.AreSame(ExpectedException, result);
        }

        void TaskReturns(int expectedValue, TimeSpan? timeout = null)
        {
            wait(Arg.Any<Task>(), timeout ?? Arg.Any<TimeSpan>()).Returns(true);
            getStreamResult().Returns(expectedValue);
        }

        void TaskFails(Exception expectedException, TimeSpan timeout)
        {
            wait(Arg.Any<Task>(), timeout).Throws(new AggregateException(expectedException));
            getStreamResult().Throws(expectedException);
        }

        void TaskTimesOut(TimeSpan timeout)
        {
            wait(Arg.Any<Task>(), timeout).Returns(false);
        }

        async IAsyncEnumerable<int> TestStream()
        {
            yield return getStreamResult();
        }
    }
}