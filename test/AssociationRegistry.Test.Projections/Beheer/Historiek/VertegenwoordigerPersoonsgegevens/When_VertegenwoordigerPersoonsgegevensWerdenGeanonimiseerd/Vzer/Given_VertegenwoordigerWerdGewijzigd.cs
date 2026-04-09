namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Vzer;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens;
using Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdGewijzigd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(4);

    [Fact]
    public void Persoonsgegevens_Are_Anonymized_Foreach_VertegenwoordigerWerdGewijzigd()
    {
        var vertegenwoordigerWerdGewijzigdGebeurtenissen = fixture
            .Result.Gebeurtenissen.Where(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigd))
            .ToList();

        vertegenwoordigerWerdGewijzigdGebeurtenissen.ForEach(x =>
            x.Should()
                .BeEquivalentTo(
                    new BeheerVerenigingHistoriekGebeurtenis(
                        Beschrijving: BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdGewijzigd,
                        nameof(VertegenwoordigerWerdGewijzigd),
                        VertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdGewijzigd).MakeAnonymous(),
                        fixture.MetadataInitiator,
                        fixture.MetadataTijdstip
                    )
                )
        );
    }

    [Fact]
    public void Persoonsgegevens_Are_Anonymized_For_Geregistreerd() =>
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
