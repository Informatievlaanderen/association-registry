namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Kbo;

using Admin.ProjectionHost.Projections.Historiek;
using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.VertegenwoordigerPersoonsgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegdVanuitKBO(
    BeheerHistoriekScenarioFixture<VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdVanuitKBOScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Persoonsgegevens_Are_Anonymized()
    {
        var gebeurtenis = fixture.Result.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegdVanuitKBO)
        );

        gebeurtenis
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdToegevoegdVanuitKBO,
                    nameof(VertegenwoordigerWerdToegevoegdVanuitKBO),
                    KBOVertegenwoordigerData
                        .Create(fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO)
                        .MakeAnonymous(),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
    }
}
