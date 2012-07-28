using System;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Output;
using SmartFormat.Core.Parsing;

namespace EasyAssertions
{
    internal class ExpectedFormatter : IFormatter
    {
        private ExpectedFormatter() { }

        public static readonly ExpectedFormatter Instance = new ExpectedFormatter();

        public void EvaluateFormat(object current, Format format, ref bool handled, IOutput output, FormatDetails formatDetails)
        {
            Expected expected = current as Expected;
            if (expected == null) return;

            handled = true;

            if (!string.IsNullOrEmpty(expected.Expression))
                OutputExpectedExpression(expected, output, formatDetails);

            if (format == null)
                OutputExpectedValue(expected, output, formatDetails);
            else
                OutputFormattedExpected(current, format, output, formatDetails, expected);
        }

        private static void OutputExpectedExpression(Expected expected, IOutput output, FormatDetails formatDetails)
        {
            output.Write(expected.Expression + Environment.NewLine + NewLineIndent(output), formatDetails);
        }

        private static void OutputExpectedValue(Expected expected, IOutput output, FormatDetails formatDetails)
        {
            output.Write(string.Empty + expected.Value, formatDetails);
        }

        private static void OutputFormattedExpected(object current, Format format, IOutput output, FormatDetails formatDetails, Expected expected)
        {
            formatDetails.Formatter.Format(output, format, current, formatDetails);
        }

        private static string NewLineIndent(IOutput output)
        {
            string currentOutput = output.ToString();
            int previousNewLine = currentOutput.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
            int startOfLine = previousNewLine < 0 ? 0
                : previousNewLine + Environment.NewLine.Length;
            int indexIntoLine = currentOutput.Length - startOfLine;
            return new string(' ', indexIntoLine);
        }
    }
}