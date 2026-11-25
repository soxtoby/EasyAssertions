#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.11.0
using DotNetDo;
using static DotNetDo.Tools;

[assembly: TaskDescription("Pack the EasyAssertions NuGet package into dist/packages.")]

await (DotNet.Pack with
    {
        Targets = [Do.Solution["EasyAssertions"]],
        Output = (Do.RootDirectory / "dist/packages").RecreateDirectory()
    });
