namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.MaatschappelijkeZetel;

using Admin.Schema;
using Admin.Schema.Search;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo(
    BeheerZoekenScenarioFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId);
        var vCode = fixture.Scenario.VCode;

        actual.Locatietype.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam);
        actual.IsPrimair.Should().Be(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(maatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, maatschappelijkeZetelWerdOvergenomenUitKbo);
    }

    private static void VerifyJsonLdMetadata(VerenigingZoekDocument.Types.Locatie actual, string vCode, MaatschappelijkeZetelWerdOvergenomenUitKbo locatie)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatie.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });
    }
}
