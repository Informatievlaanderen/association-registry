namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdNietGevondenInAdressenregister(
    BeheerHistoriekScenarioFixture<AdresWerdNietGevondenInAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresWerdNietGevondenInAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Adres kon niet gevonden worden in het adressenregister.",
                                               nameof(AdresWerdNietGevondenInAdressenregister),
                                               fixture.Scenario.AdresWerdNietGevondenInAdressenregister,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
