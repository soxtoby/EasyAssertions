using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EasyAssertions
{
    internal class SourceExpressionProvider : TestExpressionProvider
    {
        private readonly List<Method> methodCalls = new List<Method>();

        public static readonly SourceExpressionProvider Instance = new SourceExpressionProvider();

        private SourceExpressionProvider() { }

        public string GetExpression()
        {
            string expression = "Result";

            Source source = methodCalls.First().Source;

            if (source.FileName != null && File.Exists(source.FileName))
            {
                string[] sourceLines = File.ReadAllLines(source.FileName);
                string expressionSource = sourceLines.Skip(source.LineIndex).Join(Environment.NewLine);

                expression = string.Empty;
                ExpressionSegment segment = new ExpressionSegment { From = source.ExpressionIndex, To = source.ExpressionIndex };

                foreach (Method method in methodCalls)
                {
                    segment = method.GetSegment(expressionSource, segment.From);
                    expression += expressionSource
                        .Substring(segment.From, segment.To - segment.From)
                        .Trim();
                }
            }
            return expression;
        }

        public void RegisterAssertionMethod()
        {
            RegisterMethod(source => new Assertion(source));
        }

        public void RegisterContinuation()
        {
            RegisterMethod(source => new Continuation(source));
        }

        private void RegisterMethod(Func<Source, Method> createMethod)
        {
            Source source = GetSource();

            if (methodCalls.Any() && !methodCalls.First().Source.Equals(source))
                methodCalls.Clear();

            methodCalls.Add(createMethod(source));
        }

        private static Source GetSource()
        {
            StackTrace stack = new StackTrace(true);
            StackFrame[] frames = stack.GetFrames();

            if (frames == null)
                return new Source();

            int testFrameIndex = frames.IndexOf(f => f.GetMethod().DeclaringType.Namespace != "EasyAssertions"); // FIXME use attribute
            StackFrame testFrame = frames[testFrameIndex];
            StackFrame assertionFrame = frames[testFrameIndex - 1];

            return new Source
                {
                    FileName = testFrame.GetFileName(),
                    LineIndex = testFrame.GetFileLineNumber() - 1,
                    ExpressionIndex = testFrame.GetFileColumnNumber() - 1,
                    AssertionMethod = GetAssertionMethodName(assertionFrame)
                };
        }

        private static string GetAssertionMethodName(StackFrame assertionFrame)
        {
            MethodBase method = assertionFrame.GetMethod();
            return method.Name.StartsWith("get_")
                ? method.Name.Substring(4)
                : method.Name;
        }

        private struct Source
        {
            public string FileName;
            public int LineIndex;
            public int ExpressionIndex;
            public string AssertionMethod;

            public bool Equals(Source other)
            {
                return Equals(other.FileName, FileName)
                    && other.LineIndex == LineIndex
                    && other.ExpressionIndex == ExpressionIndex;
            }
        }

        private abstract class Method
        {
            public readonly Source Source;

            protected Method(Source source)
            {
                Source = source;
            }

            public abstract ExpressionSegment GetSegment(string expressionSource, int fromIndex);

            protected int GetMethodCallIndex(string expressionSource, int fromIndex)
            {
                return expressionSource.IndexOf("." + Source.AssertionMethod, fromIndex);
            }
        }

        private class Assertion : Method
        {
            public Assertion(Source source)
                : base(source)
            {
            }

            public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
            {
                return new ExpressionSegment
                    {
                        From = fromIndex,
                        To = GetMethodCallIndex(expressionSource, fromIndex)
                    };
            }
        }

        private class Continuation : Method
        {
            public Continuation(Source source)
                : base(source)
            {
            }

            public override ExpressionSegment GetSegment(string expressionSource, int fromIndex)
            {
                int endOfContinuationProperty = GetMethodCallIndex(expressionSource, fromIndex) + Source.AssertionMethod.Length + 1;
                return new ExpressionSegment
                    {
                        From = endOfContinuationProperty,
                        To = endOfContinuationProperty
                    };
            }
        }

        private class ExpressionSegment
        {
            public int From;
            public int To;
        }
    }
}