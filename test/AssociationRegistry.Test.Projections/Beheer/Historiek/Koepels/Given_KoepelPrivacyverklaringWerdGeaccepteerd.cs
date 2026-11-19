namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Koepels;

using Admin.Schema.Historiek;
using Events;
using Scenario.Koepels;

[Collection(nameof(ProjectionContext))]
public class Given_KoepelPrivacyverklaringWerdGeaccepteerd(
    BeheerHistoriekScenarioFixture<KoepelPrivacyverklaringWerdGeaccepteerdScenario> fixture
) : BeheerHistoriekScenarioClassFixture<KoepelPrivacyverklaringWerdGeaccepteerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: "Privacyverklaring voor koepels werd geaccepteerd.",
                    nameof(KoepelPrivacyverklaringWerdGeaccepteerd),
                    fixture.Scenario.KoepelPrivacyverklaringWerdGeaccepteerd,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
