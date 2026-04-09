namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Kbo;

using Admin.ProjectionHost.Projections.Historiek;
using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.VertegenwoordigerPersoonsgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdGewijzigdInKBO(
    BeheerHistoriekScenarioFixture<VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdInKBOScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdGewijzigdInKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(5);

    [Fact]
    public void Persoonsgegevens_Are_Anonymized_Foreach_VertegenwoordigerWerdGewijzigd()
    {
        var gebeurtenissen = fixture
            .Result.Gebeurtenissen.Where(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigdInKBO))
            .ToList();

        gebeurtenissen.ForEach(x =>
            x.Should()
                .BeEquivalentTo(
                    new BeheerVerenigingHistoriekGebeurtenis(
                        Beschrijving: BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdGewijzigdInKBO,
                        nameof(VertegenwoordigerWerdGewijzigdInKBO),
                        KBOVertegenwoordigerData
                            .Create(fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO)
                            .MakeAnonymous(),
                        fixture.MetadataInitiator,
                        fixture.MetadataTijdstip
                    )
                )
        );
    }

    [Fact]
    public void Persoonsgegevens_Are_Anonymized_For_Toegevoegd()
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
