namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Fv;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens;
using Scenario.VertegenwoordigerPersoonsgegevens.Fv;

[Collection(nameof(ProjectionContext))]
public class Given_FVWerdGeregistreerd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterFvWerdGeregistreerdScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterFvWerdGeregistreerdScenario>
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
                    Beschrijving: $"Feitelijke vereniging werd geregistreerd met naam '{fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam}'.",
                    nameof(FeitelijkeVerenigingWerdGeregistreerd),
                    VerenigingWerdGeregistreerdData
                        .Create(fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd)
                        .MakeAnonymous(fixture.Scenario.VertegenwoordigerIdDieGeanonimiseerdWerd),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
