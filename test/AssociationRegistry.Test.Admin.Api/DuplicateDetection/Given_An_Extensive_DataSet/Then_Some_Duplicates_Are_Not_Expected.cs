namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using DuplicateVerenigingDetection;
using FluentAssertions;
using Seed;
using Xunit;
using Xunit.Abstractions;

public class Then_Some_Duplicates_Are_Not_Expected: DuplicateDetectionTest
{
    public Then_Some_Duplicates_Are_Not_Expected(ITestOutputHelper helper): base("uniques", helper)
    {
    }

    [Fact]
    public async Task With_Expected_Vereniging_In_Duplicate_List()
    {
        var failures = new List<string>();

        IReadOnlyCollection<DuplicaatVereniging> duplicates = null;

        foreach (var dubbelDetectieData in DubbelDetectieData)
        {
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
                    failures.Add($"Failed for '{verwachteNietDubbel}': \nDuplicates: {string.Join(", ", duplicates.Select(x => x.Naam))}");

                }
            }
        }

        if (failures.Any())
        {
            var failureMessage = string.Join(Environment.NewLine, failures);
            Assert.Fail($"The following verifications failed({failures.Count}):{Environment.NewLine}{failureMessage}");
        }
    }
}
