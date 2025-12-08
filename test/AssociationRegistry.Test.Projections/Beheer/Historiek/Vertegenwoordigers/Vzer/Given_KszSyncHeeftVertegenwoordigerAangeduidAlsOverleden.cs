namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
    BeheerHistoriekScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario>
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
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden.Voornaam} {fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden.Achternaam}' is overleden volgens KSZ en werd verwijderd.",
                                               nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden),
                                               KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenData.Create(fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
