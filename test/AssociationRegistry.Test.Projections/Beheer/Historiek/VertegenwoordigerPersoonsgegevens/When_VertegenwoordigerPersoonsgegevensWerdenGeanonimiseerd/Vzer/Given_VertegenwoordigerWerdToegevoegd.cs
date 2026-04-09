namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Vzer;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens;
using Scenario.VertegenwoordigerPersoonsgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegd(
    BeheerHistoriekScenarioFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Persoonsgegevens_Are_Anonymized()
    {
        var vertegenwoordigerWerdToegevoegdGebeurtenis = fixture.Result.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegd)
        );

        vertegenwoordigerWerdToegevoegdGebeurtenis
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdToegevoegd,
                    nameof(VertegenwoordigerWerdToegevoegd),
                    VertegenwoordigerData.Create(fixture.Scenario.VertegenwoordigerWerdToegevoegd).MakeAnonymous(),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
    }
}
