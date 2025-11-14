namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Dubbels;

using Admin.Schema.Historiek;
using Events;
using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGemarkeerdAlsDubbel(BeheerHistoriekScenarioFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario>
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
                                               Beschrijving: $"Vereniging werd gemarkeerd als dubbel van {fixture.Scenario.AuthentiekeVereniging.VCode}.",
                                               nameof(VerenigingWerdGemarkeerdAlsDubbelVan),
                                               new
                                               {
                                                   VCode = fixture.Scenario.VCode,
                                                   VCodeAuthentiekeVereniging = fixture.Scenario.AuthentiekeVereniging.VCode,
                                               },
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
