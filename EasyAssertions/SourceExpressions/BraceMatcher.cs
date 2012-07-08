namespace EasyAssertions
{
    internal class BraceMatcher
    {
        private readonly string source;
        private readonly char open;
        private readonly char close;

        public static int FindClosingBrace(string source, int startIndex = 0, char open = '(', char close = ')')
        {
            return new BraceMatcher(source, open, close).MatchFrom(startIndex);
        }

        public static int FindNext(string source, char charToFind, int startIndex = 0)
        {
            return new BraceMatcher(source, '\0', charToFind).MatchFrom(startIndex, 1);
        }

        public BraceMatcher(string source, char open = '(', char close = ')')
        {
            this.source = source;
            this.open = open;
            this.close = close;
        }

        public int MatchFrom(int startIndex, int initialDepth = 0)
        {
            int depth = initialDepth;
            bool inString = false;
            bool escapeNextChar = false;
            for (int i = startIndex; i < source.Length; i++)
            {
                char c = source[i];

                if (!escapeNextChar && c == '"')
                {
                    inString = !inString;
                }

                if (inString)
                {
                    if (escapeNextChar)
                        escapeNextChar = false;
                    else if (c == '\\')
                        escapeNextChar = true;
                }
                else
                {
                    if (c == open)
                    {
                        depth++;
                    }
                    else if (c == close)
                    {
                        depth--;
                        if (depth == 0)
                            return i;
                    }
                }
            }
            return -1;
        }
    }
}