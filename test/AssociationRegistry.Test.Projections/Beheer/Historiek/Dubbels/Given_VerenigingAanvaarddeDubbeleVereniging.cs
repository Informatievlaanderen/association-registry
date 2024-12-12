namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Dubbels;

using Admin.Schema.Historiek;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingAanvaarddeDubbeleVereniging(BeheerHistoriekScenarioFixture<VerenigingAanvaarddeDubbeleVerenigingScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingAanvaarddeDubbeleVerenigingScenario>
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
                                               Beschrijving: $"Vereniging {fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode} werd toegevoegd als dubbel.",
                                               nameof(VerenigingAanvaarddeDubbeleVereniging),
                                               new
                                               {
                                                   VCode = fixture.Scenario.VCode,
                                                   VCodeDubbeleVereniging = fixture.Scenario.DubbeleVerenigingWerdGeregistreerd.VCode,
                                               },
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
