namespace EasyAssertions;

record SourceAddress(string? FilePath, int LineNumber, int ColumnNumber)
{
    public override string ToString() => $"{FilePath}:{LineNumber}:{ColumnNumber}";
}