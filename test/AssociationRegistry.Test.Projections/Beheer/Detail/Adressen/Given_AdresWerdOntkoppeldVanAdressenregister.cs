﻿namespace AssociationRegistry.Test.Projections.Beheer.Detail.Adressen;

using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOntkoppeldVanAdressenregister(
    BeheerDetailScenarioFixture<AdresWerdOntkoppeldVanAdressenregisterScenario> fixture)
    : BeheerDetailScenarioClassFixture<AdresWerdOntkoppeldVanAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void AdresId_And_VerwijstNaar_Is_Null()
    {
        var locatie = fixture.Result.Locaties.Single(x => x.LocatieId == fixture.Scenario.AdresWerdOntkoppeldVanAdressenregister.LocatieId);
        locatie.AdresId.Should().BeNull();
        locatie.VerwijstNaar.Should().BeNull();
        locatie.Adres.Should().NotBeNull();
    }
}
