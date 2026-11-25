using System.Reflection;
using NUnit.Framework;

namespace EasyAssertions.UnitTests;

public class DeterministicSourcePathResolverTests
{
    string tempDirectoryPath = null!;

    [SetUp]
    public void SetUp()
    {
        tempDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(tempDirectoryPath, true);
    }

    [Test]
    public void Resolve_SourceFileUnderAncestorOfSearchDirectory_ReturnsRealPath()
    {
        var sourceFilePath = CreateFile("Project", "Tests.cs");
        var outputDirectoryPath = CreateDirectory("Project", "bin", "Debug");
        var sut = new DeterministicSourcePathResolver(_ => new[] { outputDirectoryPath });

        var result = sut.Resolve("/_/Project/Tests.cs", null);

        Assert.AreEqual(sourceFilePath, Path.GetFullPath(result));
    }

    [Test]
    public void Resolve_SourceFileUnderLaterSearchDirectory_ReturnsRealPath()
    {
        var sourceFilePath = CreateFile("Source", "Project", "Tests.cs");
        var outputDirectoryPath = CreateDirectory("Output", "bin");
        var workingDirectoryPath = CreateDirectory("Source", "Project");
        var sut = new DeterministicSourcePathResolver(_ => new[] { outputDirectoryPath, workingDirectoryPath });

        var result = sut.Resolve("/_/Project/Tests.cs", null);

        Assert.AreEqual(sourceFilePath, Path.GetFullPath(result));
    }

    [Test]
    public void Resolve_AfterSourceFileNotFound_StillResolvesOtherSourceFiles()
    {
        var sourceFilePath = CreateFile("Project", "Tests.cs");
        var outputDirectoryPath = CreateDirectory("Project", "bin");
        var sut = new DeterministicSourcePathResolver(_ => new[] { outputDirectoryPath });
        sut.Resolve("/_/Project/Generated.cs", null);

        var result = sut.Resolve("/_/Project/Tests.cs", null);

        Assert.AreEqual(sourceFilePath, Path.GetFullPath(result));
    }

    [Test]
    public void Resolve_AssembliesFromDifferentSourceRoots_ResolvesEachUnderItsOwnSourceRoot()
    {
        var testsAssembly = typeof(DeterministicSourcePathResolverTests).Assembly;
        var libraryAssembly = typeof(EasyAssertion).Assembly;
        var testsSourceFilePath = CreateFile("TestsRepo", "Project", "Assertions.cs");
        var librarySourceFilePath = CreateFile("LibraryRepo", "Project", "Assertions.cs");
        var searchDirectoryPaths = new Dictionary<Assembly, string>
            {
                [testsAssembly] = CreateDirectory("TestsRepo", "bin"),
                [libraryAssembly] = CreateDirectory("LibraryRepo", "bin")
            };
        var sut = new DeterministicSourcePathResolver(assembly => new[] { searchDirectoryPaths[assembly!] });

        Assert.AreEqual(testsSourceFilePath, Path.GetFullPath(sut.Resolve("/_/Project/Assertions.cs", testsAssembly)));
        Assert.AreEqual(librarySourceFilePath, Path.GetFullPath(sut.Resolve("/_/Project/Assertions.cs", libraryAssembly)));
    }

    string CreateFile(params string[] pathSegments)
    {
        var filePath = Path.Combine(pathSegments.Prepend(tempDirectoryPath).ToArray());
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, string.Empty);
        return filePath;
    }

    string CreateDirectory(params string[] pathSegments)
    {
        return Directory.CreateDirectory(Path.Combine(pathSegments.Prepend(tempDirectoryPath).ToArray())).FullName;
    }
}