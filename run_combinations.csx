#r "nuget: System.Diagnostics.Process"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

// adjust these to match your exact class-name substrings:
const string WhenSearching = "When_Searching";
var OtherGroups = new[]
{
    "DetailAll",
    "Formatting_When_Formatting_An_Adres",
    "Given_an_Event_That_Is_Not_Handled",
    "Mapping",
    "Queries",
    "When_fetching_the_documentation",
    "When_Retrieving_Detail",
    "When_Retrieving_Detail_vereniging_context_json",
    "When_Retrieving_HoofdactiviteitenLijst",
    "When_Saving_A_Document_To_Elastic"
};

var scriptArgs = Environment
  .GetCommandLineArgs()
  .Skip(1)
  .ToArray();

if (scriptArgs.Length != 1)
{
    Console.WriteLine("Usage: dotnet script run_combinations.csx <path-to-.csproj>");
    return;
}

var project = scriptArgs[0];

var failures = new List<string>();

async Task<int> RunCombination(string group)
{
    var filter = $"FullyQualifiedName~{WhenSearching}|FullyQualifiedName~{group}";
    Console.WriteLine($"\n=== Running When_Searching + {group} ===");

    var psi = new ProcessStartInfo("dotnet", $"test \"{project}\" --no-build --filter \"{filter}\"")
    {
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute        = false,
        CreateNoWindow         = true,
    };
    var proc = Process.Start(psi)!;
    var stdout = await proc.StandardOutput.ReadToEndAsync();
    var stderr = await proc.StandardError.ReadToEndAsync();
    proc.WaitForExit();

    Console.WriteLine(stdout);
    if (proc.ExitCode != 0)
    {
        Console.WriteLine($"❌ Combination failed: When_Searching + {group}");
        return 1;
    }
    else
    {
        Console.WriteLine($"✔ Combination passed: When_Searching + {group}");
        return 0;
    }
}

foreach (var group in OtherGroups)
{
    var result = await RunCombination(group);
    if (result != 0)
        failures.Add(group);
}

Console.WriteLine("\n=== Summary ===");
if (failures.Count == 0)
{
    Console.WriteLine("All combinations passed!");
}
else
{
    Console.WriteLine("Failures occurred with the following groups:");
    foreach (var f in failures)
        Console.WriteLine($" - {f}");
}
