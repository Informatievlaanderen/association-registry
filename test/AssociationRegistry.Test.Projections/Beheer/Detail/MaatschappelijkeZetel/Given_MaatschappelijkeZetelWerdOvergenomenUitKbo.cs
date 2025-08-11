namespace AssociationRegistry.Test.Projections.Beheer.Detail.MaatschappelijkeZetel;

using Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo(
    BeheerDetailScenarioFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId);
        var vCode = fixture.Scenario.VCode;

        actual.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie);
        actual.Adresvoorstelling.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, maatschappelijkeZetelWerdOvergenomenUitKbo);
    }

    private static void VerifyJsonLdMetadata(Locatie actual, string vCode, MaatschappelijkeZetelWerdOvergenomenUitKbo locatie)
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
