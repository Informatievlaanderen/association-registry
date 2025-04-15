namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOntkoppeldVanAdressenregister(
    BeheerHistoriekScenarioFixture<AdresWerdOntkoppeldVanAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresWerdOntkoppeldVanAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Adres werd ontkoppeld van het adressenregister.",
                                               nameof(AdresWerdOntkoppeldVanAdressenregister),
                                               fixture.Scenario.AdresWerdOntkoppeldVanAdressenregister,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
