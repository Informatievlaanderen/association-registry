namespace AssociationRegistry.Test.Projections.Beheer.Detail.Adressen;

using Admin.ProjectionHost.Projections.Detail;
using Formats;
using Scenario.Adressen;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdGewijzigdInAdressenregister(
    BeheerDetailScenarioFixture<AdresWerdGewijzigdInAdressenregisterScenario> fixture)
    : BeheerDetailScenarioClassFixture<AdresWerdGewijzigdInAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Adres_Properties_Are_Correctly_Set()
    {
        var adresWerdGewijzigdInAdressenregister = fixture.Scenario.AdresWerdGewijzigdInAdressenregister;

        var locatie = fixture.Result.Locaties.Single(x => x.LocatieId == adresWerdGewijzigdInAdressenregister.LocatieId);

        locatie.AdresId.Should().BeEquivalentTo(adresWerdGewijzigdInAdressenregister.AdresId);
        locatie.VerwijstNaar.Should().BeEquivalentTo(BeheerVerenigingDetailMapper.MapAdresVerwijzing(adresWerdGewijzigdInAdressenregister.AdresId));
        locatie.Adresvoorstelling.Should().BeEquivalentTo(adresWerdGewijzigdInAdressenregister.Adres.ToAdresString());
        locatie.Adres.Should().BeEquivalentTo(BeheerVerenigingDetailMapper.MapAdres(adresWerdGewijzigdInAdressenregister.Adres, adresWerdGewijzigdInAdressenregister.VCode, adresWerdGewijzigdInAdressenregister.LocatieId));
    }
}
