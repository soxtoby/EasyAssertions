using System;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Output;
using SmartFormat.Core.Parsing;

namespace EasyAssertions
{
    public class ExpectedFormatter : IFormatter
    {
        private ExpectedFormatter() { }

        public static readonly ExpectedFormatter Instance = new ExpectedFormatter();

        public void EvaluateFormat(object current, Format format, ref bool handled, IOutput output, FormatDetails formatDetails)
        {
            FailureMessage.ExpectedWrapper expected = current as FailureMessage.ExpectedWrapper;
            if (expected == null) return;

            handled = true;

            if (expected.Expression != null)
                OutputExpectedExpression(expected, output, formatDetails);

            if (format == null)
                OutputExpectedValue(expected, output, formatDetails);
            else
                OutputFormattedExpected(current, format, output, formatDetails, expected);
        }

        private static void OutputExpectedExpression(FailureMessage.ExpectedWrapper expected, IOutput output, FormatDetails formatDetails)
        {
            output.Write(expected.Expression + Environment.NewLine + NewLineIndent(output), formatDetails);
        }

        private static void OutputExpectedValue(FailureMessage.ExpectedWrapper expected, IOutput output, FormatDetails formatDetails)
        {
            output.Write(string.Empty + expected.Value, formatDetails);
        }

        private static void OutputFormattedExpected(object current, Format format, IOutput output, FormatDetails formatDetails, FailureMessage.ExpectedWrapper expected)
        {
            formatDetails.Formatter.Format(output, format, current, formatDetails);
        }

        private static string NewLineIndent(IOutput output)
        {
            string currentOutput = output.ToString();
            int indexIntoLine = currentOutput.Length - Environment.NewLine.Length - currentOutput.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
            return new string(' ', indexIntoLine);
        }
    }
}