namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet;

using FluentAssertions;
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
        await Assert.AllAsync(VerwachteDubbels, async verwachteDubbel =>
        {
            var duplicates = await GetDuplicatesFor(verwachteDubbel.TeRegistrerenNaam);

            duplicates.Select(x => x.Naam).Should().Contain(verwachteDubbel.Naam, because: $"Expected '{verwachteDubbel.Naam}' to be found in all duplicate names, but found mismatch.");
        });
    }
}
