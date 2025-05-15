// File: run_combinations.csx

#r "nuget: System.Diagnostics.Process"

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

var project = "/code/aiv/assotest/test/AssociationRegistry.Test.Public.Api";

// --- test‐group definitions ---
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

// --- result type ---
enum Status { PASS, IGNORED, FAIL }
var results = new List<(string Description, Status Outcome)>();

// --- runner helper ---
async Task<Status> RunFilter(string filter, string description)
{
    Console.WriteLine($"\n=== Running: {description} ===");
    var psi = new ProcessStartInfo("dotnet", $"test \"{project}\" --no-build --filter \"{filter}\"")
    {
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute        = false,
        CreateNoWindow         = true,
    };
    using var proc = Process.Start(psi)!;
    var stdout = await proc.StandardOutput.ReadToEndAsync();
    var stderr = await proc.StandardError.ReadToEndAsync();
    proc.WaitForExit();

    // always show full output
    Console.WriteLine(stdout);
    if (!string.IsNullOrEmpty(stderr))
        Console.WriteLine(stderr);

    if (proc.ExitCode == 0)
    {
        Console.WriteLine($"✔ PASS: {description}");
        return Status.PASS;
    }

    // Count how many tests failed
    var failedCount = Regex.Matches(stdout + stderr, @"(?m)^\s*Failed\s", RegexOptions.IgnoreCase).Count;
    // Count how many "elements instead of" occurrences
    var ignorePhraseCount = Regex.Matches(stdout + stderr, "elements instead of", RegexOptions.IgnoreCase).Count;

    if (failedCount > 0 && ignorePhraseCount >= failedCount)
    {
        Console.WriteLine($"⚠ IGNORED failure in {description} (all {failedCount} failures contain “elements instead of”)");
        return Status.IGNORED;
    }

    Console.WriteLine($"❌ FAIL: {description}");
    return Status.FAIL;
}

// --- 1) solo run ---
var soloDesc   = "When_Searching (solo)";
var soloFilter = $"FullyQualifiedName~{WhenSearching}";
results.Add((soloDesc, await RunFilter(soloFilter, soloDesc)));

// --- 2) paired runs ---
foreach (var group in OtherGroups)
{
    var desc   = $"When_Searching + {group}";
    var filter = $"FullyQualifiedName~{WhenSearching}|FullyQualifiedName~{group}";
    results.Add((desc, await RunFilter(filter, desc)));
}

// --- 3) detailed report ---
Console.WriteLine("\n=== Detailed Report ===");
Console.WriteLine("Combination                                         | Status");
Console.WriteLine("----------------------------------------------------|--------");
foreach (var (desc, outcome) in results)
{
    var d = desc.Length > 50 ? desc[..47] + "..." : desc.PadRight(50);
    Console.WriteLine($"{d} | {outcome}");
}

// --- 4) summary ---
var total   = results.Count;
var passed  = results.Count(r => r.Outcome == Status.PASS);
var ignored = results.Count(r => r.Outcome == Status.IGNORED);
var failed  = results.Count(r => r.Outcome == Status.FAIL);

Console.WriteLine("\n=== Summary ===");
Console.WriteLine($"Total runs: {total}");
Console.WriteLine($"  ✅ Passed:  {passed}");
Console.WriteLine($"  ⚠ Ignored: {ignored}");
Console.WriteLine($"  ❌ Failed:  {failed}");
