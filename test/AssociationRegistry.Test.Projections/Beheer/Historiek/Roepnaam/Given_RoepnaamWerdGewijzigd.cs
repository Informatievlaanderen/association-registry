namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Roepnaam;

using Admin.Schema.Historiek;
using Events;
using Scenario.Roepnaam;

[Collection(nameof(ProjectionContext))]
public class Given_RoepnaamWerdGewijzigd(
    BeheerHistoriekScenarioFixture<RoepnaamWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<RoepnaamWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Historiek_Saved_RoepnaamWerdGewijzigd()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"Roepnaam werd gewijzigd naar '{fixture.Scenario.RoepnaamWerdGewijzigd.Roepnaam}'.",
                                               nameof(RoepnaamWerdGewijzigd),
                                               fixture.Scenario.RoepnaamWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
