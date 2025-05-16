namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using DuplicateVerenigingDetection;
using FluentAssertions;
using Newtonsoft.Json;
using Seed;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

public class Then_Some_Duplicates_Are_Expected: DuplicateDetectionTest
{
    private readonly ITestOutputHelper _helper;

    public Then_Some_Duplicates_Are_Expected(ITestOutputHelper helper) : base("duplicates", helper)
    {
        _helper = helper;
    }

    [Fact (Skip = "to replace with a singular duplicate test strategy")]
    public async Task GetDuplicatesFor()
    {
        var actual = await GetDuplicatesFor("vereniging");
        actual.First().Naam.Should().Be("vereniging");
    }

    [Fact (Skip = "to replace with a singular duplicate test strategy")]
    public async Task With_Expected_Vereniging_In_Duplicate_List()
    {
        var missedFailures = new List<string>();
        var unnecessaryFailures = new List<string>();
        var foundScores = new List<double>();
        var unnecessaryScores = new List<double>();

        IReadOnlyCollection<DuplicaatVereniging> duplicates = null;

        foreach (var dubbelDetectieData in DubbelDetectieData)
        {
            foreach (var verwachteDubbel in dubbelDetectieData.TeRegistrerenDubbelNamen)
            {
                try
                {
                    duplicates = await GetDuplicatesFor(verwachteDubbel);

                    duplicates.Select(x => x.Naam).Should().Contain(dubbelDetectieData.GeregistreerdeNaam, because: $"Expected '{dubbelDetectieData.GeregistreerdeNaam}' to be found in all duplicate names, but found mismatch.");

                    var okDuplicate = duplicates.SingleOrDefault(x => x.Naam == dubbelDetectieData.GeregistreerdeNaam);

                    if (okDuplicate is not null)
                    {
                        foundScores.Add(okDuplicate.Scoring.Score.Value);
                    }
                }
                catch (Exception ex)
                {
                    missedFailures.Add($"Failed for '{verwachteDubbel}': {ex.Message}");
                }
            }

            foreach (var verwachteNietDubbel in dubbelDetectieData.TeRegistrerenGeenDubbelNamen)
            {
                try
                {
                    duplicates = await GetDuplicatesFor(verwachteNietDubbel);
                    duplicates = duplicates.Where(x => x.Naam  != verwachteNietDubbel).ToList();
                    duplicates.Should().BeEmpty($"Found duplicates: {string.Join(", ", duplicates.Select(x => x.Naam))}");
                    //duplicates.Select(x => x.Naam).Should().NotContain(dubbelDetectieData.GeregistreerdeNaam, because: $"Expected '{dubbelDetectieData.GeregistreerdeNaam}' not to be found in all duplicate names.");
                }
                catch (Exception ex)
                {
                    unnecessaryFailures.Add($"Failed for '{verwachteNietDubbel}': \nDuplicates: {string.Join(", ", duplicates.Select(x => $"{x.Naam} {JsonConvert.SerializeObject(x.Scoring.Explanation)}"))}");
                    var okDuplicate = duplicates.SingleOrDefault(x => x.Naam == dubbelDetectieData.GeregistreerdeNaam);

                    foreach (var duplicaatVereniging in duplicates)
                    {
                            unnecessaryScores.Add(duplicaatVereniging.Scoring.Score.Value);
                    }

                }
            }

        }

        _helper.WriteLine($"Missed: ({missedFailures.Count})");
        _helper.WriteLine($"Unnecessary: ({unnecessaryFailures.Count})");
        if (missedFailures.Any())
        {
            var failureMessage = string.Join(Environment.NewLine, missedFailures);
            _helper.WriteLine($"The following verifications failed({missedFailures.Count}):{Environment.NewLine}{failureMessage}");

            failureMessage = string.Join(Environment.NewLine, unnecessaryFailures);
            _helper.WriteLine($"The following verifications failed({unnecessaryFailures.Count}):{Environment.NewLine}{failureMessage}");
        }
        if(!foundScores.Any()) foundScores.Add(0);
        if(!unnecessaryScores.Any()) unnecessaryScores.Add(0);
        Assert.Fail($"Missed: ({missedFailures.Count}) AVG|MIN|MAX Score: {foundScores.Average()}|{foundScores.Min()}|{foundScores.Max()}|\n" +
                    $"Unnecessary: ({unnecessaryFailures.Count}) AVG|MIN|MAX Score: {unnecessaryScores.Average()}|{unnecessaryScores.Min()}|{unnecessaryScores.Max()}|\n");

    }

}
