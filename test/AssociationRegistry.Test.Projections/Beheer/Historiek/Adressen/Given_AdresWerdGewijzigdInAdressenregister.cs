namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdGewijzigdInAdressenregister(
    BeheerHistoriekScenarioFixture<AdresWerdGewijzigdInAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresWerdGewijzigdInAdressenregisterScenario>
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
                                               Beschrijving: "Adres werd gewijzigd in het adressenregister.",
                                               nameof(AdresWerdGewijzigdInAdressenregister),
                                               fixture.Scenario.AdresWerdGewijzigdInAdressenregister,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
