namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Locaties;

using Admin.Schema.Historiek;
using Events;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdToegevoegd(
    BeheerHistoriekScenarioFixture<LocatieWerdToegevoegdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LocatieWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Historiek_Saved_Has_Expected_Values()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving:
                                               $"'{fixture.Scenario.LocatieWerdToegevoegd.Locatie.Locatietype}' locatie '{fixture.Scenario.LocatieWerdToegevoegd.Locatie.Naam}' werd toegevoegd.",
                                               nameof(LocatieWerdToegevoegd),
                                               fixture.Scenario.LocatieWerdToegevoegd.Locatie,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
