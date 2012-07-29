using System.Collections.Generic;
using System.Linq;

namespace EasyAssertions
{
    internal class BraceMatcher
    {
        private const char OpenParen = '(';
        private const char CloseParen = ')';
        private const char OpenBrace = '{';
        private const char CloseBrace = '}';
        private const char DoubleQuotes = '"';
        private const char Backslash = '\\';
        private const char Null = '\0';

        private readonly string source;
        private readonly Stack<char> expectedClosingBraces = new Stack<char>();

        public static int FindClosingBrace(string source, int startIndex = 0)
        {
            return new BraceMatcher(source).MatchFrom(startIndex, Null);
        }

        public static int FindNext(string source, char charToFind, int startIndex = 0)
        {
            return new BraceMatcher(source).MatchFrom(startIndex, charToFind);
        }

        public BraceMatcher(string source)
        {
            this.source = source;
        }

        public int MatchFrom(int startIndex, char charToFind)
        {
            bool inString = false;
            bool escapeNextChar = false;
            for (int i = startIndex; i < source.Length; i++)
            {
                char c = source[i];

                if (!escapeNextChar && c == DoubleQuotes)
                    inString = !inString;

                if (inString)
                {
                    if (escapeNextChar)
                        escapeNextChar = false;
                    else if (c == Backslash)
                        escapeNextChar = true;
                }
                else
                {
                    if (c == OpenParen)
                    {
                        expectedClosingBraces.Push(CloseParen);
                    }
                    else if (c == OpenBrace)
                    {
                        expectedClosingBraces.Push(CloseBrace);
                    }
                    else if (expectedClosingBraces.None() && c == charToFind)
                    {
                        return i;
                    }
                    else if (expectedClosingBraces.Any() && c == expectedClosingBraces.Peek())
                    {
                        expectedClosingBraces.Pop();
                        if (charToFind == Null && expectedClosingBraces.None())
                            return i;
                    }
                }
            }
            return -1;
        }
    }
}