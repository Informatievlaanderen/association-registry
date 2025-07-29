namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Lidmaatschap;


using Events;
using Formats;
using JsonLdContext;
using Scenario.Lidmaatschappen;
using Public.Schema.Detail;
using Public.Schema.Search;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd(
    PubliekZoekenScenarioFixture<LidmaatschapWerdGewijzigdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var lidmaatschap = fixture.Scenario.LidmaatschapWerdGewijzigd;
        var vCode = fixture.Scenario.VCode;
        var actual = fixture.Result.Lidmaatschappen.First(x => x.LidmaatschapId == lidmaatschap.Lidmaatschap.LidmaatschapId);

        actual.Beschrijving.Should().BeEquivalentTo(lidmaatschap.Lidmaatschap.Beschrijving);
        actual.Identificatie.Should().BeEquivalentTo(lidmaatschap.Lidmaatschap.Identificatie);
        actual.AndereVereniging.Should().BeEquivalentTo(lidmaatschap.Lidmaatschap.AndereVereniging);
        actual.DatumVan.Should().BeEquivalentTo(lidmaatschap.Lidmaatschap.DatumVan.FormatAsBelgianDate());
        actual.DatumTot.Should().BeEquivalentTo(lidmaatschap.Lidmaatschap.DatumTot.FormatAsBelgianDate());

        VerifyJsonLdMetadata(actual, vCode, lidmaatschap);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Lidmaatschappen.Select(x => x.LidmaatschapId).Should().BeInAscendingOrder();
    }

    private static void VerifyJsonLdMetadata(VerenigingZoekDocument.Types.Lidmaatschap actual, string vCode, LidmaatschapWerdGewijzigd lidmaatschap)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Lidmaatschap.CreateWithIdValues(vCode, lidmaatschap.Lidmaatschap.LidmaatschapId.ToString()),
            Type = JsonLdType.Lidmaatschap.Type,
        });
    }
}
