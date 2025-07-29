namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Locaties;

using Admin.Schema;
using Admin.Schema.Search;
using Events;
using Formats;
using JsonLdContext;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdToegevoegd(
    BeheerZoekenScenarioFixture<LocatieWerdToegevoegdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<LocatieWerdToegevoegdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var locatieWerdToegevoegd = fixture.Scenario.LocatieWerdToegevoegd;
        var vCode = fixture.Scenario.VCode;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatieWerdToegevoegd.Locatie.LocatieId);

        actual.Locatietype.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Naam);
        actual.IsPrimair.Should().Be(locatieWerdToegevoegd.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, locatieWerdToegevoegd);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }

    private static void VerifyJsonLdMetadata(VerenigingZoekDocument.Types.Locatie actual, string vCode, LocatieWerdToegevoegd locatieWerdToegevoegd)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieWerdToegevoegd.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });
    }
}
