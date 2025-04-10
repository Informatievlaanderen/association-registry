namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Locaties;

using Admin.Schema.Historiek;
using Events;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdGewijzigd(
    BeheerHistoriekScenarioFixture<LocatieWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LocatieWerdGewijzigdScenario>
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
                                               Beschrijving: $"'{fixture.Scenario.LocatieWerdGewijzigd.Locatie.Locatietype}' locatie '{fixture.Scenario.LocatieWerdGewijzigd.Locatie.Naam}' werd gewijzigd.",
                                               nameof(LocatieWerdGewijzigd),
                                               fixture.Scenario.LocatieWerdGewijzigd.Locatie,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
