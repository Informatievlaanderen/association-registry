namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using FluentAssertions;
using Seed;
using Xunit;
using Xunit.Abstractions;

public class Then_Some_Duplicates_Are_Expected: DuplicateDetectionTest
{

    public Then_Some_Duplicates_Are_Expected(ITestOutputHelper helper) : base("duplicates", helper)
    {
    }

    [Fact]
    public async Task With_Expected_Vereniging_In_Duplicate_List()
    {
        var failures = new List<string>();

        foreach (var dubbelDetectieData in DubbelDetectieData)
        {
            foreach (var verwachteDubbel in dubbelDetectieData.TeRegistrerenDubbelNamen)
            {
                try
                {
                    var duplicates = await GetDuplicatesFor(verwachteDubbel);

                    duplicates.Select(x => x.Naam).Should().Contain(dubbelDetectieData.GeregistreerdeNaam, because: $"Expected '{dubbelDetectieData.GeregistreerdeNaam}' to be found in all duplicate names, but found mismatch.");
                }
                catch (Exception ex)
                {
                    failures.Add($"Failed for '{verwachteDubbel}': {ex.Message}");
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
