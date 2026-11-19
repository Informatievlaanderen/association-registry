namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Koepels;

using Admin.Schema.Historiek;
using Events;
using Scenario.Koepels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGeidentificeerdAlsKoepel(
    BeheerHistoriekScenarioFixture<VerenigingWerdGeidentificeerdAlsKoepelScenario> fixture
) : BeheerHistoriekScenarioClassFixture<VerenigingWerdGeidentificeerdAlsKoepelScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated() =>
        fixture
            .Result.Gebeurtenissen.Last()
            .Should()
            .BeEquivalentTo(
                new BeheerVerenigingHistoriekGebeurtenis(
                    Beschrijving: "Vereniging werd geïdentificeerd als koepel.",
                    nameof(VerenigingWerdGeidentificeerdAlsKoepel),
                    fixture.Scenario.VerenigingWerdGeidentificeerdAlsKoepel,
                    fixture.MetadataInitiator,
                    fixture.MetadataTijdstip
                )
            );
}
