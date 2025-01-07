namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Dubbels;

using Admin.Schema.Historiek;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingAanvaarddeCorrectieDubbeleVereniging(BeheerHistoriekScenarioFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingAanvaarddeCorrectieDubbeleVerenigingScenario>
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
                                               Beschrijving: $"Vereniging {fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode} werd verwijderd als dubbel door correctie.",
                                               nameof(VerenigingAanvaarddeCorrectieDubbeleVereniging),
                                               new
                                               {
                                                   VCode = fixture.Scenario.VCode,
                                                   VCodeDubbeleVereniging = fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode,
                                               },
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
