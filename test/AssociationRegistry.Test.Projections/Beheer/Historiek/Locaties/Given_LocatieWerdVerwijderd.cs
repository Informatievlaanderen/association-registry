namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Locaties;

using Admin.Schema.Historiek;
using Events;
using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdVerwijderd(
    BeheerHistoriekScenarioFixture<LocatieWerdVerwijderdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LocatieWerdVerwijderdScenario>
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
                                               $"'{fixture.Scenario.LocatieWerdVerwijderd.Locatie.Locatietype}' locatie '{fixture.Scenario.LocatieWerdVerwijderd.Locatie.Naam}' werd verwijderd.",
                                               nameof(LocatieWerdVerwijderd),
                                               fixture.Scenario.LocatieWerdVerwijderd.Locatie,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
