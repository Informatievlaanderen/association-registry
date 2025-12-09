namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Vertegenwoordigers.Vzer;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(
    BeheerHistoriekScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario>
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
                                               Beschrijving:$"Vertegenwoordiger '{fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend.Voornaam} {fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend.Achternaam}' werd niet teruggevonden uit KSZ en werd verwijderd.",
                                               nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend),
                                               VertegenwoordigerWerdVerwijderdData.Create(fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend),
                                                                            fixture.MetadataInitiator,
                                                                            fixture.MetadataTijdstip));
}
