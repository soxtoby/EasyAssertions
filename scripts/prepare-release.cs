#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.11.0
using System.Text.RegularExpressions;
using DotNetDo;
using Serilog;

[assembly: TaskDescription("Prepare the next release version and changelog.")]

if (Do.GitRepo.IsDirty)
    throw new InvalidOperationException("prepare-release requires a clean Git worktree.");

var projectFile = Do.RootDirectory / "EasyAssertions/EasyAssertions.csproj";
var changelogFile = Do.RootDirectory / "CHANGELOG.md";
var changelog = changelogFile.ReadText().Replace("\r\n", "\n");

var current = ParseProjectVersion(projectFile);
var latest = ParseLatestRelease(changelog);
if (latest != current)
    throw new InvalidOperationException($"Project version {current} does not match latest changelog release {latest?.ToString() ?? "<none>"}.");

var unreleasedMatch = ParseUnreleased(changelog);
var unreleased = unreleasedMatch.Groups["notes"].Value;
var next = InferBump(unreleased) switch
    {
        Bump.Major => new Version(current.Major + 1, 0, 0),
        Bump.Minor => new Version(current.Major, current.Minor + 1, 0),
        _ => new Version(current.Major, current.Minor, current.Build + 1),
    };

projectFile.WriteText(projectFile.ReadText().RegexReplace(
    "<Version>[^<]+</Version>",
    $"<Version>{next}</Version>"));

var before = changelog[..unreleasedMatch.Index];
var after = changelog[(unreleasedMatch.Index + unreleasedMatch.Length)..].TrimStart('\n');
var released = $"## Unreleased\n\n## v{next}\n\n{unreleased.Trim()}\n";
changelogFile.WriteText(before + released + (after.Length == 0 ? "" : "\n" + after.TrimEnd() + "\n"));

Log.Information("Next version: {Next}", next);

static Version ParseProjectVersion(AbsolutePath projectFile)
{
    var version = projectFile.ReadXml()
        .Descendants("Version")
        .Select(element => element.Value)
        .SingleOrDefault();
    return Version.TryParse(version, out var parsed)
        ? parsed
        : throw new InvalidOperationException("EasyAssertions.csproj has no valid Version.");
}

static Version? ParseLatestRelease(string changelog)
{
    var match = changelog.RegexMatch(@"(?m)^## v(?<version>\d+\.\d+\.\d+)\s*$");
    return match.Success ? Version.Parse(match.Groups["version"].Value) : null;
}

static Match ParseUnreleased(string changelog)
{
    var match = changelog.RegexMatch(@"(?ms)^## Unreleased\s*\n(?<notes>.*?)(?=^## |\z)");
    if (!match.Success || match.Groups["notes"].Value.IsNullOrWhiteSpace())
        throw new InvalidOperationException("CHANGELOG.md has no Unreleased notes.");
    return match;
}

static Bump InferBump(string notes)
{
    var headings = notes.RegexMatches(@"(?m)^### (?<heading>.+?)\s*$")
        .Select(match => match.Groups["heading"].Value)
        .ToArray();
    if (headings.None())
        throw new InvalidOperationException("Unreleased notes have no change headings.");

    var unknown = headings.Except(["Breaking", "Added", "Changed", "Fixed"], StringComparer.Ordinal).ToArray();
    if (unknown.Any())
        throw new InvalidOperationException($"Unknown Unreleased change heading: {unknown.JoinWith(", ")}.");

    return headings.Contains("Breaking", StringComparer.Ordinal) ? Bump.Major
        : headings.Contains("Added", StringComparer.Ordinal) || headings.Contains("Changed", StringComparer.Ordinal) ? Bump.Minor
        : Bump.Patch;
}

enum Bump
{
    Major, Minor, Patch
}
