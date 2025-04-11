namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Registratie;

using Admin.Schema.Historiek;
using Events;
using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    BeheerHistoriekScenarioFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario>
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
                                               Beschrijving: $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam}'.",
                                               nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
                                               fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
