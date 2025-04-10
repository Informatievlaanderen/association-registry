namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresNietUniekInAdressenregister(
    BeheerHistoriekScenarioFixture<AdresNietUniekInAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresNietUniekInAdressenregisterScenario>
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
                                               Beschrijving: "Adres niet uniek in het adressenregister.",
                                               nameof(AdresNietUniekInAdressenregister),
                                               fixture.Scenario.AdresNietUniekInAdressenregister,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
