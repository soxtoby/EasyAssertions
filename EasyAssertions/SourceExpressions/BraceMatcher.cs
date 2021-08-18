using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    internal abstract class BraceMatcher
    {
        private const char OpenParen = '(';
        private const char CloseParen = ')';
        private const char OpenBrace = '{';
        private const char CloseBrace = '}';
        private const char DoubleQuotes = '"';
        private const char Backslash = '\\';

        private readonly string source;
        protected readonly Stack<char> ExpectedClosingBraces = new();
        private bool inString;
        private bool escapeNextChar;

        public static int FindClosingBrace(string source, int startIndex = 0)
        {
            return new ClosingBraceFinder(source).MatchFrom(startIndex);
        }

        public static int FindNext(string source, char charToFind, int startIndex = 0)
        {
            return new CharFinder(source, charToFind).MatchFrom(startIndex);
        }

        protected BraceMatcher(string source)
        {
            this.source = source;
        }

        private int MatchFrom(int startIndex)
        {
            inString = false;
            escapeNextChar = false;
            return source
                .IndexOfOrDefault(EvaluateChar, startIndex, -1);
        }

        private bool EvaluateChar(char c)
        {
            if (IsUnescapedQuotes(c))
                inString = !inString;

            return inString
                ? EvaluateStringChar(c)
                : EvaluateCodeChar(c);
        }

        private bool IsUnescapedQuotes(char c)
        {
            return !escapeNextChar
                && c == DoubleQuotes;
        }

        private bool EvaluateStringChar(char c)
        {
            if (escapeNextChar)
                escapeNextChar = false;
            else if (c == Backslash)
                escapeNextChar = true;
            return false;
        }

        protected virtual bool EvaluateCodeChar(char c)
        {
            switch (c)
            {
                case OpenParen:
                    ExpectedClosingBraces.Push(CloseParen);
                    break;
                case OpenBrace:
                    ExpectedClosingBraces.Push(CloseBrace);
                    break;
                default:
                    if (IsExpectedClosingBrace(c))
                        return EvaluateClosingBrace();
                    break;
            }
            return false;
        }

        private bool IsExpectedClosingBrace(char c)
        {
            return ExpectedClosingBraces.Any() && c == ExpectedClosingBraces.Peek();
        }

        protected virtual bool EvaluateClosingBrace()
        {
            ExpectedClosingBraces.Pop();
            return false;
        }
    }

    internal class ClosingBraceFinder : BraceMatcher
    {
        public ClosingBraceFinder(string source) : base(source) { }

        protected override bool EvaluateClosingBrace()
        {
            base.EvaluateClosingBrace();
            return ExpectedClosingBraces.None();
        }
    }

    internal class CharFinder : BraceMatcher
    {
        private readonly char charToFind;

        public CharFinder(string source, char charToFind)
            : base(source)
        {
            this.charToFind = charToFind;
        }

        protected override bool EvaluateCodeChar(char c)
        {
            return ExpectedClosingBraces.None() && c == charToFind
                || base.EvaluateCodeChar(c);
        }
    }
}