namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.MaatschappelijkeZetel;

using Admin.Schema;
using Admin.Schema.Search;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdGewijzigdInKbo(
    BeheerZoekenScenarioFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Only_Adres_Is_Updated()
    {
        var scenarioMaatschappelijkeZetelWerdGewijzigdInKbo = fixture.Scenario.MaatschappelijkeZetelWerdGewijzigdInKbo;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.LocatieId);
        var vCode = fixture.Scenario.AggregateId;

        actual.Locatietype.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Naam);
        actual.IsPrimair.Should().Be(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(scenarioMaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.ToAdresString());

        VerifyJsonLdMetadata(actual, vCode, scenarioMaatschappelijkeZetelWerdGewijzigdInKbo);
    }

    private static void VerifyJsonLdMetadata(VerenigingZoekDocument.Types.Locatie actual, string vCode, MaatschappelijkeZetelWerdGewijzigdInKbo locatie)
    {
        actual.JsonLdMetadata.Should().BeEquivalentTo(new JsonLdMetadata
        {
            Id = JsonLdType.Locatie.CreateWithIdValues(vCode, locatie.Locatie.LocatieId.ToString()),
            Type = JsonLdType.Locatie.Type,
        });
    }
}
