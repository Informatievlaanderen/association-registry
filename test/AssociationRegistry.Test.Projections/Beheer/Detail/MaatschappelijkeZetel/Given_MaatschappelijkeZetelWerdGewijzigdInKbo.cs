namespace AssociationRegistry.Test.Projections.Beheer.Detail.MaatschappelijkeZetel;

using Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using Events;
using Formats;
using JsonLdContext;
using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdGewijzigdInKbo(
    BeheerDetailScenarioFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Only_Adres_Is_Updated()
    {
        var scenarioMaatschappelijkeZetelWerdGewijzigdInKbo = fixture.Scenario.MaatschappelijkeZetelWerdGewijzigdInKbo;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.LocatieId);
        var vCode = fixture.Scenario.VCode;

        actual.Adres.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres);
        actual.Adresvoorstelling.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, scenarioMaatschappelijkeZetelWerdGewijzigdInKbo);
    }

    private static void VerifyJsonLdMetadata(Locatie actual, string vCode, MaatschappelijkeZetelWerdGewijzigdInKbo locatie)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatie.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });

        actual.Adres.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Adres.CreateWithIdValues(vCode, locatie.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Adres.Type,
        });

        actual.VerwijstNaar.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                locatie.Locatie.AdresId?.Bronwaarde.Split('/').Last()),
            Type = JsonLdType.AdresVerwijzing.Type,
        });
    }
}
