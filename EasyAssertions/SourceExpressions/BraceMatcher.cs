using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    abstract class BraceMatcher
    {
        const char OpenParen = '(';
        const char CloseParen = ')';
        const char OpenBrace = '{';
        const char CloseBrace = '}';
        const char DoubleQuotes = '"';
        const char Backslash = '\\';

        readonly string source;
        protected readonly Stack<char> ExpectedClosingBraces = new();
        bool inString;
        bool escapeNextChar;

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

        int MatchFrom(int startIndex)
        {
            inString = false;
            escapeNextChar = false;
            return source
                .IndexOfOrDefault(EvaluateChar, startIndex, -1);
        }

        bool EvaluateChar(char c)
        {
            if (IsUnescapedQuotes(c))
                inString = !inString;

            return inString
                ? EvaluateStringChar(c)
                : EvaluateCodeChar(c);
        }

        bool IsUnescapedQuotes(char c)
        {
            return !escapeNextChar
                && c == DoubleQuotes;
        }

        bool EvaluateStringChar(char c)
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

        bool IsExpectedClosingBrace(char c)
        {
            return ExpectedClosingBraces.Any() && c == ExpectedClosingBraces.Peek();
        }

        protected virtual bool EvaluateClosingBrace()
        {
            ExpectedClosingBraces.Pop();
            return false;
        }
    }

    class ClosingBraceFinder : BraceMatcher
    {
        public ClosingBraceFinder(string source) : base(source) { }

        protected override bool EvaluateClosingBrace()
        {
            base.EvaluateClosingBrace();
            return ExpectedClosingBraces.None();
        }
    }

    class CharFinder : BraceMatcher
    {
        readonly char charToFind;

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