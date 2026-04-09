namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Vzer;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens;
using Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VzerWerdGeregistreerd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVzerWerdGeregistreerdScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVzerWerdGeregistreerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void Persoonsgegevens_Are_Anonymized() =>
        fixture
            .Result.Gebeurtenissen.First()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: $"Vereniging zonder eigen rechtspersoonlijkheid werd geregistreerd met naam '{fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam}'.",
                    nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd),
                    VerenigingWerdGeregistreerdData
                        .Create(fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
                        .MakeAnonymous(fixture.Scenario.VertegenwoordigerIdDieGeanonimiseerdWerd),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
