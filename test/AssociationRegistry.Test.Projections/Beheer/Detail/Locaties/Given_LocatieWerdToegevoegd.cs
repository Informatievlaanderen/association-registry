namespace AssociationRegistry.Test.Projections.Beheer.Detail.Locaties;

using Admin.Schema;
using Admin.Schema.Detail;
using Events;
using Formats;
using JsonLdContext;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdToegevoegd(
    BeheerDetailScenarioFixture<LocatieWerdToegevoegdScenario> fixture)
    : BeheerDetailScenarioClassFixture<LocatieWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        var locatieWerdToegevoegd = fixture.Scenario.LocatieWerdToegevoegd;
        var vCode = fixture.Scenario.VCode;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatieWerdToegevoegd.Locatie.LocatieId);

        actual.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, locatieWerdToegevoegd);
    }

    private static void VerifyJsonLdMetadata(Locatie actual, string vCode, LocatieWerdToegevoegd locatieWerdToegevoegd)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatieWerdToegevoegd.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });

        actual.Adres.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Adres.CreateWithIdValues(vCode, locatieWerdToegevoegd.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Adres.Type,
        });

        actual.VerwijstNaar.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                locatieWerdToegevoegd.Locatie.AdresId?.Bronwaarde.Split('/').Last()),
            Type = JsonLdType.AdresVerwijzing.Type,
        });
    }
}
