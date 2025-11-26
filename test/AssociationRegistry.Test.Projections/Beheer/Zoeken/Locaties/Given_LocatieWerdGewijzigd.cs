namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Locaties;

using Admin.Schema;
using Admin.Schema.Search;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdGewijzigd(
    BeheerZoekenScenarioFixture<LocatieWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<LocatieWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var locatieWerdGewijzigd = fixture.Scenario.LocatieWerdGewijzigd;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatieWerdGewijzigd.Locatie.LocatieId);
        var vCode = fixture.Scenario.AggregateId;

        actual.Locatietype.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Naam);
        actual.IsPrimair.Should().Be(locatieWerdGewijzigd.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, locatieWerdGewijzigd);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }

    private static void VerifyJsonLdMetadata(VerenigingZoekDocument.Types.Locatie actual, string vCode, LocatieWerdGewijzigd locatieWerdGewijzigd)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieWerdGewijzigd.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });
    }
}
