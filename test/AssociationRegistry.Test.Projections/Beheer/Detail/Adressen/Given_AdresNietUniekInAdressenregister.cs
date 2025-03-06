namespace AssociationRegistry.Test.Projections.Beheer.Detail.Adressen;

using Scenario.Adressen;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_AdresNietUniekInAdressenregister(
    BeheerDetailScenarioFixture<AdresNietUniekInAdressenregisterScenario> fixture)
    : BeheerDetailScenarioClassFixture<AdresNietUniekInAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void AdresId_And_VerwijstNaar_Is_Null()
    {
        var locatie = fixture.Result.Locaties.Single(x => x.LocatieId == fixture.Scenario.AdresNietUniekInAdressenregister.LocatieId);
        locatie.AdresId.Should().BeNull();
        locatie.VerwijstNaar.Should().BeNull();
        locatie.Adres.Should().NotBeNull();
    }
}
