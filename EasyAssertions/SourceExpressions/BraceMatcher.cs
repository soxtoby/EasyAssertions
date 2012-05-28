namespace EasyAssertions
{
    public class BraceMatcher
    {
        private readonly string source;

        public BraceMatcher(string source)
        {
            this.source = source;
        }

        public int MatchFrom(int startIndex)
        {
            int depth = 0;
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
                    if (c == '(')
                    {
                        depth++;
                    }
                    else if (c == ')')
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