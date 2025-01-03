namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Dubbels;

using Admin.Schema.Historiek;
using Events;
using Vereniging;

[Collection(nameof(ProjectionContext))]
public class Given_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(BeheerHistoriekScenarioFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario>
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
                                               Beschrijving: $"Vereniging is geen dubbel meer van {fixture.Scenario.WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.VCodeAuthentiekeVereniging}.",
                                               nameof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt),
                                               new
                                               {
                                                   VCode = fixture.Scenario.VCode,
                                                   VorigeStatus = VerenigingStatus.Actief.StatusNaam,
                                               },
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
