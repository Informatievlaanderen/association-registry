namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Koepels;

using Admin.Schema.Historiek;
using Events;
using Scenario.Koepels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGeidentificeerdAlsGeenKoepel(
    BeheerHistoriekScenarioFixture<VerenigingWerdGeidentificeerdAlsGeenKoepelScenario> fixture
) : BeheerHistoriekScenarioClassFixture<VerenigingWerdGeidentificeerdAlsGeenKoepelScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(5);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.TakeLast(2)
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    new BeheerVerenigingHistoriekGebeurtenis(
                        Beschrijving: "Vereniging werd geïdentificeerd als niet-koepel.",
                        nameof(VerenigingWerdGeidentificeerdAlsGeenKoepel),
                        fixture.Scenario.VerenigingWerdGeidentificeerdAlsGeenKoepel,
                        fixture.MetadataInitiator,
                        fixture.MetadataTijdstip
                    ),
                    new BeheerVerenigingHistoriekGebeurtenis(
                        Beschrijving: "Privacyverklaring voor koepels werd ingetrokken.",
                        nameof(KoepelPrivacyverklaringWerdIngetrokken),
                        fixture.Scenario.KoepelPrivacyverklaringWerdIngetrokken,
                        fixture.MetadataInitiator,
                        fixture.MetadataTijdstip
                    ),
                },
                options => options.WithStrictOrdering()
            );
}
