﻿using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class TaskAssertionTests : AssertionTests
    {
        TaskCompletionSource<int> taskSource = null!;
        Func<Task, TimeSpan, bool> defaultWaitForTask = null!;
        Func<Task, TimeSpan, bool> wait = null!;
        Task<int> task = null!;

        [SetUp]
        public void SetUp()
        {
            defaultWaitForTask = TaskAssertions.WaitForTask;
            wait = Substitute.For<Func<Task, TimeSpan, bool>>();
            TaskAssertions.WaitForTask = wait;
            taskSource = new TaskCompletionSource<int>();
            task = taskSource.Task;
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

            ((Task)task).ShouldComplete();
        }

        [Test]
        public void ShouldComplete_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => ((Task)task).ShouldComplete(msg));
        }

        [Test]
        public void ShouldComplete_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldComplete(msg));
        }

        [Test]
        public void ShouldComplete_CorrectlyRegistersAssertion()
        {
            Task actualExpression = task;

            AssertTimesOut(TimeSpan.FromSeconds(1), msg => actualExpression.ShouldComplete(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_CompletesWithinTimeout_DoesNotThrow()
        {
            TaskReturns(0, TimeSpan.FromMilliseconds(1));

            ((Task)task).ShouldComplete(1);
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => ((Task)task).ShouldComplete(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldComplete(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            uint timeout = 1;
            Task actualExpression = task;

            AssertTimesOut(TimeSpan.FromMilliseconds(timeout), msg => actualExpression.ShouldComplete(timeout, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_CompletesWithinTimeout_DoesNotThrow()
        {
            TaskReturns(0, TimeSpan.FromMilliseconds(1));

            ((Task)task).ShouldComplete(TimeSpan.FromMilliseconds(1));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => ((Task)task).ShouldComplete(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => ((Task)task).ShouldComplete(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldComplete(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            Task actualExpression = task;

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
        public void ShouldCompleteWithValue_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task<int>), null, msg => ((Task<int>?)null).ShouldComplete(msg));
        }

        [Test]
        public void ShouldCompleteWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertTimesOut(TimeSpan.FromSeconds(1),msg => actualExpression.ShouldComplete(msg));

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
        public void ShouldCompleteWithinMillisecondsWithValue_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task<int>), null, msg => ((Task<int>?)null).ShouldComplete(1, msg));
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
        public void ShouldCompleteWithinTimeSpanWithValue_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task<int>), null, msg => ((Task<int>?)null).ShouldComplete(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => actualExpression.ShouldComplete(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithType_FailsWithCorrectType_ReturnsException()
        {
            AssertReturnsException(new InvalidOperationException(), TimeSpan.FromSeconds(1), () => task.ShouldFailWith<InvalidOperationException>());
        }

        [Test]
        public void ShouldFailWithType_ExpectedTaskCanceledException_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromSeconds(1), () => task.ShouldFailWith<TaskCanceledException>());
        }

        [Test]
        public void ShouldFailWithType_FailsWithWrongType_FailsWithWrongExceptionMessage()
        {
            AssertFailsWithWrongExceptionMessage(typeof(InvalidOperationException), new Exception(), TimeSpan.FromSeconds(1), msg => task.ShouldFailWith<InvalidOperationException>(msg));
        }

        [Test]
        public void ShouldFailWithType_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => task.ShouldFailWith<InvalidOperationException>(msg));
        }

        [Test]
        public void ShouldFailWithType_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFailWith<InvalidOperationException>(msg));
        }

        [Test]
        public void ShouldFailWithType_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldFailWith<Exception>(msg));
        }

        [Test]
        public void ShouldFailWithType_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertTimesOut(TimeSpan.FromSeconds(1), msg => actualExpression.ShouldFailWith<Exception>(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_FailsWithCorrectType_ReturnsException()
        {
            AssertReturnsException(new InvalidOperationException(), TimeSpan.FromMilliseconds(1), () => task.ShouldFailWith<InvalidOperationException>(1));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_ExpectedTaskCanceledException_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => task.ShouldFailWith<TaskCanceledException>(1));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_FailsWithWrongType_FailsWithWrongExceptionMessage()
        {
            AssertFailsWithWrongExceptionMessage(typeof(InvalidOperationException), new Exception(), TimeSpan.FromMilliseconds(1), msg => task.ShouldFailWith<InvalidOperationException>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldFailWith<InvalidOperationException>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFailWith<InvalidOperationException>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldFailWith<Exception>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFailWith<InvalidOperationException>(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_FailsWithCorrectType_ReturnsException()
        {
            AssertReturnsException(new InvalidOperationException(), TimeSpan.FromMilliseconds(1), () => task.ShouldFailWith<InvalidOperationException>(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_ExpectedTaskCanceledException_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => task.ShouldFailWith<TaskCanceledException>(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => task.ShouldFailWith<Exception>(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_FailsWithWrongType_FailsWithWrongExceptionMessage()
        {
            AssertFailsWithWrongExceptionMessage(typeof(InvalidOperationException), new Exception(), TimeSpan.FromMilliseconds(1), msg => task.ShouldFailWith<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldFailWith<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFailWith<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldFailWith<Exception>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFailWith<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFail_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromSeconds(1), () => task.ShouldFail());
        }

        [Test]
        public void ShouldFail_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromSeconds(1), () => task.ShouldFail());
        }

        [Test]
        public void ShouldFail_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromSeconds(1), msg => task.ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldFail(msg));
        }

        [Test]
        public void ShouldFail_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertTimesOut(TimeSpan.FromSeconds(1), msg => actualExpression.ShouldFail(msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinMilliseconds_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => task.ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => task.ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(1, msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

        [Test]
        public void ShouldFailWithinTimeSpan_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), TimeSpan.FromMilliseconds(1), () => task.ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TaskCancelled_ReturnsException()
        {
            AssertReturnsTaskCanceledException(TimeSpan.FromMilliseconds(1), () => task.ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            var result = Assert.Throws<ArgumentOutOfRangeException>(() => task.ShouldFail(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => task.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => task.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_ActualIsNull_FailsWithWrongTypeMessage()
        {
            AssertFailsWithTypesNotEqualMessage(typeof(Task), null, msg => ((Task?)null).ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            var actualExpression = task;

            AssertFailsWithNoExceptionMessage(msg => actualExpression.ShouldFail(TimeSpan.FromMilliseconds(1), msg));

            Assert.AreEqual(nameof(actualExpression), TestExpression.GetActual());
        }

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
            wait(task, timeout).Returns(c => task.Wait(timeout));
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
            TaskReturns(0, Arg.Any<TimeSpan>());

            var result = Assert.Throws<Exception>(() => callAssertion("foo"));

            Assert.AreSame(ExpectedException, result);
        }

        void TaskReturns(int expectedValue, TimeSpan timeout)
        {
            wait(task, timeout).Returns(true);
            taskSource.SetResult(expectedValue);
        }

        void TaskFails(Exception expectedException, TimeSpan timeout)
        {
            wait(task, timeout).Throws(new AggregateException(expectedException));
            taskSource.SetException(expectedException);
        }

        void TaskTimesOut(TimeSpan timeout)
        {
            wait(task, timeout).Returns(false);
        }
    }
}
