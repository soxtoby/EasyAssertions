using System.Reflection;

namespace EasyAssertions;

record SourceAddress(string? FilePath, int LineNumber, int ColumnNumber, Assembly? CompiledAssembly)
{
    public override string ToString() => $"{FilePath}:{LineNumber}:{ColumnNumber}";
}