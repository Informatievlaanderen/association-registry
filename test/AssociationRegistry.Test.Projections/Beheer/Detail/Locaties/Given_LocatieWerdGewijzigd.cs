namespace AssociationRegistry.Test.Projections.Beheer.Detail.Locaties;

using Admin.Schema;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdGewijzigd(
    BeheerDetailScenarioFixture<LocatieWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<LocatieWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        var locatieWerdGewijzigd = fixture.Scenario.LocatieWerdGewijzigd;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatieWerdGewijzigd.Locatie.LocatieId);
        var vCode = fixture.Scenario.AggregateId;

        actual.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, locatieWerdGewijzigd);
    }

    private static void VerifyJsonLdMetadata(Locatie actual, string vCode, LocatieWerdGewijzigd locatieWerdGewijzigd)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieWerdGewijzigd.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });

        actual.Adres.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Adres.CreateWithIdValues(vCode, locatieWerdGewijzigd.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Adres.Type,
        });

        actual.VerwijstNaar.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                locatieWerdGewijzigd.Locatie.AdresId?.Bronwaarde.Split('/').Last()),
            Type = JsonLdType.AdresVerwijzing.Type,
        });
    }
}
