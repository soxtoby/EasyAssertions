using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class ValueTaskAssertionTests : AssertionTests
    {
        TaskCompletionSource<int> taskSource = null!;
        Func<Task, TimeSpan, bool> defaultWaitForTask = null!;
        Func<Task, TimeSpan, bool> wait = null!;
        ValueTask<int> task;

        [SetUp]
        public void SetUp()
        {
            defaultWaitForTask = TaskAssertions.WaitForTask;
            wait = Substitute.For<Func<Task, TimeSpan, bool>>();
            TaskAssertions.WaitForTask = wait;
            taskSource = new TaskCompletionSource<int>();
            task = new ValueTask<int>(taskSource.Task);
        }

        [TearDown]
        public void TearDown()
        {
            TaskAssertions.WaitForTask = defaultWaitForTask;
        }

        [Test]
        public void ShouldComplete_CompletesWithinOneSecond_DoesNotThrow()
        {
            TaskReturns(0, TimeSpan.FromSeconds(1));

            VoidTask().ShouldComplete();
        }

        [Test]
        public void ShouldComplete_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => VoidTask().ShouldComplete(msg));
        }

        [Test]
        public void ShouldComplete_CorrectlyRegistersAssertion()
        {
            var actualExpression = VoidTask();

            AssertTimesOut(TimeSpan.FromSeconds(1), msg => actualExpression.ShouldComplete(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_CompletesWithinTimeout_DoesNotThrow()
        {
            TaskReturns(0, TimeSpan.FromMilliseconds(1));

            VoidTask().ShouldComplete(1);
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => VoidTask().ShouldComplete(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            uint timeout = 1;
            var actualExpression = VoidTask();

            AssertTimesOut(TimeSpan.FromMilliseconds(timeout), msg => actualExpression.ShouldComplete(timeout, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_CompletesWithinTimeout_DoesNotThrow()
        {
            TaskReturns(0, TimeSpan.FromMilliseconds(1));

            VoidTask().ShouldComplete(TimeSpan.FromMilliseconds(1));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => VoidTask().ShouldComplete(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => VoidTask().ShouldComplete(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            var actualExpression = VoidTask();

            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => actualExpression.ShouldComplete(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithValue_CompletesWithinOneSecond_ReturnsResult()
        {
            AssertReturnsResult(1, TimeSpan.FromSeconds(1), () => task.ShouldComplete());
        }

        [Test]
        public void ShouldCompleteWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => task.ShouldComplete(msg));
        }

        [Test]
        public void ShouldCompleteWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertTimesOut(TimeSpan.FromSeconds(1), msg => actualExpression.ShouldComplete(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_CompletesWithinTimeout_ReturnsResult()
        {
            AssertReturnsResult(1, TimeSpan.FromMilliseconds(1), () => task.ShouldComplete(1));
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => task.ShouldComplete(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldComplete(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => actualExpression.ShouldComplete(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CompletesWithinTimeout_ReturnsResult()
        {
            AssertReturnsResult(1, TimeSpan.FromMilliseconds(1), () => task.ShouldComplete(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldComplete(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => actualExpression.ShouldComplete(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFail_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromSeconds(1), () => VoidTask().ShouldFail());
        }

        [Test]
        public void ShouldFail_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromSeconds(1), () => VoidTask().ShouldFail());
        }

        [Test]
        public void ShouldFail_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => VoidTask().ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => VoidTask().ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_CorrectlyRegistersAssertion()
        {
            var actualExpression = VoidTask();

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinMilliseconds_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => VoidTask().ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => VoidTask().ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => VoidTask().ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => VoidTask().ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            var actualExpression = VoidTask();

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinTimeSpan_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => VoidTask().ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => VoidTask().ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => VoidTask().ShouldFail(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => VoidTask().ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => VoidTask().ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            var actualExpression = VoidTask();

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithValue_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromSeconds(1), () => task.ShouldFail());
        }

        [Test]
        public void ShouldFailWithValue_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromSeconds(1), () => task.ShouldFail());
        }

        [Test]
        public void ShouldFailWithValue_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => task.ShouldFail(msg));
        }

        [Test]
        public void ShouldFailWithValue_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFail(msg));
        }

        [Test]
        public void ShouldFailWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithValue_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => task.ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithValue_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => task.ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithValue_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithValue_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithValue_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => task.ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithValue_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => task.ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithValue_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException result = Assert.Throws<ArgumentOutOfRangeException>(() => task.ShouldFail(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithValue_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithValue_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        async ValueTask VoidTask() => await task;

        void AssertReturnsResult(int expectedResult, TimeSpan timeout, Func<Actual<int>> callAssertion)
        {
            TaskReturns(expectedResult, timeout);

            var result = callAssertion();

            Assert.AreEqual(expectedResult, result.And);
        }

        void AssertReturnsException<TException>(TException expectedException, TimeSpan timeout, Func<ActualException<TException>> callAssertion) where TException : Exception
        {
            TaskFails(expectedException, timeout);

            var result = callAssertion();

            Assert.AreSame(expectedException, result.And);
        }

        void AssertReturnsTaskCanceledException<TException>(TimeSpan timeout, Func<ActualException<TException>> callAssertion)
            where TException : Exception
        {
            wait(Arg.Any<Task>(), timeout).Returns(c => c.Arg<Task>().Wait(timeout));
            taskSource.SetCanceled();

            var result = callAssertion();

            Assert.IsInstanceOf<TaskCanceledException>(result.And);
        }

        void AssertTimesOut(TimeSpan timeout, Action<string> callAssertion)
        {
            Error.TaskTimedOut(timeout, "foo").Returns(ExpectedException);
            TaskTimesOut(timeout);

            var result = Assert.Throws<Exception>(() => callAssertion("foo"));

            Assert.AreSame(ExpectedException, result);
        }

        void AssertFailsWithWrongExceptionMessage<TException>(Type expectedExceptionType, Exception actualException, TimeSpan timeout, Func<string, ActualException<TException>> callAssertion) where TException : Exception
        {
            Error.WrongException(expectedExceptionType, actualException, null, "foo").Returns(ExpectedException);
            TaskFails(actualException, timeout);

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
            taskSource.SetResult(expectedValue);
        }

        void TaskFails(Exception expectedException, TimeSpan timeout)
        {
            wait(Arg.Any<Task>(), timeout).Throws(new AggregateException(expectedException));
            taskSource.SetException(expectedException);
        }

        void TaskTimesOut(TimeSpan timeout)
        {
            wait(Arg.Any<Task>(), timeout).Returns(false);
        }
    }
}
