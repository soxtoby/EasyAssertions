using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace EasyAssertions.UnitTests
{
    public class TaskAssertionTests : AssertionTests
    {
        private TaskCompletionSource<int> taskSource;

        [SetUp]
        public void SetUp()
        {
            taskSource = new TaskCompletionSource<int>();
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_CompletesWithinTimeout_DoesNotThrow()
        {
            taskSource.SetResult(0);
            
            ((Task)taskSource.Task).ShouldCompleteWithin(1);
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => ((Task)taskSource.Task).ShouldCompleteWithin(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            taskSource.SetResult(0);
            uint timeout = 1;
            Task actualTask = taskSource.Task;

            actualTask.ShouldCompleteWithin(timeout);

            Assert.AreEqual(nameof(actualTask), TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_CompletesWithinTimeout_DoesNotThrow()
        {
            taskSource.SetResult(0);

            ((Task)taskSource.Task).ShouldCompleteWithin(TimeSpan.FromMilliseconds(1));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException result = Assert.Throws<ArgumentOutOfRangeException>(() => ((Task)taskSource.Task).ShouldCompleteWithin(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => ((Task)taskSource.Task).ShouldCompleteWithin(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            taskSource.SetResult(0);
            TimeSpan timeout = TimeSpan.FromMilliseconds(1);
            Task actualTask = taskSource.Task;

            actualTask.ShouldCompleteWithin(timeout);

            Assert.AreEqual(nameof(actualTask), TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_CompletesWithinTimeout_ReturnsResult()
        {
            AssertReturnsResult(1, () => taskSource.Task.ShouldCompleteWithin(1));
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException result = Assert.Throws<ArgumentOutOfRangeException>(() => taskSource.Task.ShouldCompleteWithin(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => taskSource.Task.ShouldCompleteWithin(1, msg));
        }

        [Test]
        public void ShouldCompleteWithinMillisecondsWithValue_CorrectlyRegistersAssertion()
        {
            taskSource.SetResult(0);
            uint timeout = 1;

            taskSource.Task.ShouldCompleteWithin(timeout);

            Assert.AreEqual($"{nameof(taskSource)}.{nameof(taskSource.Task)}", TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CompletesWithinTimeout_ReturnsResult()
        {
            AssertReturnsResult(1, () => taskSource.Task.ShouldCompleteWithin(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_DoesNotComplete_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => taskSource.Task.ShouldCompleteWithin(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldCompleteWithinTimeSpanWithValue_CorrectlyRegistersAssertion()
        {
            taskSource.SetResult(0);
            uint timeout = 1;

            taskSource.Task.ShouldCompleteWithin(timeout);

            Assert.AreEqual($"{nameof(taskSource)}.{nameof(taskSource.Task)}", TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_FailsWithCorrectType_ReturnsException()
        {
            AssertReturnsException(new InvalidOperationException(), () => taskSource.Task.ShouldFail<InvalidOperationException>(1));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_FailsWithWrongType_FailsWithWrongExceptionMessage()
        {
            AssertFailsWithWrongExceptionMessage(typeof(InvalidOperationException), new Exception(), msg => taskSource.Task.ShouldFail<InvalidOperationException>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => taskSource.Task.ShouldFail<InvalidOperationException>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => taskSource.Task.ShouldFail<InvalidOperationException>(1, msg));
        }

        [Test]
        public void ShouldFailWithinMillisecondsWithType_CorrectlyRegistersAssertion()
        {
            taskSource.SetException(new Exception());
            uint timeout = 1;

            taskSource.Task.ShouldFail<Exception>(timeout);

            Assert.AreEqual($"{nameof(taskSource)}.{nameof(taskSource.Task)}", TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_FailsWithCorrectType_ReturnsException()
        {
            AssertReturnsException(new InvalidOperationException(), () => taskSource.Task.ShouldFail<InvalidOperationException>(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException result = Assert.Throws<ArgumentOutOfRangeException>(() => taskSource.Task.ShouldFail<Exception>(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_FailsWithWrongType_FailsWithWrongExceptionMessage()
        {
            AssertFailsWithWrongExceptionMessage(typeof(InvalidOperationException), new Exception(), msg => taskSource.Task.ShouldFail<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => taskSource.Task.ShouldFail<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => taskSource.Task.ShouldFail<InvalidOperationException>(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpanWithType_CorrectlyRegistersAssertion()
        {
            taskSource.SetException(new Exception());
            TimeSpan timeout = TimeSpan.FromMilliseconds(1);

            taskSource.Task.ShouldFail<Exception>(timeout);

            Assert.AreEqual($"{nameof(taskSource)}.{nameof(taskSource.Task)}", TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldFailWithinMilliseconds_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), () => taskSource.Task.ShouldFail(1));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => taskSource.Task.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => taskSource.Task.ShouldFail(1, msg));
        }

        [Test]
        public void ShouldFailWithinMilliseconds_CorrectlyRegistersAssertion()
        {
            taskSource.SetException(new Exception());
            uint timeout = 1;

            taskSource.Task.ShouldFail(timeout);

            Assert.AreEqual($"{nameof(taskSource)}.{nameof(taskSource.Task)}", TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        [Test]
        public void ShouldFailWithinTimeSpan_Fails_ReturnsException()
        {
            AssertReturnsException(new Exception(), () => taskSource.Task.ShouldFail(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_NegativeTimeSpan_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException result = Assert.Throws<ArgumentOutOfRangeException>(() => taskSource.Task.ShouldFail(TimeSpan.FromTicks(-1)));

            Assert.AreEqual("timeout", result.ParamName);
        }

        [Test]
        public void ShouldFailWithinTimeSpan_TimesOut_FailsWithTimeoutMessage()
        {
            AssertTimesOut(TimeSpan.FromMilliseconds(1), msg => taskSource.Task.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_DoesNotFail_FailsWithNoExceptionMessage()
        {
            AssertFailsWithNoExceptionMessage(msg => taskSource.Task.ShouldFail(TimeSpan.FromMilliseconds(1), msg));
        }

        [Test]
        public void ShouldFailWithinTimeSpan_CorrectlyRegistersAssertion()
        {
            taskSource.SetException(new Exception());
            TimeSpan timeout = TimeSpan.FromMilliseconds(1);

            taskSource.Task.ShouldFail(timeout);

            Assert.AreEqual($"{nameof(taskSource)}.{nameof(taskSource.Task)}", TestExpression.GetActual());
            Assert.AreEqual(nameof(timeout), TestExpression.GetExpected());
        }

        private void AssertReturnsResult(int expectedResult, Func<Actual<int>> callAssertion)
        {
            taskSource.SetResult(1);

            Actual<int> result = callAssertion();

            Assert.AreEqual(expectedResult, result.And);
        }

        private void AssertReturnsException<TException>(TException expectedException, Func<ActualException<TException>> callAssertion) where TException : Exception
        {
            taskSource.SetException(expectedException);

            ActualException<TException> result = callAssertion();

            Assert.AreSame(expectedException, result.And);
        }

        private void AssertTimesOut(TimeSpan timeout, Action<string> callAssertion)
        {
            MockFormatter.TaskTimedOut(timeout, "foo").Returns("bar");

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => callAssertion("foo"));

            Assert.AreEqual("bar", result.Message);
        }

        private void AssertFailsWithWrongExceptionMessage<TException>(Type expectedExceptionType, Exception actualException, Func<string, ActualException<TException>> callAssertion) where TException : Exception
        {
            MockFormatter.WrongException(expectedExceptionType, actualException.GetType(), null, "foo").Returns("bar");
            taskSource.SetException(actualException);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => callAssertion("foo"));

            Assert.AreEqual("bar", result.Message);
            Assert.AreSame(actualException, result.InnerException);
        }

        private void AssertFailsWithNoExceptionMessage<TException>(Func<string, ActualException<TException>> callAssertion) where TException : Exception
        {
            MockFormatter.NoException(typeof(TException), message: "foo").Returns("bar");
            taskSource.SetResult(0);

            EasyAssertionException result = Assert.Throws<EasyAssertionException>(() => callAssertion("foo"));

            Assert.AreEqual("bar", result.Message);
        }
    }
}
