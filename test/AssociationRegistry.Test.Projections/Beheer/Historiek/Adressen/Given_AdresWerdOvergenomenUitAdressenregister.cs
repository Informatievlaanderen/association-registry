namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOvergenomenUitAdressenregister(
    BeheerHistoriekScenarioFixture<AdresWerdOvergenomenUitAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresWerdOvergenomenUitAdressenregisterScenario>
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
                                               Beschrijving: "Adres werd overgenomen uit het adressenregister.",
                                               nameof(AdresWerdOvergenomenUitAdressenregister),
                                               fixture.Scenario.AdresWerdOvergenomenUitAdressenregister,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
