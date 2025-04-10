namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Hoofdactiviteiten;

using Admin.Schema.Historiek;
using Events;
using Scenario.Hoofdactiviteiten;

[Collection(nameof(ProjectionContext))]
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
    BeheerHistoriekScenarioFixture<HoofdactiviteitenVerenigingsloketWerdenGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<HoofdactiviteitenVerenigingsloketWerdenGewijzigdScenario>
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
                                               Beschrijving: $"Hoofdactiviteiten verenigingsloket werden gewijzigd.",
                                               nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
                                               fixture.Scenario.HoofdactiviteitenVerenigingsloketWerdenGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
