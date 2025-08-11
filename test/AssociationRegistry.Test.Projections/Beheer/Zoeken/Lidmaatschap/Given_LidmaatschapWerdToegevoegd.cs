namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Lidmaatschap;

using Admin.Schema;
using Admin.Schema.Search;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd(
    BeheerZoekenScenarioFixture<LidmaatschapWerdToegevoegdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var lidmaatschapWerdToegevoegd = fixture.Scenario.LidmaatschapWerdToegevoegdFirst;
        var vCode = fixture.Scenario.VCode;
        var actual = fixture.Result.Lidmaatschappen.First(x => x.LidmaatschapId == lidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId);

        actual.Beschrijving.Should().BeEquivalentTo(lidmaatschapWerdToegevoegd.Lidmaatschap.Beschrijving);
        actual.Identificatie.Should().BeEquivalentTo(lidmaatschapWerdToegevoegd.Lidmaatschap.Identificatie);
        actual.AndereVereniging.Should().BeEquivalentTo(lidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging);
        actual.DatumVan.Should().BeEquivalentTo(lidmaatschapWerdToegevoegd.Lidmaatschap.DatumVan.FormatAsBelgianDate());
        actual.DatumTot.Should().BeEquivalentTo(lidmaatschapWerdToegevoegd.Lidmaatschap.DatumTot.FormatAsBelgianDate());

        VerifyJsonLdMetadata(actual, vCode, lidmaatschapWerdToegevoegd);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Lidmaatschappen.Select(x => x.LidmaatschapId).Should().BeInAscendingOrder();
    }

    private static void VerifyJsonLdMetadata(VerenigingZoekDocument.Types.Lidmaatschap actual, string vCode, LidmaatschapWerdToegevoegd lidmaatschap)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Lidmaatschap.CreateWithIdValues(vCode, lidmaatschap.Lidmaatschap.LidmaatschapId.ToString()),
            Type = JsonLdType.Lidmaatschap.Type,
        });
    }
}
