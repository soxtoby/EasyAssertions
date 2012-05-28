namespace EasyAssertions
{
    internal struct SourceAddress
    {
        public string FileName;
        public int LineIndex;
        public int ExpressionIndex;

        public bool Equals(SourceAddress other)
        {
            return Equals(other.FileName, FileName)
                && other.LineIndex == LineIndex
                    && other.ExpressionIndex == ExpressionIndex;
        }
    }
}