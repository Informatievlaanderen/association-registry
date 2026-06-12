namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Adressen;

using Admin.Schema.Historiek;
using Events;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister(
    BeheerHistoriekScenarioFixture<AdresKonNietOvergenomenWordenUitAdressenregisterScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<AdresKonNietOvergenomenWordenUitAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Historiek_Saved_Adres_Kon_Worden_Overgenomen_()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               "Adres kon niet overgenomen worden uit het adressenregister.",
                                               nameof(AdresKonNietOvergenomenWordenUitAdressenregister),
                                               fixture.Scenario.AdresKonNietOvergenomenWordenUitAdressenregister,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
