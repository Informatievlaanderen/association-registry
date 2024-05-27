using System;
using System.IO;

var buildNumber = Environment.GetEnvironmentVariable("CI_BUILD_NUMBER") ?? "0.0.0";
var gitHash = Environment.GetEnvironmentVariable("GIT_HASH") ?? "unknown";
var product = "Basisregisters Vlaanderen";
var copyright = "Copyright (c) Vlaamse overheid";
var company = "Vlaamse overheid";

var content = $@"
using System.Reflection;

[assembly: AssemblyVersion(""{buildNumber}"")]
[assembly: AssemblyFileVersion(""{buildNumber}"")]
[assembly: AssemblyInformationalVersion(""{gitHash}"")]
[assembly: AssemblyProduct(""{product}"")]
[assembly: AssemblyCopyright(""{copyright}"")]
[assembly: AssemblyCompany(""{company}"")]
";

File.WriteAllText("SolutionInfo.cs", content);