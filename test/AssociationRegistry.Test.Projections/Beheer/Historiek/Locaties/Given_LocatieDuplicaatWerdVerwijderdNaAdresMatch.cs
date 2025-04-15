namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Locaties;

using Admin.Schema.Historiek;
using AssociationRegistry.Test.Projections.Scenario.Locaties;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(
    BeheerHistoriekScenarioFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario>
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
                                               Beschrijving: $"Locatie '{fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.LocatieNaam}' met ID {fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.VerwijderdeLocatieId} werd verwijderd omdat de gegevens exact overeenkomen met locatie ID {fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch.BehoudenLocatieId}.",
                                               nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
                                               fixture.Scenario.LocatieDuplicaatWerdVerwijderdNaAdresMatch,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
