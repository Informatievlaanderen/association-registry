namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Dubbels;

using Admin.Schema.Historiek;
using Events;
using Scenario.Dubbels;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerenigingWerdGecorrigeerd(BeheerHistoriekScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario>
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
                                               Beschrijving: $"Markering als dubbel van vereniging {fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd.VCode} werd gecorrigeerd.",
                                               nameof(MarkeringDubbeleVerengingWerdGecorrigeerd),
                                               new
                                               {
                                                   VCode = fixture.Scenario.VCode,
                                                   VCodeAuthentiekeVereniging = fixture.Scenario.AuthentiekeVerenigingWerdGeregistreerd.VCode,
                                               },
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
