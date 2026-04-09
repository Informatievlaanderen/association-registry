namespace AssociationRegistry.Test.Projections.Beheer.Historiek.VertegenwoordigerPersoonsgegevens.When_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd.Kbo;

using Admin.ProjectionHost.Projections.Historiek;
using Admin.Schema.Historiek;
using Admin.Schema.Historiek.EventData;
using Events;
using Scenario.VertegenwoordigerPersoonsgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderdUitKBO(
    BeheerHistoriekScenarioFixture<VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdVerwijderdUitKBOScenario> fixture
)
    : BeheerHistoriekScenarioClassFixture<VrtPersoonsgegevensWerdenGeanonimiseerdAfterVertegenwoordigerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(4);

    [Fact]
    public void Persoonsgegevens_Are_Anonymized_For_Verwijderd()
    {
        var gebeurtenis = fixture.Result.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderdUitKBO)
        );

        gebeurtenis
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdVerwijderdUitKBO,
                    nameof(VertegenwoordigerWerdVerwijderdUitKBO),
                    KBOVertegenwoordigerData
                        .Create(fixture.Scenario.VertegenwoordigerWerdVerwijderdUitKBO)
                        .MakeAnonymous(),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
    }

    [Fact]
    public void Persoonsgegevens_Are_Anonymized_For_Overgenomen()
    {
        var gebeurtenis = fixture.Result.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VertegenwoordigerWerdOvergenomenUitKBO)
        );

        gebeurtenis
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: BeheerHistoriekBeschrijvingen.VertegenwoordigerWerdOvergenomenUitKBO,
                    nameof(VertegenwoordigerWerdOvergenomenUitKBO),
                    KBOVertegenwoordigerData
                        .Create(fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO)
                        .MakeAnonymous(),
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
    }
}
