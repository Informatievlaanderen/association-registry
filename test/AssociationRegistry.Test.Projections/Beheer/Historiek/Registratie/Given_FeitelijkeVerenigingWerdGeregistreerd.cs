namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Registratie;

using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGeregistreerd(
    BeheerHistoriekScenarioFixture<FeitelijkeVerenigingWerdGeregistreerdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<FeitelijkeVerenigingWerdGeregistreerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(1);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"Feitelijke vereniging werd geregistreerd met naam '{fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam}'.",
                                               nameof(FeitelijkeVerenigingWerdGeregistreerd),
                                               VerenigingWerdGeregistreerdData.Create(fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd),
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
