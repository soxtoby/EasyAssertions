#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.11.0
using System.Text.RegularExpressions;
using DotNetDo;
using static DotNetDo.Tools;

[assembly: TaskDescription("Publish the packed NuGet package and its GitHub release.")]

var tag = Do.GitHubActions?.Workflow.ReferenceName is { } defaultTag
    ? Do.Param("tag", defaultTag, "Release tag.").Value
    : Do.Param("tag").Required().Value;
var apiKey = Do.Secret("nuget_api_key", null, "Temporary NuGet API key.").Required();

var version = (Do.RootDirectory / "EasyAssertions/EasyAssertions.csproj").ReadXml()
    .Descendants("Version")
    .Select(element => element.Value)
    .SingleOrDefault()
    ?? throw new InvalidOperationException("EasyAssertions.csproj has no Version.");

var expectedTag = "v" + version;
if (tag != expectedTag)
    throw new InvalidOperationException($"Tag '{tag}' does not match project version '{expectedTag}'.");

var changelog = (Do.RootDirectory / "CHANGELOG.md").ReadText().Replace("\r\n", "\n");
var notesMatch = changelog.RegexMatch($@"(?ms)^## {Regex.Escape(tag)}\s*\n(?<notes>.*?)(?=^## |\z)");
if (!notesMatch.Success || notesMatch.Groups["notes"].Value.IsNullOrWhiteSpace())
    throw new InvalidOperationException($"CHANGELOG.md has no release notes for {tag}.");

var packages = Do.RootDirectory / "dist/packages";
var expected = $"EasyAssertions.{version}.nupkg";
var names = packages.GlobFiles("*.nupkg").Select(package => package.Name!).ToArray();
if (!names.SequenceEqual([expected], StringComparer.Ordinal))
    throw new InvalidOperationException($"Expected {expected} but found {names.JoinWith(", ")}. Run 'pack' before publishing the release.");
var package = packages / expected;

await (DotNet.NuGetPush with
    {
        Package = package,
        ApiKey = apiKey.Unwrap(),
        Source = "https://api.nuget.org/v3/index.json",
        SkipDuplicate = true,
    });

var notesFile = Do.CreateTempFile("easyassertions-release-notes-", ".md");
notesFile.WriteText(notesMatch.Groups["notes"].Value.Trim() + Environment.NewLine);
await Do.Exec($"gh release create {tag.QuotedArgument()} {package.QuotedArgument()} --title {tag.QuotedArgument()} --notes-file {notesFile.QuotedArgument()}");
