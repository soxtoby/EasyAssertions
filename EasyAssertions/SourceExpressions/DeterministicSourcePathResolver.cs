using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyAssertions;

/// <summary>
/// Deterministic builds (e.g. with ContinuousIntegrationBuild enabled) replace the source root in source paths
/// with a placeholder like <c>/_/</c>, so the real source root has to be found at runtime.
/// </summary>
class DeterministicSourcePathResolver
{
    static readonly Regex DeterministicRootPattern = new(@"^/_\d*/", RegexOptions.Compiled);

    public static readonly DeterministicSourcePathResolver Default = new(DefaultSearchDirectoryPaths);

    readonly Func<Assembly?, IEnumerable<string>> getSearchDirectoryPaths;
    readonly ConcurrentDictionary<(Assembly? CompiledAssembly, string DeterministicRoot), string> sourceRootPaths = new();

    public DeterministicSourcePathResolver(Func<Assembly?, IEnumerable<string>> getSearchDirectoryPaths) =>
        this.getSearchDirectoryPaths = getSearchDirectoryPaths;

    public string Resolve(string sourcePath, Assembly? compiledAssembly)
    {
        var deterministicRoot = DeterministicRootPattern.Match(sourcePath).Value;
        if (deterministicRoot == string.Empty)
            return sourcePath;

        var relativeSourcePath = sourcePath[deterministicRoot.Length..];

        return SourceRootPath(compiledAssembly, deterministicRoot, relativeSourcePath) is { } sourceRootPath
            ? Path.Combine(sourceRootPath, relativeSourcePath)
            : sourcePath;
    }

    string? SourceRootPath(Assembly? compiledAssembly, string deterministicRoot, string relativeSourcePath)
    {
        var cacheKey = (compiledAssembly, deterministicRoot);

        if (!sourceRootPaths.TryGetValue(cacheKey, out var sourceRootPath))
        {
            sourceRootPath = FindSourceRootPath(relativeSourcePath, getSearchDirectoryPaths(compiledAssembly));

            // A failed search isn't cached, so that one missing file (e.g. generated code) doesn't write off the whole source root
            if (sourceRootPath is not null)
                sourceRootPaths[cacheKey] = sourceRootPath;
        }

        return sourceRootPath;
    }

    /// <summary>
    /// Looks for the source file relative to each of the search directories and their ancestors,
    /// on the assumption that the code is being run from somewhere underneath the source root.
    /// </summary>
    static string? FindSourceRootPath(string relativeSourcePath, IEnumerable<string> searchDirectoryPaths)
    {
        return searchDirectoryPaths
            .SelectMany(SelfAndAncestors)
            .Distinct()
            .FirstOrDefault(directoryPath => File.Exists(Path.Combine(directoryPath, relativeSourcePath)));
    }

    static IEnumerable<string> SelfAndAncestors(string directoryPath)
    {
        for (var directory = new DirectoryInfo(directoryPath); directory is not null; directory = directory.Parent)
            yield return directory.FullName;
    }

    // Deferred because getting the location of a dynamic assembly can throw, which shouldn't get in the way of reading ordinary source paths
    static IEnumerable<string> DefaultSearchDirectoryPaths(Assembly? compiledAssembly)
    {
        if (compiledAssembly?.Location.NullIfEmpty() is { } assemblyPath)
            yield return Path.GetDirectoryName(assemblyPath);

        yield return AppContext.BaseDirectory;
        yield return Environment.CurrentDirectory;
    }
}