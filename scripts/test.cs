#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.11.0
using DotNetDo;
using static DotNetDo.Tools;

[assembly: TaskDescription("Build the solution and run the tests.")]

await DotNet.Test;
