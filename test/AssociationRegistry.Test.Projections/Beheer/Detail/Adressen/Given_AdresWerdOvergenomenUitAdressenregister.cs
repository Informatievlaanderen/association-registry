namespace AssociationRegistry.Test.Projections.Beheer.Detail.Adressen;

using Admin.ProjectionHost.Projections.Detail;
using Formats;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOvergenomenUitAdressenregister(
    BeheerDetailScenarioFixture<AdresWerdOvergenomenUitAdressenregisterScenario> fixture)
    : BeheerDetailScenarioClassFixture<AdresWerdOvergenomenUitAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Adres_Properties_Are_Correctly_Set()
    {
        var scenarioAdresWerdOvergenomenUitAdressenregister = fixture.Scenario.AdresWerdOvergenomenUitAdressenregister;

        var locatie = fixture.Result.Locaties.Single(x => x.LocatieId == scenarioAdresWerdOvergenomenUitAdressenregister.LocatieId);

        locatie.AdresId.Should().BeEquivalentTo(scenarioAdresWerdOvergenomenUitAdressenregister.AdresId);
        locatie.VerwijstNaar.Should().BeEquivalentTo(BeheerVerenigingDetailMapper.MapAdresVerwijzing(scenarioAdresWerdOvergenomenUitAdressenregister.AdresId));
        locatie.Adresvoorstelling.Should().BeEquivalentTo(scenarioAdresWerdOvergenomenUitAdressenregister.Adres.ToAdresString());
        locatie.Adres.Should().BeEquivalentTo(BeheerVerenigingDetailMapper.MapAdres(scenarioAdresWerdOvergenomenUitAdressenregister.Adres, scenarioAdresWerdOvergenomenUitAdressenregister.VCode, scenarioAdresWerdOvergenomenUitAdressenregister.LocatieId));
    }
}
