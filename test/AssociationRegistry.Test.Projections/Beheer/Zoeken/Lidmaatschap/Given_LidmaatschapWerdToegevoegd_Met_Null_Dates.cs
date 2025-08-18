namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Lidmaatschap;

using Admin.Schema;
using Contracts.JsonLdContext;
using Formats;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd_Met_Null_Dates(
    BeheerZoekenScenarioFixture<LidmaatschapWerdToegevoegdMetNullDatesScenario> fixture)
    : BeheerZoekenScenarioClassFixture<LidmaatschapWerdToegevoegdMetNullDatesScenario>
{
    [Fact]
    public void Supports_Empty_String_When_Dates_Are_Null()
    {
        var lidmaatschapWerdToegevoegd = fixture.Scenario.LidmaatschapWerdToegevoegdWithNullDates;
        var actual = fixture.Result.Lidmaatschappen.First(x => x.LidmaatschapId == lidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId);

        actual.DatumVan.Should().BeEmpty();
        actual.DatumTot.Should().BeEmpty();
    }
}
